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
    public class PublishingController : ActiveController
    {
        [ActiveEvent(Name = "Brix.Core.InitialLoading")]
        protected void Brix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
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
                if (Page.Request.Params["dashboard"] == "true")
                {
                    string nUrl = "~/?login=true";

                    string baseUrl = GetApplicationBaseUrl();
                    string curUrl = Page.Request.Url.ToString();

                    if (baseUrl != curUrl)
                    {
                        nUrl += "&ret=" + Page.Server.UrlEncode(curUrl);
                    }
                    AjaxManager.Instance.Redirect(nUrl);
                }
                else
                {
                    // Getting relative URL ...
                    string baseUrl = GetApplicationBaseUrl();
                    string relUrl = Page.Request.Url.ToString().Replace(baseUrl, "");

                    if (relUrl.IndexOf('?') != -1)
                    {
                        relUrl = relUrl.Substring(0, relUrl.IndexOf('?'));
                    }

                    Node node = new Node();

                    node["URL"].Value = relUrl;
                    node["BaseURL"].Value = baseUrl;

                    RaiseEvent(
                        "Magix.Publishing.UrlRequested",
                        node);
                }
            }
        }

        private bool ShouldShowDashboard()
        {
            return User.Current != null &&
                User.Current.InRole("Administrator") &&
                Page.Request.Params["dashboard"] == "true";
        }

        private bool ShouldShowLoginBox()
        {
            return (User.Current == null &&
                Page.Request.Params["login"] == "true") || 
                !string.IsNullOrEmpty(Page.Request.Params["openID"]);
        }

        private void IncludeCssFiles()
        {
            IncludeCssFile("media/magix-ux-skins/default.css");
            IncludeCssFile("media/modules/SingleContainer.css");
        }
    }
}
