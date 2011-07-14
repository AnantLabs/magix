/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Web;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class AdministratorDashboardController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorDashboard")]
        protected void Magix_Publishing_LoadAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            RaiseEvent("Magix.Publishing.LoadHeader");
            RaiseEvent("Magix.Publishing.LoadAdministratorMenu");

            // For now, just loading EditPages ...
            // TODO: Implement Actual DashBoard ...
            RaiseEvent("Magix.Publishing.EditPages");
        }

        // This method has every single change to its input Node structure being overridable ...
        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorMenu")]
        protected void Magix_Publishing_LoadAdministratorMenu(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("Top"))
                e.Params["Top"].Value = 4;

            if (!e.Params.Contains("Height"))
                e.Params["Height"].Value = 17;

            if (!e.Params.Contains("Width"))
                e.Params["Width"].Value = 6;

            if (!e.Params.Contains("Caption"))
                e.Params["Caption"].Value = "Menu";

            if (!e.Params.Contains("CssClass"))
                e.Params["CssClass"].Value = "administrator-menu";

            if (!e.Params["Items"].Contains("Home"))
            {
                e.Params["Items"]["Home"]["Caption"].Value = "Dashboard ...";
                e.Params["Items"]["Home"]["Selected"].Value = true;
                e.Params["Items"]["Home"]["Event"]["Name"].Value = "Magix.Publishing.LoadAdministratorDashboard";
            }
            if (!e.Params["Items"].Contains("Publishing"))
            {
                e.Params["Items"]["Publishing"]["Caption"].Value = "Publishing";
            }
            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("DBAdmin"))
            {
                e.Params["Items"]["Admin"]["Items"]["DBAdmin"]["Caption"].Value = "Database ...";
                e.Params["Items"]["Admin"]["Items"]["DBAdmin"]["Event"]["Name"].Value = "Magix.Publishing.ViewClasses";
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
            if (!e.Params["Items"]["Publishing"]["Items"].Contains("Pages"))
            {
                e.Params["Items"]["Publishing"]["Items"]["Pages"]["Caption"].Value = "Pages ...";
                e.Params["Items"]["Publishing"]["Items"]["Pages"]["Event"]["Name"].Value = "Magix.Publishing.EditPages";
            }
            if (!e.Params["Items"]["Publishing"]["Items"].Contains("Templates"))
            {
                e.Params["Items"]["Publishing"]["Items"]["Templates"]["Caption"].Value = "Templates ...";
                e.Params["Items"]["Publishing"]["Items"]["Templates"]["Event"]["Name"].Value = "Magix.Publishing.EditTemplates";
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

        [ActiveEvent(Name = "Magix.Publishing.ViewClasses")]
        private void Magix_Publishing_ViewClasses(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["Caption"].Value = "Browsing Database of '" +
                Adapter.Instance.GetConnectionString() + "'";
            node["FontSize"].Value = 18;
            node["Lock"].Value = true;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.SetFormCaption",
                node);

            node = new Node();

            node["container"].Value = "child";
            node["Width"].Value = 19;
            node["WindowCssClass"].Value = "mux-shaded mux-rounded browser";
            node["Caption"].Value = "Browse classes";
            node["NoHeader"].Value = true;
            node["CloseEvent"].Value = "Magix.Publishing.ViewClassesClosed";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClasses",
                node);
        }
    }
}
