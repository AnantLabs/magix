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
    public class InitialLoading_Controller : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.InitialLoading")]
        protected void Magix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
            // We always log user out if he visits the 'login URL' ...
            if (Page.Request.Params["login"] == "true")
                User.Current = null;

            // Including standard CSS files ...
            IncludeCssFiles();

            if (ShouldShowLoginBox())
            {
                // Loading login module ...
                Node node = new Node();

                node["Container"].Value = "content1";
                node["Width"].Value = 8;
                node["Push"].Value = 8;
                node["Last"].Value = true;
                node["Top"].Value = 5;
                node["Focus"].Value = true;

                RaiseEvent(
                    "Magix.Core.LoadLoginModule",
                    node);
            }
            else if(ShouldShowDashboard())
            {
                RaiseEvent("Magix.Publishing.LoadDashboard");
            }
            else
            {
                // Checking to see if user is requesting dashboard
                if (Page.Request.Params["dashboard"] == "true")
                {
                    // User is requesting Dashboard, but is not logged in as an admin ...
                    // Redirecting to Login 'page' and giving the return URL to be the current one

                    string nUrl = 
                        "~/?login=true&ret=" + 
                        Page.Server.UrlEncode(Page.Request.Url.ToString());

                    AjaxManager.Instance.Redirect(nUrl);
                }
                else
                {
                    // Getting relative URL ...
                    string relUrl = ""; // Defaulting to 'root' ...
                    if (!string.IsNullOrEmpty(Page.Request.Params["page"]))
                        relUrl = Page.Request.Params["page"];

                    Node node = new Node();
                    node["URL"].Value = relUrl;

                    RaiseEvent(
                        "Magix.Publishing.UrlRequested",
                        node);
                }
            }
        }

        private bool ShouldShowDashboard()
        {
            return User.Current != null &&
                Page.Request.Params["dashboard"] == "true" &&
                User.Current.InRole("Administrator");
        }

        private bool ShouldShowLoginBox()
        {
            bool retVal = false;

            // If user specifically asks for login page ...
            if (Page.Request.Params["login"] == "true")
                retVal = true;

            // User [or another machine] has asked for OpenID credentials ...
            if (!string.IsNullOrEmpty(Page.Request.Params["openID"]))
                retVal = true;

            return retVal;
        }

        private void IncludeCssFiles()
        {
            IncludeCssFile("media/magix-ux-skins/default.css");
            IncludeCssFile("media/modules/SingleContainer.css");
        }
    }
}
