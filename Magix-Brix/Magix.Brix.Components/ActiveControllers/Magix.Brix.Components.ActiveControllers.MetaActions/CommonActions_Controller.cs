/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.UX.Widgets;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Collections.Generic;
using System.Globalization;
using Magix.Brix.Components.ActiveTypes.MetaViews;
using System.IO;
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    /**
     * Level2: Contains common end user useful actions which doesn't really belong 
     * any particular place, but which can still be immensily useful for
     * 'scripting purposes'. Perceive these as 'plugins' ... ;) - [Or extra 
     * candy if you wish]. Often they're 'simplifications' of other more
     * 'hard core' Active Events
     */
    [ActiveController]
    public class CommonActions_Controller : ActiveController
    {
        /**
         * Level2: Returns the 'Name' setting for the current logged in User. Sets the
         * setting to 'Default' if no existing value is found. Both are mandatory params.
         * Returns new value as 'Value'
         */
        [ActiveEvent(Name = "Magix.Common.GetUserSetting")]
        protected void Magix_Common_GetUserSetting(object sender, ActiveEventArgs e)
        {
            string val = User.Current.GetSetting<string>(
                e.Params["Name"].Get<string>(),
                e.Params["Default"].Value.ToString());

            e.Params["Value"].Value = val;
        }

        /**
         * Sets the given setting 'Name' to the given value 'Value' for the currently
         * logged in User
         */
        [ActiveEvent(Name = "Magix.Common.SetUserSetting")]
        protected void Magix_Common_SetUserSetting(object sender, ActiveEventArgs e)
        {
            User.Current.SetSetting<string>(
                e.Params["Name"].Get<string>(),
                e.Params["Value"].Get<string>());
        }

        /**
         * Level2: Simplification of 'Magix.Core.SendEmail', will among
         * other things using the User.Current as sender unless explicitly
         * overridden. Will also unless to email address is given, send email to
         * yourself. Takes these parameters 'Header', 'Body', 'Email' [from email], 'From' [name]
         * 'To' [which can be a list of emails or one email]
         */
        [ActiveEvent(Name = "Magix.Common.SendEmail")]
        protected void Magix_Common_SendEmail(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Header"].Value =
                e.Params.Contains("Header") && !string.IsNullOrEmpty(e.Params["Header"].Get<string>()) ?
                    e.Params["Header"].Get<string>() :
                    "Message from Marvin ...";

            node["Body"].Value =
                e.Params.Contains("Body") && !string.IsNullOrEmpty(e.Params["Body"].Get<string>()) ?
                    e.Params["Body"].Get<string>() :
                    "Opps, someone forgot to attach the message ...";

            node["AdminEmail"].Value =
                e.Params.Contains("Email") && !string.IsNullOrEmpty(e.Params["Email"].Get<string>()) ?
                    e.Params["Email"].Get<string>() :
                    User.Current.Email;

            node["AdminEmailFrom"].Value =
                e.Params.Contains("From") && !string.IsNullOrEmpty(e.Params["From"].Get<string>()) ?
                    e.Params["From"].Get<string>() :
                    User.Current.FullName;

            if (e.Params.Contains("To"))
            {
                if (!string.IsNullOrEmpty(e.Params["To"].Get<string>()))
                {
                    node["EmailAddresses"]["only"].Value = e.Params["To"].Get<string>();
                }
                else
                {
                    node["EmailAddresses"].AddRange(e.Params["To"].UnTie());
                    if (node["EmailAddresses"][0].Value == null)
                    {
                        // Just in case this is a 'template action' with empty placeholders for end-user
                        // to fill in ...
                        node["EmailAddresses"][0].Value = User.Current.Email;
                    }
                }
            }
            else
            {
                // Sending yourself an email ...
                node["EmailAddresses"]["only"].Value = User.Current.Email;
            }

            RaiseEvent(
                "Magix.Core.SendEmail",
                node);
        }

        /**
         * Level1: Will serialize the Active SingleView Form and send an email from 'Email' and 'From' 
         * to the 'Email' field on the given form. Before the email is sendt both the 'Header' and
         * the 'Body' will substitute every single occurency of [x] with the MetaView's property 
         * with the same name [x]
         */
        [ActiveEvent(Name = "Magix.Common.SendEmailFromForm")]
        protected void Magix_Common_SendEmailFromForm(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("OriginalWebPartID"))
                throw new ArgumentException("This Action can only be raised from within a SingleView form");

            if (!e.Params.Contains("Email"))
                throw new ArgumentException("You do need at the very least a 'From Address' for the SendEmailFromForm Event to work");

            // Getting current MetaView content...
            Node node = new Node();
            node["OriginalWebPartID"].Value = e.Params["OriginalWebPartID"].Value;

            RaiseEvent(
                "Magix.MetaView.SerializeSingleViewForm",
                node);

            if (node.Count == 0)
            {
                throw new ArgumentException("There are no forms on the screen now that can be used for raising this action. This action can only be raised from within a Single View MetaView ...");
            }

            if (!node.Contains("Email"))
            {
                throw new ArgumentException("The SingleView MetaView must at the minimum contain at least one field, and this field must be called 'Email'. It will serve as the To Email Address ...");
            }

            node["OriginalWebPartID"].UnTie();

            string header = e.Params["Header"].Get<string>();
            string body = e.Params["Body"].Get<string>();

            // Substituting ...
            foreach (Node idx in node)
            {
                header = header.Replace(string.Format("[{0}]", idx.Name), idx.Value.ToString());
                body = body.Replace(string.Format("[{0}]", idx.Name), idx.Value.ToString());
            }

            e.Params["Header"].Value = header;
            e.Params["Body"].Value = body;

            // Have to rename from 'Email' [View Property Name] to 'To' which our Email logic understands ...
            e.Params["To"].Value = node["Email"].Value;

            RaiseEvent(
                "Magix.Common.SendEmail",
                e.Params);
        }

        /**
         * Level2: Will to a String.Replace on the given 'Source' or 'SourceNode'. Will replace 'OldString' or 'OldStringNode'
         * with 'NewString' or 'NewStringNode' and return the value either in 'Result' or 'ResultNode', direct
         * value [no 'Node' part] always have preference
         */
        [ActiveEvent(Name = "Magix.Common.ReplaceStringValue")]
        protected void Magix_Common_ReplaceStringValue(object sender, ActiveEventArgs e)
        {
            string source = 
                e.Params.Contains("Source") ? 
                    e.Params["Source"].Get<string>() : 
                    e.Params[e.Params["SourceNode"].Get<string>()].Get<string>();
            string oldString =
                e.Params.Contains("OldString") ?
                    e.Params["OldString"].Get<string>() :
                    e.Params[e.Params["OldStringNode"].Get<string>()].Get<string>();
            string newString =
                e.Params.Contains("NewString") ?
                    e.Params["NewString"].Get<string>() :
                    e.Params[e.Params["NewStringNode"].Get<string>()].Get<string>();

            string transformed = source.Replace(oldString, newString);

            if (e.Params.Contains("ResultNode"))
                e.Params[e.Params["ResultNode"].Get<string>()].Value = transformed;
            else
                e.Params["Result"].Value = transformed;
        }

        /**
         * Level2: Will call 'Magix.MetaAction.RaiseAction' for every single 'ActionName' in the Actions [list]
         * Parameter. Useful for creating complex abstractions, doing multiple tasks at once or 'encapsulating'
         * your entire logic inside one Action
         */
        [ActiveEvent(Name = "Magix.Common.MultiAction")]
        protected void Magix_Common_MultiAction(object sender, ActiveEventArgs e)
        {
            foreach (Node idx in e.Params["Actions"])
            {
                Node eventNodes = idx;

                RaiseEvent(
                    "Magix.MetaAction.RaiseAction",
                    eventNodes);
            }
        }

        /**
         * Level2: Will rename the given 'FromName' to 'ToName'. Will throw exception if no 'FromName' exists,
         * or parameters are missing
         */
        [ActiveEvent(Name = "Magix.Common.RenameNode")]
        protected void Magix_Common_RenameNode(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("FromName") || !e.Params.Contains("ToName"))
                throw new ArgumentException("You need to specify both FromName and ToName to RenameNode ...");
            string fromName = e.Params["FromName"].Get<string>();

            if (e.Params.Contains(fromName))
                throw new ArgumentException("No such FromName in RenameNode ...");

            string toName = e.Params["ToName"].Get<string>();
            e.Params[fromName].Name = toName;
        }

        /**
         * Level2: Will strip every single Parameter OUT of the Node structure except the given 'But'. But can be
         * either one single name of an object or a list of nodes containing several names. Useful for
         * shrinking nodes as the grow due to being passed around or being parts of MultiActions or something
         * similar
         */
        [ActiveEvent(Name = "Magix.Common.StripAllParametersExcept")]
        protected void Magix_Common_StripAllParametersExcept(object sender, ActiveEventArgs e)
        {
            string but = e.Params["But"].Get<string>();
            if (!string.IsNullOrEmpty(but))
            {
                // Keeping only ONE node ...
                List<Node> nodes = new List<Node>();
                foreach (Node ix in e.Params)
                {
                    if (ix.Name != but)
                        nodes.Add(ix);
                }
                foreach (Node idx in nodes)
                {
                    idx.UnTie();
                }
            }
            else
            {
                // Array of stuff to keep ...
                List<Node> nodes = new List<Node>();
                foreach (Node ix in e.Params["But"])
                {
                    if (ix.Name != but)
                        nodes.Add(ix);
                }
                foreach (Node idx in nodes)
                {
                    idx.UnTie();
                }
            }
        }

        /**
         * Level2: Will return the given MetaObject [MetaObjectID] as a Key/Value pair. Will not traverse
         * Child Objects though. Useful for fetching objects for any one reasons you might have,
         * as long as you know their ID
         */
        [ActiveEvent(Name = "Magix.Common.GetSingleMetaObject")]
        protected void Magix_Common_GetSingleMetaObject(object sender, ActiveEventArgs e)
        {
            MetaObject o = MetaObject.SelectByID(e.Params["MetaObjectID"].Get<int>());

            if (o == null)
                throw new ArgumentException(
                    @"Some wize-guy have deleted your object dude. 
Update the MetaObjectID property of your Action to another Meta Object ...");

            foreach (MetaObject.Property idx in o.Values)
            {
                e.Params[idx.Name].Value = idx.Value;
            }
        }

        /**
         * Level2: Will reload the Original WebPart, intended to be, within the 'current WebPart container'
         * on the page. Meaning, if you've allowed the user to 'fuzz around all over the place' till he
         * no longer can remember what originally was within a specific WebPart Container, he can 
         * raise this event [somehow], which will 'reload the original content' into the 'current
         * container' [container raising the event]
         */
        [ActiveEvent(Name = "Magix.Common.ReloadOriginalWebPart")]
        protected void Magix_Common_ReloadOriginalWebPart(object sender, ActiveEventArgs e)
        {
            int po = e.Params["Parameters"]["OriginalWebPartID"].Get<int>();

            Node node = new Node();

            node["OriginalWebPartID"].Value = po;

            RaiseEvent(
                "Magix.Publishing.ReloadWebPart",
                node);
        }

        /**
         * Level2: If raised from within a MetaView on a specific MetaObject ['MetaObjectID'], 
         * somehow, will show the Signature Module for that particular MetaObject for
         * its 'ActionSenderName' property. When Signature is done [signing complete]
         * the original content of the Container will be reloaded
         */
        [ActiveEvent(Name = "Magix.Common.LoadSignatureForCurrentMetaObject")]
        protected void Magix_Common_LoadSignatureForCurrentMetaObject(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("OriginalWebPartID") ||
                !e.Params.Contains("MetaObjectID"))
            {
                throw new ArgumentException("Sorry buddy, but this Action only works from a MetaView WebPart ...");
            }
            else
            {
                e.Params["OKEvent"].Value = "Magix.MetaView.UnLoadSignature";
                e.Params["OKEvent"]["Params"]["MetaObjectID"].Value = e.Params["MetaObjectID"].Value;
                e.Params["OKEvent"]["Params"]["OriginalWebPartID"].Value = e.Params["OriginalWebPartID"].Value;
                e.Params["OKEvent"]["Params"]["Name"].Value = e.Params["ActionSenderName"].Value;

                e.Params["CancelEvent"].Value = "Magix.Publishing.ReloadWebPart";
                e.Params["CancelEvent"]["Params"]["OriginalWebPartID"].Value = e.Params["OriginalWebPartID"].Value;

                if (e.Params.Contains("Value") &&
                    !string.IsNullOrEmpty(e.Params["Value"].Get<string>()))
                    e.Params["Coords"].Value = e.Params["Value"].Value;

                RaiseEvent(
                    "Magix.Signature.LoadSignature",
                    e.Params);
            }
        }

        /**
         * Level2: Helper for Signature Module, to store it correctly upon finishing and saving a new Signature.
         * Will extract the 'Signature' content and store into the 'Name' property of the given
         * 'MetaObjectID' MetaObject and save the MetaObject
         */
        [ActiveEvent(Name = "Magix.MetaView.UnLoadSignature")]
        protected void Magix_Signature_UnLoadSignature(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject o = MetaObject.SelectByID(e.Params["MetaObjectID"].Get<int>());

                MetaObject.Property val = o.Values.Find(
                    delegate(MetaObject.Property idx)
                    {
                        return idx.Name == e.Params["Name"].Get<string>();
                    });
                if (val == null)
                {
                    val = new MetaObject.Property();
                    val.Name = e.Params["Name"].Get<string>();
                    o.Values.Add(val);

                    o.Save();
                }

                val.Value = e.Params["Signature"].Get<string>();

                val.Save();

                tr.Commit();
            }

            RaiseEvent(
                "Magix.Publishing.ReloadWebPart",
                e.Params);
        }

        /**
         * Level2: Will set the given Session Variable ['Name'] to the 'Value'. Useful for creating caches
         * of huge things, you need to occur really fast [or something]. Session Variables like
         * these can later be retrieved by its sibling method 'Magix.Common.GetSessionVariable'.
         * Things stored into the Session will be on a per user level [meaning, it'll take a LOT of 
         * memory on your server], but it will be very fast to retrieve later. Be Cautious here!
         */
        [ActiveEvent(Name = "Magix.Common.SetSessionVariable")]
        protected void Magix_Common_SetSessionVariable(object sender, ActiveEventArgs e)
        {
            Page.Session[e.Params["Name"].Get<string>()] = e.Params["Value"].Value;
        }

        /**
         * Level2: Will return the given Session Variable ['Name'] to the 'Value' output node. Useful for retrieving caches
         * of huge things, you need to occur really fast [or something]. Session Variables like
         * these can be set by its sibling method 'Magix.Common.SetSessionVariable'.
         * Things stored into the Session will be on a per user level [meaning, it'll take a LOT of 
         * memory on your server], but it will be very fast to retrieve later. Be Cautious here!
         */
        [ActiveEvent(Name = "Magix.Common.GetSessionVariable")]
        protected void Magix_Common_GetSessionVariable(object sender, ActiveEventArgs e)
        {
            e.Params["Value"].Value = Page.Session[e.Params["Name"].Get<string>()];
        }

        /**
         * Level2: Expects to be given a 'MetaViewName' which it will turn into a MetaView
         * object, which it will use as its foundation for exporting all MetaObjects of the 
         * MetaView's TypeName into a CSV file, which it will redirect the client's web 
         * browser towards. You can also override how the type is being rendered by 
         * adding up 'WhiteListColumns' and 'Type' parameters, which will override 
         * the default behavior for the MetaView. Set 'Redirect' to false if you wish
         * to stop redirecting to the newly created file to occur. Regardless the relative 
         * path to the file created will be returned as 'FileName' and the number of 
         * records as 'NoRecords'
         */
        [ActiveEvent(Name = "Magix.Common.ExportMetaView2CSV")]
        protected void Magix_Common_ExportMetaView2CSV(object sender, ActiveEventArgs e)
        {
            DateTime begin = DateTime.Now;

            Node n = new Node();

            n["FullTypeName"].Value = typeof(MetaObject).FullName + "-META";
            n["MetaViewName"].Value = e.Params["MetaViewName"].Value;

            // Signalizing 'everything' to the GetContentsOfClass event handler ...
            n["Start"].Value = 0;
            n["End"].Value = -1;

            // Making sure everything is sorted according to Newest FIRST ...!
            // TODO: There are tons of OTHER Grids in this system, e.g. SearchActions grid, 
            // which are NOT sorted correctly. Fix this at some point ... !
            n["Criteria"]["C1"]["Name"].Value = "Sort";
            n["Criteria"]["C1"]["Value"].Value = "Created";
            n["Criteria"]["C1"]["Ascending"].Value = false;

            // TODO: Support 'Massively Large DataSets' by allowing for 'CallBackEvent' to be supplied
            // ... which then will call CallBackEventHandler every n'th iteration or something ...
            RaiseEvent(
                "DBAdmin.Data.GetContentsOfClass",
                n);

            n["FileName"].Value = 
                "Tmp/" + 
                e.Params["MetaViewName"].Get<string>() + 
                "-" + 
                DateTime.Now.ToString("yyyy-MM-mm-HH-mm-ss", CultureInfo.InvariantCulture) + ".csv";

            if (e.Params.Contains("Redirect"))
                n["Redirect"].Value = e.Params["Redirect"].Value;

            RaiseEvent(
                "Magix.Common.ExportMetaViewObjectList2CSV",
                n);

            e.Params["FileName"].Value = n["FileName"].Value;
            e.Params["NoRecords"].Value = n["Objects"].Count;

            TimeSpan timer = DateTime.Now - begin;

            Node log = new Node();

            log["LogItemType"].Value = "Magix.Common.ExportMetaView2CSV";
            log["Header"].Value = "CSV File was Created";

            log["Message"].Value = string.Format(@"
CSV File '{0}' was created at {1} from MetaView '{5}' of MetaType '{2}'. 
The file has {3} records in it. Time to create file was {4} milliseconds",
                e.Params["FileName"].Value,
                DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"),
                e.Params["MetaViewTypeName"].Value,
                n["Objects"].Count,
                timer.TotalMilliseconds,
                e.Params["MetaViewName"].Get<string>());

            if (User.Current != null)
                log["ObjectID"].Value = User.Current.ID;

            RaiseEvent(
                "Magix.Core.Log",
                log);
        }

        /**
         * Level2: Will export a node list in 'Objects' List form to a CSV file, 
         * and redirect the client to that newly created CSV file. Set 'Redirect' to 
         * false to stop redirecting to the newly created CSV file to occur
         */
        [ActiveEvent(Name = "Magix.Common.ExportMetaViewObjectList2CSV")]
        protected void Magix_Common_ExportMetaViewObjectList2CSV(object sender, ActiveEventArgs e)
        {
            MetaView v = MetaView.SelectFirst(
                Criteria.Eq(
                    "Name", 
                    e.Params["MetaViewName"].Get<string>()));

            List<string> cols = new List<string>();
            foreach (MetaView.MetaViewProperty idx in v.Properties)
            {
                if (idx.Name.StartsWith(":"))
                    continue; // e.g. ":Save" or ":Delete" - System columns. Do NOT render to csv ...
                if (idx.Name.Contains(":"))
                {
                    // 'Complex prperty', e.g. "select:Gender.Sex:Sex".
                    // ALWAYS renders the 'Column Name' at the END ...!
                    cols.Add(idx.Name.Substring(idx.Name.LastIndexOf(":") + 1));
                }
                else
                {
                    cols.Add(idx.Name);
                }
            }

            using (TextWriter text = File.CreateText(Page.MapPath("~/" + e.Params["FileName"].Get<string>())))
            {
                // Rendering headers ...
                text.Write("ID, Created");
                foreach (string idx in cols)
                {
                    text.Write(",");
                    text.Write(idx);
                }
                text.WriteLine();

                // Rendering objects ...
                foreach (Node idx in e.Params["Objects"])
                {
                    text.Write(idx["ID"].Value.ToString() + ",");
                    text.Write(idx["Created"].Value.ToString());
                    foreach (string idxCol in cols)
                    {
                        text.Write(",");
                        if (idx["Properties"].Contains(idxCol))
                        {
                            string content = idx["Properties"][idxCol].Get<string>() ?? "";
                            content = content
                                .Replace("\\", "\\\\")
                                .Replace("\"", "'");
                            text.Write("\"" + content + "\"");
                        }
                    }
                    text.WriteLine();
                }
            }

            // Checking to see if we're supposed to redirect client browser
            if (!e.Params.Contains("Redirect") ||
                e.Params["Redirect"].Get<bool>())
            {
                Node xx = new Node();
                xx["URL"].Value = e.Params["FileName"].Get<string>();

                RaiseEvent(
                    "Magix.Common.RedirectClient",
                    xx);
            }
        }

        /**
         * Level2: Redirect clients to the given 'URL' parameter
         */
        [ActiveEvent(Name = "Magix.Common.RedirectClient")]
        protected void Magix_Common_RedirectClient(object sender, ActiveEventArgs e)
        {
            string path = e.Params["URL"].Get<string>();

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Tried to redirect to a 'null' URL ...??");

            if (!path.StartsWith("http"))
                path = "~/" + path;

            AjaxManager.Instance.Redirect(path);
        }

        /**
         * Level2: Given a 'FileName' a 'Folder' and a 'MetaViewName' this method will 
         * try to import the given CSV file according to the given MetaView ['MetaViewName']
         * into your MetaObject data storage
         */
        [ActiveEvent(Name = "Magix.Common.ImportCSVFile")]
        protected void Magix_Common_ImportCSVFile(object sender, ActiveEventArgs e)
        {
            MetaView m = 
                MetaView.SelectFirst(
                    Criteria.Eq(
                        "Name", 
                        e.Params["MetaViewName"].Get<string>()));

            if (m == null)
                throw new ArgumentException(
                    @"Sorry, but you need to submit a 'MetaViewName' to an 
existing MetaView to import CSV files");

            if (string.IsNullOrEmpty(m.TypeName))
                throw new ArgumentException("Sorry, but your MetaView doesn't have a TypeName, which means we don't know which types to create from your CSV file");

            if (m.Properties.Count == 0)
                throw new ArgumentException(
                    @"Sorry, but your MetaView doesn't contain any properties, 
hence nothing will become imported, and this function call is useless. 
Add up properties that corresponds to the columns in your CSV file if you wish to import it");

            string folder = (e.Params["Folder"].Get<string>() ?? "").Trim().Trim('/');
            if (!string.IsNullOrEmpty(folder))
                folder += "/";

            string fileName = Page.MapPath(
                "~/" +
                folder +
                e.Params["FileName"].Get<string>());

            int count = ImportCSVFileFromMetaView(fileName, m);

            if (!e.Params.Contains("NoMessage") || 
                !e.Params["NoMessage"].Get<bool>())
            {
                Node node = new Node();

                node["Message"].Value = string.Format(@"
You've successfully imported {0} items of type '{1}' from the file '{2}' using the MetaView '{3}'",
                    count,
                    m.TypeName,
                    fileName,
                    m.Name);

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
            }

            Node l = new Node();
            l["LogItemType"].Value = "Magix.Common.ImportCSVFile";
            l["Header"].Value = "File '" + fileName + "' was imported";
            l["Message"].Value = string.Format(@"
File '{0}' was imported, creating {1} items of type '{2}' from MetaView '{3}' on page '{4}'",
                fileName,
                count,
                m.TypeName,
                m.Name,
                Page.Request.Url.ToString());

            RaiseEvent(
                "Magix.Core.Log",
                l);
        }

        private int ImportCSVFileFromMetaView(string fileName, MetaView m)
        {
            int retVal = 0;
            List<string> viewCols = GetViewCols(m);
            using (TextReader reader = File.OpenText(fileName))
            {
                List<string> fileCols = GetFileCols(reader);

                using (Transaction tr = Adapter.Instance.BeginTransaction())
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                            break;

                        List<string> values = GetFileValues(line);

                        CreateMetaObject(viewCols, fileCols, values, m.TypeName, fileName);
                        retVal += 1;
                    }
                    tr.Commit();
                }
            }
            return retVal;
        }

        /*
         * Creates, and saves, one meta object mapping the values into the viewCols from
         * indexing the values through the fileCols
         */
        private void CreateMetaObject(
            List<string> viewCols, 
            List<string> fileCols, 
            List<string> values, 
            string typeName,
            string fileName)
        {
            MetaObject o = new MetaObject();
            o.TypeName = typeName;
            string fileName2 = fileName;
            if (fileName2.IndexOf('\\') != -1)
                fileName2 = fileName2.Substring(fileName2.LastIndexOf('\\') + 1);
            o.Reference = "Import: " + fileName2;

            int idxNo = 0;
            foreach (string idxViewColumnName in viewCols)
            {
                MetaObject.Property p = new MetaObject.Property();
                p.Name = idxViewColumnName;

                int indexOfViewInFileCols = fileCols.IndexOf(idxViewColumnName);

                if (indexOfViewInFileCols > -1 && 
                    indexOfViewInFileCols < values.Count) // In case line in file is 'chopped' ...
                {
                    p.Value = values[indexOfViewInFileCols];
                }
                o.Values.Add(p);
                idxNo += 1;
            }

            o.Save();
        }

        /*
         * Given a string, which is one line from a CSV file, breaks it down into a
         * list of values it returns back to caller ...
         */
        private List<string> GetFileValues(string line)
        {
            line = line.Trim();
            List<string> values = new List<string>();
            bool isInside = true; // We start OUT by being inside ...
            bool hasFnutt = false;
            string buffer = "";
            int startNo = 0;
            if (line.Length > 0 && line[0] == '"')
            {
                startNo += 1;
                hasFnutt = true;
            }
            for (int idxNo = startNo; idxNo < line.Length; idxNo++)
            {
                if (isInside)
                {
                    // Inside of a "
                    char idxC = line[idxNo];
                    if (idxC == '\\')
                    {
                        idxNo += 1; // skipping this one, reading next

                        if (idxNo >= line.Length)
                            break; // Ops. At end ...! Buffer being added further down ...

                        idxC = line[idxNo];
                        buffer += idxC;
                    }
                    else if (hasFnutt && idxC == '"')
                    {
                        // Ending entity ...
                        hasFnutt = false;
                        values.Add(buffer);
                        buffer = "";
                        isInside = false;
                    }
                    else if (!hasFnutt && idxC == ',')
                    {
                        // Ending entity ...
                        values.Add(buffer);
                        buffer = "";
                        isInside = false;
                        idxNo -= 1; // Need this ...!
                    }
                    else
                    {
                        buffer += idxC;
                    }
                }
                else
                {
                    // OUTSIDE of the " [or ,]
                    char idxC = line[idxNo];
                    if (idxC == ',')
                    {
                        // Going inside again ...
                        // Need to discard the first '"' if existing ...
                        if (line.Length > idxNo + 1)
                        {
                            if (line[idxNo + 1] == '"')
                            {
                                hasFnutt = true;
                                idxNo += 1; // Skipping the " parts ...
                            }
                        }
                        isInside = true;
                    }
                }
            }
            values.Add(buffer); // LAST value ...
            return values;
        }

        /*
         * Helper for above, returns a List containing all 'columns' in the file. Expects
         * to be positioned at the beginning of the CSV file before being called ...
         */
        private List<string> GetFileCols(TextReader reader)
        {
            List<string> cols = new List<string>();

            string firstLine = reader.ReadLine();

            foreach (string idx in firstLine.Split(','))
            {
                string col = idx.Trim().Trim('"');
                cols.Add(col);
            }

            return cols;
        }

        /*
         * Helper for above. Returns a List of names from properties from the MetaView
         */
        private List<string> GetViewCols(MetaView m)
        {
            List<string> viewCols = new List<string>();
            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                string name = idx.Name;
                if (name.IndexOf(":") == 0)
                    continue;

                if (name.Contains(":"))
                {
                    name = name.Substring(name.LastIndexOf(":"));
                }
                viewCols.Add(name);
            }
            return viewCols;
        }
    }
}
