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
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class DashboardController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.LoadDashboard")]
        protected void Magix_Publishing_LoadDashboard(object sender, ActiveEventArgs e)
        {
            if (User.Current.InRole("Administrator"))
            {
                RaiseEvent("Magix.Publishing.LoadAdministratorDashboard", e.Params);
            }
            else
            {
                RaiseEvent("Magix.Publishing.LoadUserDashboard", e.Params);
            }
        }

        [ActiveEvent(Name = "Magix.Core.UserLoggedIn")]
        protected void Magix_Core_UserLoggedIn(object sender, ActiveEventArgs e)
        {
            // Getting relative URL ...
            string baseUrl = GetApplicationBaseUrl().ToLowerInvariant();
            string relUrl = Page.Request.Url.ToString().ToLowerInvariant().Replace("default.aspx", "").Replace(baseUrl, "");

            if (User.Current.InRole("Administrator"))
            {
                AjaxManager.Instance.Redirect("~/?dashboard=true");
            }
            else
            {
                AjaxManager.Instance.Redirect("~/");
            }
        }
    }
}
