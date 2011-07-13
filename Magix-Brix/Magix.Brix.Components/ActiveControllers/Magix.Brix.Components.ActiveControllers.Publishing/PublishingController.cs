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

            // Getting relative URL ...
            string baseUrl = GetApplicationBaseUrl();
            string relUrl = Page.Request.Url.ToString().Replace(baseUrl, "");
        }

        private void IncludeCssFiles()
        {
            IncludeCssFile("media/magix-ux-skins/default.css");
            IncludeCssFile("media/modules/SingleContainer.css");
        }
    }
}
