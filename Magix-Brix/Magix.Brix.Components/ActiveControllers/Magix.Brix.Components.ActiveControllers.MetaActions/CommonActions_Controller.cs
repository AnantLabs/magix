﻿/*
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
CSV File '{0}' was created at {1} from MetaType '{2}' and MetaView '{5}' with {3} records in. Time to create 
file was {4} milliseconds",
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
    }
}





















