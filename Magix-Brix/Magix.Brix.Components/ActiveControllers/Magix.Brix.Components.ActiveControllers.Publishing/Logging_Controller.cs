/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Web;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.Logging;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     *Level2:  Main 'router' in dispatching important [ADMIN] Dashboard functionality 
     * [As in; Administrator user logged in]
     */
    [ActiveController]
    public class Logging_Controller : ActiveController
    {
        /**
         * Level2: Will display the Grid of the latest Log Items for the user to be able
         * to drill down, and have a more thourough look
         */
        [ActiveEvent(Name = "Magix.Publishing.ViewLog")]
        private void Magix_Publishing_ViewLog(object sender, ActiveEventArgs e)
        {
            // Resetting counter
            HttpContext.Current.Session.Remove("LogCount");

            Node node = new Node();

            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["FullTypeName"].Value = typeof(LogItem).FullName;
            node["IsCreate"].Value = false;
            node["IsDelete"].Value = false;
            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "View";
            node["IDColumnValue"].Value = "View";
            node["IDColumnEvent"].Value = "Magix.Publishing.ViewLogItem";

            node["WhiteListColumns"]["LogItemType"].Value = true;
            node["WhiteListColumns"]["LogItemType"]["ForcedWidth"].Value = 6;
            node["WhiteListColumns"]["Header"].Value = true;
            node["WhiteListColumns"]["Header"]["ForcedWidth"].Value = 10;

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "When";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["LogItemType"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Header"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Header"]["MaxLength"].Value = 60;

            node["GetObjectsEvent"].Value = "Magix.Publishing.GetLogItems";

            // 'Passing through' ...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will return the last log items according to newest first
         */
        [ActiveEvent(Name = "Magix.Publishing.GetLogItems")]
        private void Magix_Publishing_GetLogItems(object sender, ActiveEventArgs e)
        {
            // All we need to do is to inject our own little sorting order ...
            e.Params["Criteria"]["C1"]["Name"].Value = "Sort";
            e.Params["Criteria"]["C1"]["Value"].Value = "When";
            e.Params["Criteria"]["C1"]["Ascending"].Value = false;

            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "DBAdmin.Data.GetContentsOfClass",
                e.Params);
        }

        /**
         * Level2: Will show one LogItem in a grid for the end-user to scrutinize. Will allow the user to
         * always have to LogItems up at the same time
         */
        [ActiveEvent(Name = "Magix.Publishing.ViewLogItem")]
        protected void Magix_Publishing_ViewLogItem(object sender, ActiveEventArgs e)
        {
            // Getting the requested User ...
            LogItem l = LogItem.SelectByID(e.Params["ID"].Get<int>());

            e.Params["Width"].Value = 24;
            e.Params["Last"].Value = true;
            e.Params["ClearBoth"].Value = true;
            e.Params["MarginBottom"].Value = 10;

            // First filtering OUT columns ...!
            e.Params["WhiteListColumns"]["LogItemType"].Value = true;
            e.Params["WhiteListColumns"]["When"].Value = true;
            e.Params["WhiteListColumns"]["Header"].Value = true;
            e.Params["WhiteListColumns"]["Message"].Value = true;
            e.Params["WhiteListColumns"]["ObjectID"].Value = true;
            e.Params["WhiteListColumns"]["ParentID"].Value = true;
            e.Params["WhiteListColumns"]["StackTrace"].Value = true;
            e.Params["WhiteListColumns"]["IPAddress"].Value = true;
            e.Params["WhiteListColumns"]["User"].Value = true;
            e.Params["WhiteListColumns"]["UserID"].Value = true;

            e.Params["WhiteListProperties"]["Name"].Value = true;
            e.Params["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 2;
            e.Params["WhiteListProperties"]["Value"].Value = true;
            e.Params["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 4;

            e.Params["Type"]["Properties"]["LogItemType"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["LogItemType"]["Bold"].Value = true;
            e.Params["Type"]["Properties"]["Header"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["When"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["Message"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["ObjectID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["ParentID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["StackTrace"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["IPAddress"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["User"]["ReadOnly"].Value = true;

            if (!e.Params.Contains("Append"))
            {
                if (HttpContext.Current.Session["LogCount"] == null)
                {
                    HttpContext.Current.Session["LogCount"] = 0;
                }
                int val = (int)HttpContext.Current.Session["LogCount"];
                if (val % 2 != 0)
                {
                    e.Params["ChildCssClass"].Value = "mux-rounded mux-shaded span-10 prepend-top mux-paddings last";
                }
                else
                {
                    e.Params["ChildCssClass"].Value = "mux-rounded mux-shaded span-10 prepend-top mux-paddings";
                }
                e.Params["Append"].Value = val % 2 != 0;
                HttpContext.Current.Session["LogCount"] = (val + 1) % 2;
            }
            e.Params["Container"].Value = "content4";
            e.Params["Caption"].Value =
                string.Format(
                    "Editing LogItem: {0}",
                    l.Header);

            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "DBAdmin.Form.ViewComplexObject",
                e.Params);
        }
    }
}
