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
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Main 'router' in dispatching important [ADMIN] Dashboard functionality 
     * [As in; Administrator user logged in]
     */
    [ActiveController]
    public class AdministratorDashboard_Controller : ActiveController
    {
        /**
         * Level2: Loads Administrator Dashboard [back-web]
         */
        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorDashboard")]
        protected void Magix_Publishing_LoadAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            RaiseEvent("Magix.Publishing.LoadAdministratorHeader");
            RaiseEvent("Magix.Publishing.LoadAdministratorMenu");

            LoadDesktopIcons();
            LoadDashboard();
        }

        private void LoadDesktopIcons()
        {
            Node node = new Node();

            RaiseEvent(
                "Magix.Publishing.GetDashBoardDesktopPlugins",
                node);

            // Ordering keyboard shortcut keys ...
            string shortcuts = "vwrazfdxcqes";
            int idxNo = 0;
            foreach (Node idx in node["Items"])
            {
                idx["Shortcut"].Value = shortcuts[idxNo % shortcuts.Length].ToString();
                idx["ToolTip"].Value = "Key: " + shortcuts[idxNo % shortcuts.Length].ToString();
                idxNo += 1;
            }

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "mux-dashboard";
            node["ChildCssClass"].Value = "mux-desktop-images";

            node["DisplayTooltips"].Value = false;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.ImageList",
                "content3",
                node);

            node = new Node();
            node["Append"].Value = true;
            node["ChildCssClass"].Value = "mux-analog-clock";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.AnalogClock",
                "content3",
                node);
        }

        /*
         * Creates the Dashboard Grid for us, by among other things raising the
         * 'Magix.Publishing.GetDataForAdministratorDashboard' event which others
         * could connect to, and fill in their own plugin extensions for
         */
        private void LoadDashboard()
        {
            Node node = new Node();

            node["ChildCssClass"].Value = "mux-desktop-dashboard-items-grid";
            node["Append"].Value = true;
            node["Container"].Value = "content3";
            node["FullTypeName"].Value = "Dashboard-Type-META";
            node["ReuseNode"].Value = true;
            node["ID"].Value = -1;

            node["WhiteListColumns"]["PagesCount"].Value = true;
            node["WhiteListColumns"]["TemplatesCount"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Name"]["Header"].Value = "Objects";
            node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["Header"].Value = "Count";
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 2;

            node["Type"]["Properties"]["PagesCount"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["PagesCount"]["Header"].Value = "Pages";
            node["Type"]["Properties"]["PagesCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditPages";
            node["Type"]["Properties"]["TemplatesCount"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["TemplatesCount"]["Header"].Value = "Templates";
            node["Type"]["Properties"]["TemplatesCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditTemplates";

            node["Object"]["ID"].Value = -1;
            node["Object"]["Properties"]["PagesCount"].Value = WebPage.Count.ToString();
            node["Object"]["Properties"]["TemplatesCount"].Value = WebPageTemplate.Count.ToString();

            node["DoNotRebind"].Value = true;

            // Getting plugins ...
            RaiseEvent(
                "Magix.Publishing.GetDataForAdministratorDashboard",
                node);

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        /**
         * Level2: Will return the icons needed to populate the 'desktop' with 
         * your most important 'shortcut icons'
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["DBManager"]["Image"].Value = "media/images/desktop-icons/database.png";
            e.Params["Items"]["DBManager"]["Shortcut"].Value = "q";
            e.Params["Items"]["DBManager"]["Text"].Value = "Click to launch Database Manager [Key D]";
            e.Params["Items"]["DBManager"]["CSS"].Value = "mux-desktop-icon mux-database";
            e.Params["Items"]["DBManager"]["Event"].Value = "Magix.Publishing.ViewClasses";

            e.Params["Items"]["FileExplorer"]["Image"].Value = "media/images/desktop-icons/briefcase.png";
            e.Params["Items"]["FileExplorer"]["Shortcut"].Value = "w";
            e.Params["Items"]["FileExplorer"]["Text"].Value = "Click to launch File Explorer [Key F]";
            e.Params["Items"]["FileExplorer"]["CSS"].Value = "mux-desktop-icon mux-filesystem";
            e.Params["Items"]["FileExplorer"]["Event"].Value = "Magix.Publishing.ViewFileSystem";

            e.Params["Items"]["Log"]["Image"].Value = "media/images/desktop-icons/oscilloscope.png";
            e.Params["Items"]["Log"]["Shortcut"].Value = "e";
            e.Params["Items"]["Log"]["Text"].Value = "Click to view Log [Key L]";
            e.Params["Items"]["Log"]["CSS"].Value = "mux-desktop-icon mux-log";
            e.Params["Items"]["Log"]["Event"].Value = "Magix.Publishing.ViewLog";

            e.Params["Items"]["Dox"]["Image"].Value = "media/images/desktop-icons/docs.png";
            e.Params["Items"]["Dox"]["Shortcut"].Value = "r";
            e.Params["Items"]["Dox"]["Text"].Value = "Click to Download Magix' Documentation in PDF Format [Key X]";
            e.Params["Items"]["Dox"]["CSS"].Value = "mux-desktop-icon mux-dox";
            e.Params["Items"]["Dox"]["Event"].Value = "Magix.Publishing.DownloadHeavyDocumentation";

            e.Params["Items"]["Pages"]["Image"].Value = "media/images/desktop-icons/press-icon.png";
            e.Params["Items"]["Pages"]["Shortcut"].Value = "a";
            e.Params["Items"]["Pages"]["Text"].Value = "Click to open Web Pages [Key P]";
            e.Params["Items"]["Pages"]["CSS"].Value = "mux-desktop-icon mux-pages";
            e.Params["Items"]["Pages"]["Event"].Value = "Magix.Publishing.EditPages";

            e.Params["Items"]["Templates"]["Image"].Value = "media/images/desktop-icons/blue-print-icon.png";
            e.Params["Items"]["Templates"]["Shortcut"].Value = "s";
            e.Params["Items"]["Templates"]["Text"].Value = "Click to open Templates [Key M]";
            e.Params["Items"]["Templates"]["CSS"].Value = "mux-desktop-icon mux-templates";
            e.Params["Items"]["Templates"]["Event"].Value = "Magix.Publishing.EditTemplates";
        }

        /**
         * Level2: Will redirect the browser towards the complete PDF documentation for Magix', including 
         * all the C# and coding parts
         */
        [ActiveEvent(Name = "Magix.Publishing.DownloadHeavyDocumentation")]
        protected void Magix_Publishing_DownloadHeavyDocumentation(object sender, ActiveEventArgs e)
        {
            AjaxManager.Instance.WriterAtBack.Write("window.open('Tmp/Magix-For-Believing-CSharp-People.pdf');");
        }

        /**
         * Level2: Loads the Administrator SlidingMenu into the 1st content, but 
         * everything here is basically overridable. Will also raise
         * 'Magix.Publishing.GetPluginMenuItems' to allow for plugins to connect
         * up their own Administrator Dashboard menu items
         */
        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorMenu")]
        protected void Magix_Publishing_LoadAdministratorMenu(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("Top"))
                e.Params["Top"].Value = 6;

            if (!e.Params.Contains("Height"))
                e.Params["Height"].Value = 15;

            if (!e.Params.Contains("Width"))
                e.Params["Width"].Value = 6;

            if (!e.Params.Contains("Caption"))
                e.Params["Caption"].Value = "Menu";

            if (!e.Params.Contains("CssClass"))
                e.Params["CssClass"].Value = "administrator-menu";

            e.Params["Items"]["Home"]["Caption"].Value = "Dashboard ...";
            e.Params["Items"]["Home"]["Selected"].Value = true;
            e.Params["Items"]["Home"]["AccessKey"].Value = "P";
            e.Params["Items"]["Home"]["Event"]["Name"].Value = "Magix.Publishing.LoadAdministratorDashboard";
            e.Params["Items"]["Publishing"]["Caption"].Value = "Publishing";
            e.Params["Items"]["Publishing"]["Items"]["Pages"]["Caption"].Value = "Pages ...";
            e.Params["Items"]["Publishing"]["Items"]["Pages"]["Event"]["Name"].Value = "Magix.Publishing.EditPages";
            e.Params["Items"]["Publishing"]["Items"]["Templates"]["Caption"].Value = "Templates ...";
            e.Params["Items"]["Publishing"]["Items"]["Templates"]["Event"]["Name"].Value = "Magix.Publishing.EditTemplates";

            // Putting plugins just beneath Publishing menu item ...
            RaiseEvent(
                "Magix.Publishing.GetPluginMenuItems",
                e.Params);

            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
                e.Params["Items"]["Admin"]["Items"]["System"]["Caption"].Value = "System";
            }
            if (!e.Params["Items"]["Admin"]["Items"]["System"]["Items"].Contains("DBAdmin"))
            {
                e.Params["Items"]["Admin"]["Items"]["System"]["Caption"].Value = "System";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["DBAdmin"]["Caption"].Value = "Database ...";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["DBAdmin"]["Event"]["Name"].Value = "Magix.Publishing.ViewClasses";
            }
            if (!e.Params["Items"]["Admin"]["Items"]["System"]["Items"].Contains("Explorer"))
            {
                e.Params["Items"]["Admin"]["Items"]["System"]["Caption"].Value = "System";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["Explorer"]["Caption"].Value = "File system ...";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["Explorer"]["Event"]["Name"].Value = "Magix.Publishing.ViewFileSystem";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Roles"))
            {
                e.Params["Items"]["Admin"]["Items"]["Roles"]["Caption"].Value = "Roles ...";
                e.Params["Items"]["Admin"]["Items"]["Roles"]["Event"]["Name"].Value = "Magix.Publishing.EditRoles";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Users"))
            {
                e.Params["Items"]["Admin"]["Items"]["Users"]["Caption"].Value = "Users ...";
                e.Params["Items"]["Admin"]["Items"]["Users"]["Event"]["Name"].Value = "Magix.Publishing.EditUsers";
            }
            if (!e.Params["Items"]["Admin"]["Items"]["System"]["Items"].Contains("Log"))
            {
                e.Params["Items"]["Admin"]["Items"]["System"]["Caption"].Value = "System";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["Log"]["Caption"].Value = "Log ...";
                e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["Log"]["Event"]["Name"].Value = "Magix.Publishing.ViewLog";
            }

            if (!e.Params["Items"].Contains("LogOut"))
            {
                e.Params["Items"]["LogOut"]["Caption"].Value = "Logout!";
                e.Params["Items"]["LogOut"]["Event"]["Name"].Value = "Magix.Core.UserLoggedOut";
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Menu.Slider",
                "content1",
                e.Params);
        }

        /**
         * Level2: Ads up the 'common data' for the Admin Dashboard such as Users, Roles etc
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["LogItemCount"].Value = true;
            e.Params["Type"]["Properties"]["LogItemCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["LogItemCount"]["Header"].Value = "Log Items";
            e.Params["Type"]["Properties"]["LogItemCount"]["ClickLabelEvent"].Value = "Magix.Publishing.ViewLog";
            e.Params["Object"]["Properties"]["LogItemCount"].Value = LogItem.Count.ToString();

            e.Params["WhiteListColumns"]["UserCount"].Value = true;
            e.Params["Type"]["Properties"]["UserCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserCount"]["Header"].Value = "Users";
            e.Params["Type"]["Properties"]["UserCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditUsers";
            e.Params["Object"]["Properties"]["UserCount"].Value = User.Count.ToString();

            e.Params["WhiteListColumns"]["RoleCount"].Value = true;
            e.Params["Type"]["Properties"]["RoleCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["RoleCount"]["Header"].Value = "Roles";
            e.Params["Type"]["Properties"]["RoleCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditRoles";
            e.Params["Object"]["Properties"]["RoleCount"].Value = Role.Count.ToString();
        }

        /**
         * Level2: Will fire up our Database Manager
         */
        [ActiveEvent(Name = "Magix.Publishing.ViewClasses")]
        private void Magix_Publishing_ViewClasses(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Container"].Value = "child";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Container"].Value = "content3";
            node["IsFilterColumns"].Value = true;
            node["Header"].Value = Adapter.Instance.GetConnectionString();
            node["HeaderFontSize"].Value = 16;

            RaiseEvent(
                "DBAdmin.Form.ViewClasses",
                node);

            node = new Node();

            node["Caption"].Value = "Database Objects";

            RaiseEvent(
                "Magix.Core.SetFormCaption",
                node);
        }
    }
}
