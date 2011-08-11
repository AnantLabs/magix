/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
            string baseUrl = GetApplicationBaseUrl();
            string relUrl = Page.Request.Url.ToString().Replace("default.aspx", "").Replace(baseUrl, "");
            string redirect = Page.Request.Params["ret"];

            if (!string.IsNullOrEmpty(redirect))
            {
                AjaxManager.Instance.Redirect(redirect);
            }
            else
            {
                AjaxManager.Instance.Redirect("~/");
            }
        }
    }
}
