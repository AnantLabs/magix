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

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class PublishingController : ActiveController
    {
        [ActiveEvent(Name = "Brix.Core.InitialLoading")]
        protected void Page_Init_InitialLoading(object sender, ActiveEventArgs e)
        {
            // Including standard CSS files ...
            IncludeCssFiles();

            // Checking to see if some other modules have handled our request and wants
            // to be left alone ...
            if (ShouldShowLoginBox())
            {
                // Loading login module ...
                Node node = new Node();

                node["Container"].Value = "content1";
                node["Width"].Value = 8;
                node["Push"].Value = 8;
                node["Last"].Value = true;
                node["Top"].Value = 5;

                RaiseEvent(
                    "Magix.Core.LoadLoginModule",
                    node);
            }
            else
            {
                // Getting relative URL ...
                string baseUrl = GetApplicationBaseUrl().ToLowerInvariant();
                string relUrl = Page.Request.Url.ToString().ToLowerInvariant().Replace("default.aspx", "").Replace(baseUrl, "");

                if (relUrl.IndexOf('?') != -1)
                {
                    relUrl = relUrl.Substring(0, relUrl.IndexOf('?'));
                }

                Node node = new Node();

                node["URL"].Value = relUrl;

                RaiseEvent(
                    "Magix.Publishing.UrlRequested",
                    node);
            }
        }

        private bool ShouldShowLoginBox()
        {
            return Page.Request.Params["login"] == "true";
        }

        private void IncludeCssFiles()
        {
            IncludeCssFile("media/magix-ux-skins/default.css");
            IncludeCssFile("media/modules/SingleContainer.css");
        }
    }
}
