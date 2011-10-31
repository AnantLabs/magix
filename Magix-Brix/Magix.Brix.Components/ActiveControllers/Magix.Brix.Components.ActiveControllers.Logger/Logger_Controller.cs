/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Components.ActiveTypes.Logging;

namespace Magix.Brix.Components.ActiveControllers.Logger
{
    /**
     * Level2: Contains logic for Logging things
     */
    [ActiveController]
    public class Logger_Controller : ActiveController
    {
        /**
         * Level2: Logs every email sent out of Magix
         */
        [ActiveEvent(Name = "Magix.Core.SendEmail")]
        protected void Magix_Core_SendEmail(object sender, ActiveEventArgs e)
        {
            UserBase u = UserBase.Current;
            Node node = new Node();
            node["LogItemType"].Value = "Magix.Core.SendEmail";

            node["Header"].Value = e.Params["Header"].Get<string>();

            string to = "";

            foreach (Node idxEmail in e.Params["EmailAddresses"])
            {
                to += idxEmail.Value as string + ", ";
            }

            node["Message"].Value = 
                "Sent from: " +
                e.Params["AdminEmail"].Value +
                ", Sent to: " + to +
                ", Body: " + e.Params["Body"].Get<string>();

            if (u != null)
                node["ObjectID"].Value = u.ID;

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Handled here since it's one of the more 'common operations' and probably interesting
         * to see in retrospect in case something goes wrong
         */
        [ActiveEvent(Name = "Magix.Core.UserLoggedIn")]
        private void Magix_Core_UserLoggedIn(object sender, ActiveEventArgs e)
        {
            UserBase u = UserBase.Current;
            Node node = new Node();
            node["LogItemType"].Value = "Magix.Core.UserLoggedIn";
            node["Header"].Value = u.Username + " - " + u.RolesString;

            node["Message"].Value = "Sender: " + sender.ToString() + ".";
            node["ObjectID"].Value = u.ID;

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Handled here since it's one of the more 'common operations' and probably interesting
         * to see in retrospect in case something goes wrong
         */
        [ActiveEvent(Name = "Magix.Core.ShowMessage")]
        protected void Magix_Core_ShowMessage(object sender, ActiveEventArgs e)
        {
            // Only logging ERRORS ... !
            if (!e.Params.Contains("IsError") ||
                !e.Params["IsError"].Get<bool>())
                return;

            Node node = new Node();
            node["LogItemType"].Value = "Magix.Core.ShowMessage";
            if (e.Params.Contains("Username"))
            {
                UserBase u =
                    UserBase.SelectFirst(
                        Criteria.Eq("Username", e.Params["Username"].Get<string>()));
                node["ObjectID"].Value = u.ID;
                node["Header"].Value = u.Username + " - " + u.RolesString;
            }
            else
            {
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                    node["Header"].Value = UserBase.Current.Username + " - " + UserBase.Current.RolesString;
                }
                else
                {
                    node["ObjectID"].Value = -1;
                    node["Header"].Value = "anonymous";
                }
            }

            if (e.Params.Contains("IsError"))
                node["Message"].Value = "ERROR! " +
                    e.Params["Message"].Get<string>();
            else
                node["Message"].Value = e.Params["Message"].Get<string>();

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        // TODO: Implement 'warning email to admin' if LogItemType ends with '!' ...
        /**
         * Level2: Will create one LogItem with the given LogItemType, Header, Message, 
         * ObjectID, ParentID, StackTrace and so on, depending upon which data is actually 
         * being passed into it. Minimum requirement is Header. If you append an 
         * Exclamation mark as the last character of the LogItemType, the incident will be 
         * considered _serious_, and some sort of reaching out to the admin of the site 
         * might occur as a consequence, depending upon other states of the system. The
         * Log action might often be overridden, if you need to force into this specific method
         * for some reasons, you can add '-HARDLINK' to the action as you raise it
         */
        [ActiveEvent(Name = "Magix.Core.Log")]
        [ActiveEvent(Name = "Magix.Core.Log-HARDLINK")]
        protected void Magix_Core_Log(object sender, ActiveEventArgs e)
        {
            DateTime when = DateTime.Now;
            string logItemType = e.Params["LogItemType"].Get<string>("Generic-Type");
            string header = e.Params["Header"].Get<string>();
            string msg = e.Params["Message"].Get<string>() ?? "";

            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentException("Header as a minimum needs to be set when Logging ...");
            }

            int objectID = e.Params["ObjectID"].Get<int>(-1);
            int parentID = e.Params["ParentID"].Get<int>(-1);
            string stackTrace = e.Params["StackTrace"].Get<string>();
            string ip = (HttpContext.Current.Handler as Page).Request.UserHostAddress;
            UserBase user = UserBase.Current;

            LogItem l = new LogItem();
            l.When = when;
            l.LogItemType = logItemType;
            l.Header = header;
            l.Message = msg;
            l.ObjectID = objectID;
            l.ParentID = parentID;
            l.StackTrace = stackTrace;
            l.IPAddress = ip;
            l.User = user;

            HttpCookie cookie = Page.Request.Cookies["UserID"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                l.UserID = cookie.Value;
            }
            else
            {
                cookie = Page.Response.Cookies["UserID"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    l.UserID = cookie.Value;
                }
            }
            l.Save();
        }

        /**
         * Level2: Handled since it's important information since it's highly likely it's a new visitor 
         * to the app/website
         */
        [ActiveEvent(Name = "Magix.Core.NewUserIDCookieCreated")]
        protected void Magix_Core_NewUserIDCookieCreated(object sender, ActiveEventArgs e)
        {
            // TODO: @#$#@ man! Re-FACTOR ...!!
            string userID = e.Params["UserID"].Get<string>();

            // Running through 10 latest log requests with same IP address to determine
            // probability of log item comes from previous UserID with cleared cookies or such...
            bool noneExist = true;
            int no = 0;
            string firstCookie = null;
            bool allSame = true;
            bool lastSame = true;
            bool oneSame = false;
            string lastUserId = "";
            Dictionary<string, bool> keys = new Dictionary<string, bool>();
            foreach (LogItem idx in LogItem.Select(
                Criteria.Eq("IPAddress", Page.Request.UserHostAddress),
                Criteria.Range(0, 10, "When", false)))
            {
                if (firstCookie == null)
                {
                    firstCookie = idx.UserID ?? "";
                }
                else
                {
                    if (firstCookie != idx.UserID)
                        allSame = false;
                    else
                        oneSame = true;
                }
                if (!string.IsNullOrEmpty(idx.UserID))
                {
                    lastUserId = idx.UserID;
                    keys[idx.UserID] = true;
                }
                noneExist = false;
                if (no >= 4)
                {
                    // Halfway there ...
                    if (firstCookie != idx.UserID)
                        lastSame = false;
                }
                no += 1;
            }

            // Almost certainly a new visit, though not entirely certain either ...
            // Still logged as a first time visit ...
            Node n = new Node();

            n["LogItemType"].Value = "Magix.Core.NewTrackingCookieCreated";
            n["Header"].Value = "System-Message";

            n["Message"].Value = "UserAgent: " + Page.Request.UserAgent + " - Sender: " + sender.ToString() + ".";

            if (UserBase.Current != null)
            {
                n["ObjectID"].Value = UserBase.Current.ID;
            }

            RaiseEvent(
                "Magix.Core.Log",
                n);

            if (noneExist)
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit ...
                Node node2 = new Node();
                node2["LogItemType"].Value = "Magix.Core.FirstVisitFromIP";
                node2["Header"].Value = "System-Message";

                if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.ToString().Length > 0)
                    node2["Message"].Value = "Referral: " + Page.Request.UrlReferrer.ToString();
                else
                    node2["Message"].Value = "Referral: Empty Host name...";

                if (UserBase.Current != null)
                {
                    node2["ObjectID"].Value = UserBase.Current.ID;
                }

                RaiseEvent(
                    "Magix.Core.Log",
                    node2);
            }

            Node node = new Node();
            if (noneExist || (!oneSame && no > 5))
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit ...
                node["LogItemType"].Value = "Magix.Core.FirstVisit";
                node["Header"].Value = "Certainty: 0.95";

                node["Message"].Value = "UserAgent: " + Page.Request.UserAgent + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (no > 5 && keys.Count >= 3)
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit with 90% certainty ...
                node["LogItemType"].Value = "Magix.Core.FirstVisit";
                node["Header"].Value = "Certainty: 0.9";
                if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.ToString().Length > 0)
                    node["Message"].Value = "Referral: " + Page.Request.UrlReferrer.ToString();
                else
                    node["Message"].Value = "Referral: Empty Host name...";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (allSame && no >= 10)
            {
                // Highly probably the same visit
                // Logged as a 90% certainty 'Linked-Visit' ...
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.9";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (allSame && no >= 5)
            {
                // Probably the same visit
                // Logged as a 80% certainty 'Linked-Visit' ...
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.8";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (lastSame && no >= 10)
            {
                // Likely the same visit
                // Logged as a 70% certainty 'Linked-Visit' ...
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.7";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (lastSame && no >= 8)
            {
                // Probably the same visit
                // Logged as a 50% certainty 'Linked-Visit' ...
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.5";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (no > 2 && no / keys.Count >= 2)
            {
                // Probably the same visit
                // Logged as a 50% certainty 'Linked-Visit' ...
                // Though no Old-UserID is given ...!
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.5";

                node["Message"].Value = "Old-UserID: " + lastUserId + "? - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else if (no > 2 && no / keys.Count < 2)
            {
                // Probably the same visit
                // Logged as a 40% certainty 'Linked-Visit' ...
                // Though no Old-UserID is given ...!
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.4";

                node["Message"].Value = "Old-UserID: " + lastUserId + "? - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }
            else
            {
                // Probably new visit
                // Logged as a 90% certainty 'New-Visit' ...
                node["LogItemType"].Value = "Magix.Core.NewVisit";
                node["Header"].Value = "Certainty: 0.9";

                node["Message"].Value = "UserAgent: " + Page.Request.UserAgent + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
            }

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Logs the creation of every QR Code [and change to them]
         */
        [ActiveEvent(Name = "Magix.QRCodes.CreateQRCode")]
        protected void Magix_QRCodes_CreateQRCode(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["LogItemType"].Value = "Magix.QRCodes.CodeCreated";
            if (UserBase.Current != null)
            {
                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "QR Code was created by " + UserBase.Current.Username;
            }
            else
                node["Header"].Value = "Anonymous Coward just created a QR Code";

            node["Message"].Value = "File: " + e.Params["FileName"].Value;

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Handled to log everytime somebody creates a WebPage
         */
        [ActiveEvent(Name = "Magix.Publishing.CreateChildWebPage")]
        protected void Magix_Publishing_CreateChildWebPage(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["LogItemType"].Value = "Magix.Publishing.PageCreated";

            if (UserBase.Current != null)
            {
                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "Web Page was created by " + UserBase.Current.Username;
            }
            else
                node["Header"].Value = "Anonymous Coward just created a Web Page";

            node["Message"].Value = "ParentID: " + e.Params["ID"].Value;

            node["ParentID"].Value = e.Params["ID"].Value; 

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Logs every time a Web Page is being deleted
         */
        [ActiveEvent(Name = "Magix.Publishing.DeletePageObject-Confirmed")]
        protected void Magix_Publishing_DeletePageObject_Confirmed(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (UserBase.Current != null)
            {
                node["LogItemType"].Value = "Magix.Publishing.PageDeleted";
                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "Web Page was deleted by " + UserBase.Current.Username;
            }
            else
            {
                // Exclamation mark carries 'semantics' ...
                node["LogItemType"].Value = "Magix.Publishing.PageDeleted!";
                node["Header"].Value = "WARNING! WARNING! WARNING! __Anonymous-Coward__ just deleted a Web Page";
            }

            node["Message"].Value = "WebPage ID: " + e.Params["ID"].Value;

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Generic handler for handling some of the more commond Active Types deletion scenarios
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            switch (e.Params["FullTypeName"].Get<string>())
            {
                case "Magix.Brix.Components.ActiveTypes.Publishing.WebPageTemplate":
                    {
                        Node node = new Node();

                        // A Template deletion is _always_ a serious incident ... ;)
                        node["LogItemType"].Value = "Magix.Publishing.TemplateDeleted!";

                        if (UserBase.Current != null)
                        {
                            node["ObjectID"].Value = UserBase.Current.ID;
                            node["Header"].Value = "Web Page was deleted by " + UserBase.Current.Username;
                        }
                        else
                            node["Header"].Value = "WARNING! WARNING! WARNING! __Anonymous-Coward__ just deleted a Web Page";

                        node["Message"].Value = "WebPage ID: " + e.Params["ID"].Value;

                        RaiseEvent(
                            "Magix.Core.Log",
                            node);
                    } break;
                case "Magix.Brix.Components.ActiveTypes.Users.Role":
                    {
                        Node node = new Node();

                        if (UserBase.Current != null)
                        {
                            node["ObjectID"].Value = UserBase.Current.ID;
                            node["LogItemType"].Value = "Magix.Publishing.RoleDeleted";
                            node["Header"].Value = "Web Page was deleted by " + UserBase.Current.Username;
                        }
                        else
                        {
                            node["LogItemType"].Value = "Magix.Publishing.RoleDeleted!!!";
                            node["Header"].Value = "WARNING! WARNING! WARNING! __Anonymous-Coward__ just tried to delete a Role ...";
                        }

                        node["Message"].Value = "Role deleted";

                        RaiseEvent(
                            "Magix.Core.Log",
                            node);
                    } break;
            }
        }

        /**
         * Level2: Every time a user is being warned, he will be shown a Message Box. We log _ALL_ those 
         * message box warnings here, to document what the user has seen of warnings. We might
         * also log other types of modules in this event handler
         */
        [ActiveEvent(Name = "Magix.Core.LoadActiveModule")]
        protected void Magix_Core_LoadActiveModule(object sender, ActiveEventArgs e)
        {
            if (e.Params["Name"].Get<string>() == 
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox")
            {
                Node node = new Node();

                if (UserBase.Current != null)
                {
                    node["LogItemType"].Value = "Magix.Core.UserWasWarned";
                    node["ObjectID"].Value = UserBase.Current.ID;
                    node["Header"].Value = "Warning granted to user " + UserBase.Current.Username;
                }
                else
                {
                    // Warnings are mostly about objects being deleted and such, hence they do pose
                    // a danger, and should be treated as big warnings ...!!
                    node["LogItemType"].Value = "Magix.Core.UserWasWarned!";
                    node["Header"].Value = "Anonymous-Coward was warned";
                }

                node["Message"].Value = "Text: " + e.Params["Parameters"]["Text"].Value;

                RaiseEvent(
                    "Magix.Core.Log",
                    node);
            }
        }

        /**
         * Level2: Logs the creation of Users
         */
        [ActiveEvent(Name = "Magix.Publishing.CreateUser")]
        protected void Magix_Publishing_CreateUser(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (UserBase.Current != null)
            {
                node["LogItemType"].Value = "Magix.Core.UserCreated";
                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "User created by user " + UserBase.Current.Username;
            }
            else
            {
                // If an anonymous user tries to somehow create a user, then that's a MAJOR
                // issue ... !!!!!!!!!!!
                // TRIPPEL warnings ...!!
                node["LogItemType"].Value = "Magix.Core.UserCreated!!!";
                node["Header"].Value = "WARNING! WARNING! WARNING! Anonymous-Coward tried to __CREATE-A-USER__ !!!";
            }

            node["Message"].Value = "New user was created ...";

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Logs every time somebody tries to create a PDF book using Magix
         */
        [ActiveEvent(Name = "Magix.PDF.CreatePDF-Book")]
        protected void Magix_PDF_CreatePDF_Book(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["LogItemType"].Value = "Magix.PDF.CreatePDF";

            if (UserBase.Current != null)
            {
                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "PDF created by user " + UserBase.Current.Username;
            }
            else
            {
                node["Header"].Value = "Anonymous-Coward created a PDF";
            }

            node["Message"].Value = "File: " + e.Params["File"].Get<string>();

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Logs the opening of the DB Manager
         */
        [ActiveEvent(Name = "DBAdmin.Form.ViewClasses")]
        protected void DBAdmin_Form_ViewClasses(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (UserBase.Current != null)
            {
                node["LogItemType"].Value = "Magix.DBAdmin.ViewClasses";

                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "Classes viewed by " + UserBase.Current.Username;
            }
            else
            {
                // OMG ...!!!!!
                node["LogItemType"].Value = "Magix.DBAdmin.ViewClasses!!!";

                node["Header"].Value = "Anonymous-Coward views classes";
            }

            node["Message"].Value = "Class browser for Active Types started";

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Logs the opening of the File Manager
         */
        [ActiveEvent(Name = "Magix.FileExplorer.LaunchExplorer")]
        protected void Magix_FileExplorer_LaunchExplorer(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (UserBase.Current != null)
            {
                node["LogItemType"].Value = "Magix.FileManager.Opened";

                node["ObjectID"].Value = UserBase.Current.ID;
                node["Header"].Value = "File System viewed by " + UserBase.Current.Username;
            }
            else
            {
                // _PROBABLY_ a bad thing ...
                node["LogItemType"].Value = "Magix.FileManager.Opened!";

                if (e.Params["RootAccessFolder"].Get<string>() == "/")
                {
                    // OMG ...!!
                    node["LogItemType"].Value = "Magix.FileManager.Opened!!!";
                }

                node["Header"].Value = "Anonymous-Coward views file system " + e.Params["RootAccessFolder"].Value;
            }

            node["Message"].Value = "Class browser for Active Types started";

            RaiseEvent(
                "Magix.Core.Log",
                node);
        }
    }
}
