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
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Class for taking care of the Magix.Core.InitialLoading message, meaning
     * the initialization of the page on a per User level initially as they're coming
     * to a new 'page' or URL ...
     * Basically just handles Magix.Core.InitialLoading and mostly delegates from that
     * point ...
     */
    [ActiveController]
    public class InitialLoading_Controller : ActiveController
    {
        /**
         * Level2: This method will handle the 'initial loading', meaning basically that the page
         * was loaded initially by either changing the URL of the browser to the app or 
         * doing a Refresh or something. For coders, meaning basically not IsPostBack on 
         * the page object ...
         * Throws a whole range of different events based upon whether or not the User
         * is logged in or not, and which URL is being specifically requested. Its most
         * noticable outgoing event would though be 'Magix.Publishing.LoadDashboard' and
         * 'Magix.Publishing.UrlRequested'. PS! If you override this one, you've effectively 
         * completely bypassed every single existing logic in Magix, and everything are
         * 'dead event handlers' in 'limbo' not tied together at all. Which might be cool,
         * or might be a nightmare, depending upon how you use it, if you use it, use it with CARE 
         * if you do though !!
         */
        [ActiveEvent(Name = "Magix.Core.InitialLoading")]
        protected void Magix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
            // We always log user out if he visits the 'login URL' ...
            if (Page.Request.Params["login"] == "true")
                User.Current = null;

            // Including standard CSS files ...
            IncludeCssFiles();

            if (ShouldExplicitlyShowLoginForm())
            {
                // Loading login module ...
                Node node = new Node();

                node["Width"].Value = 8;
                node["Push"].Value = 8;
                node["Last"].Value = true;
                node["Top"].Value = 5;
                node["Focus"].Value = true;

                LoadModule(
                    "Magix.Brix.Components.ActiveModules.Users.Login",
                    "content1",
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

                    // Requesting our 'URL' ...
                    Node node = new Node();
                    node["URL"].Value = relUrl;

                    RaiseEvent(
                        "Magix.Publishing.UrlRequested",
                        node);
                }
            }
        }

        /*
         * Returns true if we should show Dashboard 
         * [meaning Admin is logged in and trying to reach Dashboard]
         */
        private bool ShouldShowDashboard()
        {
            return User.Current != null &&
                Page.Request.Params["dashboard"] == "true" &&
                User.Current.InRole("Administrator");
        }

        /*
         * Did we explicitly ask for a LoginBox ...?
         */
        private bool ShouldExplicitlyShowLoginForm()
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

        /*
         * Includes the Static CSS files needed mostly 'all over the place' ...
         */
        private void IncludeCssFiles()
        {
            IncludeCssFile("media/magix-ux-skins/default.css");
            IncludeCssFile("media/modules/single-container.css");

            if (Page.Request.Params["dashboard"] == "true")
            {
                IncludeCssFile("media/modules/dashboard.css");
            }
        }
    }
}
