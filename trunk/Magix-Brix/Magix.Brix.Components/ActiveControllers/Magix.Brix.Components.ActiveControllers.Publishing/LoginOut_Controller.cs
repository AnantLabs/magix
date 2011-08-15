/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using System.Net;
using System.IO;
using Magix.UX;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.Messaging;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Login and Logout Controller
     */
    [ActiveController]
    public class LoginOut_Controller : ActiveController
    {
        /**
         * Level2: Basically just checks to see if the given username/password 
         * combination exists, and if so sets the User.Current object, which
         * is static per request, returning the logged in user. Then if successful,
         * raises the 'Magix.Core.UserLoggedIn' event
         */
        [ActiveEvent(Name = "Magix.Core.LogInUser")]
        protected void Magix_Core_LogInUser(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string password = e.Params["Password"].Get<string>();

            User u = User.SelectFirst(
                Criteria.Eq("Username", username),
                Criteria.Eq("Password", password));

            if (u != null)
            {
                e.Params["Success"].Value = true;
                User.Current = u;

                RaiseEvent("Magix.Core.UserLoggedIn");
            }
        }

        /**
         * Level2: Redirects the user to Root, unless another Redirect path
         * is somehow given
         */
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

        /**
         * Level2: Resets the User.Current object, and redirects user back to root
         */
        [ActiveEvent(Name = "Magix.Core.UserLoggedOut")]
        private void Magix_Core_UserLoggedOut(object sender, ActiveEventArgs e)
        {
            // Logging out...
            UserBase.Current = null;

            // Redirecting back to landing page, to 'invalidate' DOM ...!
            AjaxManager.Instance.Redirect(GetApplicationBaseUrl());
        }

        /**
         * Level2: Returns whether or not the User is logged in or not for the LoginInOut Module
         * to know what types of controls to load
         */
        [ActiveEvent(Name = "Magix.Publishing.GetStateForLoginControl")]
        protected void Magix_Publishing_GetStateForLoginControl(object sender, ActiveEventArgs e)
        {
            if (User.Current != null)
            {
                e.Params["ShouldLoadLogout"].Value = true;
            }
            else
            {
                e.Params["ShouldLoadLogin"].Value = true;
            }
        }
    }
}
