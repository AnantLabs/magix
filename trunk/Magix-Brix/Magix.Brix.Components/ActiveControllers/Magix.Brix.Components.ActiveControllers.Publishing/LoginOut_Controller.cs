﻿/*
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
     * Login and Logout Controller
     */
    [ActiveController]
    public class LoginOut_Controller : ActiveController
    {
        /**
         * Basically just checks to see if the given username/password 
         * combination exists, and if so sets the User.Current object, which
         * is staic per request, returning the logged in user. Then if successful,
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
         * Resets the User.Current object, and redirects user back to root
         */
        [ActiveEvent(Name = "Magix.Core.UserLoggedOut")]
        private void Magix_Core_UserLoggedIn(object sender, ActiveEventArgs e)
        {
            // Logging out...
            UserBase.Current = null;

            // Redirecting back to landing page, to 'invalidate' DOM ...!
            AjaxManager.Instance.Redirect(GetApplicationBaseUrl());
        }
    }
}
