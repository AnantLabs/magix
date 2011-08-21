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

            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "Magix.Core.Log",
                node);
        }

        /**
         * Level2: Will create one LogItem with the given LogItemType, Header, Message, 
         * ObjectID, ParentID, StackTrace and so on, depending upon which data is actually 
         * being passed into it. Minimu requirement is 'Header'
         */
        [ActiveEvent(Name = "Magix.Core.Log")]
        protected void Magix_Core_Log(object sender, ActiveEventArgs e)
        {
            DateTime when = DateTime.Now;
            string logItemType = e.Params["LogItemType"].Get<string>("Generic-Type");
            string header = e.Params["Header"].Get<string>();
            string msg = e.Params["Message"].Get<string>();

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
            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "Magix.Core.Log",
                n);

            if (noneExist)
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.FirstVisitFromIP";
                node["Header"].Value = "System-Message";

                if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.ToString().Length > 0)
                    node["Message"].Value = "Referral: " + Page.Request.UrlReferrer.ToString();
                else
                    node["Message"].Value = "Referral: Empty Host name...";

                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            if (noneExist || (!oneSame && no > 5))
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.FirstVisit";
                node["Header"].Value = "Certainty: 0.95";

                node["Message"].Value = "UserAgent: " + Page.Request.UserAgent + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (no > 5 && keys.Count >= 3)
            {
                // Almost certainly a new visit, though not entirely certain either ...
                // Still logged as a first time visit with 90% certainty ...
                Node node = new Node();
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
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (allSame && no >= 10)
            {
                // Highly probably the same visit
                // Logged as a 90% certainty 'Linked-Visit' ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.9";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (allSame && no >= 5)
            {
                // Probably the same visit
                // Logged as a 80% certainty 'Linked-Visit' ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.8";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (lastSame && no >= 10)
            {
                // Likely the same visit
                // Logged as a 70% certainty 'Linked-Visit' ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.7";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (lastSame && no >= 8)
            {
                // Probably the same visit
                // Logged as a 50% certainty 'Linked-Visit' ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.5";

                node["Message"].Value = "Old-UserID: " + firstCookie + " - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (no > 2 && no / keys.Count >= 2)
            {
                // Probably the same visit
                // Logged as a 50% certainty 'Linked-Visit' ...
                // Though no Old-UserID is given ...!
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.5";

                node["Message"].Value = "Old-UserID: " + lastUserId + "? - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else if (no > 2 && no / keys.Count < 2)
            {
                // Probably the same visit
                // Logged as a 40% certainty 'Linked-Visit' ...
                // Though no Old-UserID is given ...!
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.LinkedVisit";
                node["Header"].Value = "Certainty: 0.4";

                node["Message"].Value = "Old-UserID: " + lastUserId + "? - New-UserID: " + userID + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
            else
            {
                // Probably new visit
                // Logged as a 90% certainty 'New-Visit' ...
                Node node = new Node();
                node["LogItemType"].Value = "Magix.Core.NewVisit";
                node["Header"].Value = "Certainty: 0.9";

                node["Message"].Value = "UserAgent: " + Page.Request.UserAgent + " - Sender: " + sender.ToString() + ".";
                if (UserBase.Current != null)
                {
                    node["ObjectID"].Value = UserBase.Current.ID;
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    sender,
                    "Magix.Core.Log",
                    node);
            }
        }
    }
}
