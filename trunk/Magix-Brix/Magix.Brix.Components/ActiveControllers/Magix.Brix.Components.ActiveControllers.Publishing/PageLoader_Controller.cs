/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Controller responsible for loading up 'Page Objects', 
     * and doing initialization around pages and such as they're 
     * being loaded
     */
    [ActiveController]
    public class PageLoader_Controller : ActiveController
    {
        /**
         * Level2: Basically just 're-maps' the event to Magix.Publishing.OpenPage and
         * changes the incoming URL parameter to an outgoing ID parameter of the 
         * specific page being requested ...
         */
        [ActiveEvent(Name = "Magix.Publishing.UrlRequested")]
        protected void Magix_Publishing_UrlRequested(object sender, ActiveEventArgs e)
        {
            // Including static Front-End CSS file(s) ...
            IncludeStaticFrontEndCSS();

            // Figuring out the URL and 'normalizing it' in case it was shorthand
            // typed in ...
            string url = e.Params["URL"].Get<string>();
            if (url.Length > 0 && url[0] != '/')
                url = "/" + url;

            // Finding the page with the given URL [which now should be e.g.; "" or "/New/App"]
            WebPage p = WebPage.SelectFirst(Criteria.Eq("URL", url));

            // Opening our 'page' ...
            Node node = new Node();
            node["ID"].Value = p.ID;

            RaiseEvent(
                "Magix.Publishing.OpenPage",
                node);
        }

        /**
         * Includes static CSS for our front-end
         */
        private void IncludeStaticFrontEndCSS()
        {
            Node node = new Node();
            node["CSSFile"].Value = GetApplicationBaseUrl() + "media/front-end.css";

            RaiseEvent(
                "Magix.Core.AddCustomCssFile",
                node);
        }

        // TODO: Create a Startup Default Action encapsulating method to hint towards
        // user that he can actually do this himself to bypass the menu and such...
        /**
         * Level2: Expects an ID parameter which should be the ID of a Page.
         * Will loop through every WebPart of the page, and raise 
         * 'Magix.Publishing.InjectPlugin' for every WebPart within 
         * the WebPage object. Might throw exceptions if the page is
         * not found, or the user has no access rights to that specific 
         * page or something ...
         * Will verify the user has access to the page, by raising 
         * 'Magix.Publishing.VerifyUserHasAccessToPage' and if no access
         * is granted, try to find the first child object the user has
         * access to through raising the 
         * 'Magix.Publishing.FindFirstChildPageUserCanAccess' event.
         * Will throw SecurityException or ArgumentException if page
         * not found or granted access to [nor any of its children]
         */
        [ActiveEvent(Name = "Magix.Publishing.OpenPage")]
        protected void Magix_Publishing_OpenPage(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

            if (p == null)
            {
                throw new ArgumentException("Page not found ...");
            }
            else
            {
                Node ch1 = new Node();
                ch1["ID"].Value = p.ID;

                RaiseEvent(
                    "Magix.Publishing.VerifyUserHasAccessToPage",
                    ch1);

                if (ch1.Contains("STOP") &&
                    ch1["STOP"].Get<bool>())
                {
                    // Finding first page level user [or anonymous] have access to from here ...
                    Node node = new Node();
                    node["ID"].Value = p.ID;
                    RaiseEvent(
                        "Magix.Publishing.FindFirstChildPageUserCanAccess",
                        node);

                    if (!node.Contains("AccessToID"))
                        throw new SecurityException(
                            "You don't have access to neither this page nor any of its children ...");

                    OpenPage(WebPage.SelectByID(node["AccessToID"].Get<int>()));
                }
                else
                {
                    OpenPage(p);
                }
            }
        }

        /**
         * Helper method used in 'Magix.Publishing.OpenPage' for opening the page
         * once it has been granted access to either directly, or one of its child pages.
         * Will basically just loop through every WebPart in the Page and raise 
         * 'Magix.Publishing.InjectPlugin' for each of them, after doing some manipulation
         * of the Container the WebPart is suppsed to become injected into and such. Will
         * also set the title of the page according to the name of the WebPage
         */
        private void OpenPage(WebPage page)
        {
            SetCaptionOfPage(page);

            string lastModule = "content1";

            int cl = 0;
            foreach (WebPart idx in page.WebParts)
            {
                if (idx.Container == null)
                    continue; // Skipping these buggers ...
                if (idx.Container.ViewportContainer.CompareTo(lastModule) > 0)
                    lastModule = idx.Container.ViewportContainer;

                Node tmp = new Node();

                tmp["ID"].Value = idx.ID;

                RaiseEvent(
                    "Magix.Publishing.InjectPlugin",
                    tmp);

                cl = int.Parse(lastModule.Replace("content", ""));
            }

            // Making sure 'left over' Containers are cleared ...
            if (cl < 7)
                ActiveEvents.Instance.RaiseClearControls("content" + (cl + 1));
        }

        /**
         * Sets the title of the page by raising 
         * 'Magix.Core.SetTitleOfPage' event according to the Name of the Page
         */
        private void SetCaptionOfPage(WebPage page)
        {
            Node node = new Node();

            node["Caption"].Value = page.Name;

            RaiseEvent(
                "Magix.Core.SetTitleOfPage",
                node);
        }

        /**
         * Level2: Will return the Container ID of the 'Current Container', meaning whomever 
         * raised whatever event we're currently within. Meaning if you've got a Button
         * in a Grid which raises some event which is dependent upon knowing which Container
         * its being raised from within, to load some other control [e.g. Signature Column in Grid
         * or Edit Object]
         * Is dependent upon that the PageObjectTemplateID is being passed around some way or
         * another into this bugger
         */
        [ActiveEvent(Name = "Magix.Core.GetContainerForControl")]
        protected void Magix_Core_GetContainerForControl(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("OriginalWebPartID"))
            {
                e.Params["Container"].Value =
                    WebPart.SelectByID(e.Params["OriginalWebPartID"].Get<int>()).Container.ViewportContainer;
                e.Params["FreezeContainer"].Value = true;
            }
        }
    }
}
