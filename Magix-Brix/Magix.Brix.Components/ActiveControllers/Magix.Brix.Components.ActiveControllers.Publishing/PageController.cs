/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class PageController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.InitializePublishingPlugin")]
        protected void Magix_Publishing_InitializePublishingPlugin(object sender, ActiveEventArgs e)
        {
            WebPart t = WebPart.SelectByID(e.Params["ID"].Get<int>());
            Type moduleType = Adapter.ActiveModules.Find(
                delegate(Type idx)
                {
                    return idx.FullName == t.Container.ModuleName;
                });
            foreach (PropertyInfo idx in 
                moduleType.GetProperties(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Instance))
            {
                ModuleSettingAttribute[] atrs =
                    idx.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                    as ModuleSettingAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    string propName = idx.Name;
                    foreach (WebPart.WebPartSetting idxSet in t.Settings)
                    {
                        if (idxSet.Name == moduleType.FullName + idx.Name)
                        {
                            idx.GetSetMethod(true).Invoke(e.Params["_ctrl"].Value, new object[] { idxSet.Value });
                            break;
                        }
                    }
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.UrlRequested")]
        protected void Magix_Publishing_UrlRequested(object sender, ActiveEventArgs e)
        {
            string url = e.Params["URL"].Get<string>();

            WebPage p = WebPage.SelectFirst(Criteria.Eq("URL", url));

            Node node = new Node();
            node["ID"].Value = p.ID;

            RaiseEvent(
                "Magix.Publishing.OpenPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.FindFirstPageRequestCanAccess")]
        protected void Magix_Publishing_FindFirstPageRequestCanAccess(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

            // Assuming already checked for access against this bugger ...
            foreach (WebPage idx in p.Children)
            {
                if (CheckAccess(p, e.Params))
                {
                    return;
                }
            }
        }

        private bool CheckAccess(WebPage p, Node node)
        {
            Node ch1 = new Node();
            ch1["ID"].Value = p.ID;
            RaiseEvent(
                "Magix.Publishing.CanLoadPageObject",
                ch1);

            if (!ch1.Contains("STOP") ||
                !ch1["STOP"].Get<bool>())
            {
                node["AccessToID"].Value = p.ID;
                return true;
            }
            foreach (WebPage idx in p.Children)
            {
                if (CheckAccess(idx, node))
                {
                    return true;
                }
            }
            return false;
        }

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
                    "Magix.Publishing.CanLoadPageObject",
                    ch1);

                if (ch1.Contains("STOP") &&
                    ch1["STOP"].Get<bool>())
                {
                    if (p.URL.Trim('/') == "")
                    {
                        // Finding first page level user [or anonymous] have access to from here ...
                        Node node = new Node();
                        node["ID"].Value = p.ID;
                        RaiseEvent(
                            "Magix.Publishing.FindFirstPageRequestCanAccess",
                            node);
                        if(!node.Contains("AccessToID"))
                            throw new ArgumentException("You don't have access to this website ...");
                        OpenPage(WebPage.SelectByID(node["AccessToID"].Get<int>()));
                    }
                    else
                    {
                        throw new ArgumentException("You don't have access to this page ...");
                    }
                }
                else
                {
                    OpenPage(p);
                }
            }
        }

        private void OpenPage(WebPage page)
        {
            SetCaptionOfPage(page);

            string lastModule = "content1";

            foreach (WebPart idx in page.WebParts)
            {
                if (idx.Container.ViewportContainer.CompareTo(lastModule) > 0)
                    lastModule = idx.Container.ViewportContainer;

                Node tmp = new Node();

                tmp["ID"].Value = idx.ID;

                RaiseEvent(
                    "Magix.Publishing.InjectPlugin",
                    tmp);

                int cl = int.Parse(lastModule.Replace("content", ""));
                if (cl < 7)
                    ActiveEvents.Instance.RaiseClearControls("content" + (cl + 1));
            }
        }

        private void SetCaptionOfPage(WebPage page)
        {
            Node node = new Node();

            node["Caption"].Value = page.Name;

            RaiseEvent(
                "Magix.Core.SetTitleOfPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.InjectPlugin")]
        private void Magix_Publishing_InjectPlugin(object sender, ActiveEventArgs e)
        {
            WebPart page = WebPart.SelectByID(e.Params["ID"].Get<int>());

            Node ch = new Node();

            ch["ModuleName"].Value = page.Container.ModuleName;
            ch["Container"].Value = page.Container.ViewportContainer;

            RaiseEvent(
                "Magix.Publishing.ShouldReloadWebPart",
                ch);

            if (!ch.Contains("Stop") || !ch["Stop"].Get<bool>())
            {
                Node node = new Node();

                if (page.Container.MarginBottom > 0)
                    node["BottomMargin"].Value = page.Container.MarginBottom;
                if (!string.IsNullOrEmpty(page.Container.CssClass))
                    node["CssClass"].Value = page.Container.CssClass;
                if (page.Container.Height > 0)
                    node["Height"].Value = page.Container.Height;
                if (page.Container.Last)
                    node["Last"].Value = page.Container.Last;
                if (page.Container.MarginRight > 0)
                    node["PushRight"].Value = page.Container.MarginRight;
                if (page.Container.MarginLeft > 0)
                    node["PushLeft"].Value = page.Container.MarginLeft;
                if (page.Container.MarginBottom > 0)
                    node["SpcBottom"].Value = page.Container.MarginBottom;
                if (page.Container.MarginTop > 0)
                    node["Top"].Value = page.Container.MarginTop;
                if (page.Container.Width > 0)
                    node["Width"].Value = page.Container.Width;
                node["ID"].Value = page.ID;
                node["ModuleInitializationEvent"].Value = "Magix.Publishing.InitializePublishingPlugin";
                node["PageObjectTemplateID"].Value = page.ID;

                string cssClass = node["CssClass"].Get<string>() ?? "";
                cssClass += " web-part";
                node["CssClass"].Value = cssClass;

                LoadModule(
                    page.Container.ModuleName,
                    page.Container.ViewportContainer,
                    node);
            }
            else
            {
                // Don't need to Inject Module for some reasons. It might be a Sliding Menu for instance ...
                // Though we DO need to UPDATE SETTINGS for module, since it might still be a different template ...
                Node node = new Node();

                if (page.Container.MarginBottom > 0)
                    node["BottomMargin"].Value = page.Container.MarginBottom;

                if (!string.IsNullOrEmpty(page.Container.CssClass))
                    node["CssClass"].Value = page.Container.CssClass;

                if (page.Container.Height > 0)
                    node["Height"].Value = page.Container.Height;

                if (page.Container.Last)
                    node["Last"].Value = page.Container.Last;

                if (page.Container.MarginRight > 0)
                    node["PushRight"].Value = page.Container.MarginRight;

                if (page.Container.MarginLeft > 0)
                    node["PushLeft"].Value = page.Container.MarginLeft;

                if (page.Container.MarginBottom > 0)
                    node["SpcBottom"].Value = page.Container.MarginBottom;

                if (page.Container.MarginTop > 0)
                    node["Top"].Value = page.Container.MarginTop;

                if (page.Container.Width > 0)
                    node["Width"].Value = page.Container.Width;

                node["Container"].Value = page.Container.ViewportContainer;

                RaiseEvent(
                    "Magix.Core.SetViewPortContainerSettings",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ReloadWebPart")]
        protected void Magix_Publishing_ReloadWebPart(object sender, ActiveEventArgs e)
        {
            WebPart t = WebPart.SelectByID(e.Params["PageObjectTemplateID"].Get<int>());

            Node node = new Node();

            node["Container"].Value = e.Params["Parameters"]["Container"].Value;
            node["FreezeContainer"].Value = true;
            node["ID"].Value = t.ID;

            RaiseEvent(
                "Magix.Publishing.InjectPlugin",
                node);
        }
    }
}

























