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
using System.Globalization;
using System.Text.RegularExpressions;

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
                            object nValue = Convert.ChangeType(idxSet.Value, idx.PropertyType, CultureInfo.InvariantCulture);
                            idx.GetSetMethod(true).Invoke(e.Params["_ctrl"].Value, new object[] { nValue });
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

            int cl = 0;
            foreach (WebPart idx in page.WebParts)
            {
                if (idx.Container.ViewportContainer.CompareTo(lastModule) > 0)
                    lastModule = idx.Container.ViewportContainer;

                Node tmp = new Node();

                tmp["ID"].Value = idx.ID;

                RaiseEvent(
                    "Magix.Publishing.InjectPlugin",
                    tmp);

                cl = int.Parse(lastModule.Replace("content", ""));
            }
            if (cl < 7)
                ActiveEvents.Instance.RaiseClearControls("content" + (cl + 1));
        }

        private void SetCaptionOfPage(WebPage page)
        {
            Node node = new Node();

            node["Caption"].Value = page.Name;

            RaiseEvent(
                "Magix.Core.SetTitleOfPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetWebPartValue")]
        private void Magix_Publishing_GetWebPartValue(object sender, ActiveEventArgs e)
        {
            WebPart part = WebPart.SelectByID(e.Params["WebPartID"].Get<int>());

            foreach (WebPart.WebPartSetting idx in part.Settings)
            {
                if (idx.Name == part.Container.ModuleName + e.Params["Name"].Get<string>())
                {
                    e.Params["Value"].Value = idx.Value;
                    break;
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.GetChildExcerptAdditionalControls")]
        private void Magix_Publishing_GetChildExcerptAdditionalControls(object sender, ActiveEventArgs e)
        {
            WebPage wp = WebPage.SelectByID(e.Params["ID"].Get<int>());

            foreach (WebPart idx in wp.WebParts)
            {
                string moduleName = idx.Container.ModuleName;
                switch (moduleName)
                {
                    case "Magix.Brix.Components.ActiveModules.Publishing.Content":
                        {
                            Label lbl = new Label();
                            lbl.Tag = "p";
                            lbl.CssClass = "excerpt-excerpt";
                            string text = idx.Settings.Find(
                                delegate(WebPart.WebPartSetting idxI)
                                {
                                    return idxI.Name == moduleName + "Text";
                                }).Value;
                            text = Regex.Replace(text, "<(.|\n)*?>", string.Empty);
                            if (text.Length > 150)
                                text = text.Substring(0, 147) + "...";
                            lbl.Text = text;
                            e.Params["Controls"]["Magix.Brix.Components.ActiveModules.Publishing.Content"].Value = lbl;
                        } break;
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.BuildOneChildExcerptControl")]
        private void Magix_Publishing_BuildOneChildExcerptControl(object sender, ActiveEventArgs e)
        {
            WebPage wp = WebPage.SelectByID(e.Params["ID"].Get<int>());

            Panel pnl = new Panel();
            pnl.CssClass = "excerpt-item";

            Label lb = new Label();
            lb.Tag = "h3";
            lb.Text = wp.Name;
            pnl.Controls.Add(lb);

            Node node = new Node();
            node["ID"].Value = wp.ID;

            RaiseEvent(
                "Magix.Publishing.GetChildExcerptAdditionalControls",
                node);
            if (node.Contains("Controls"))
            {
                foreach (Node idx in node["Controls"])
                {
                    pnl.Controls.Add(idx.Value as System.Web.UI.Control);
                }
            }

            e.Params["Control"].Value = pnl;
        }

        [ActiveEvent(Name = "Magix.Publishing.GetLastChildrenPages")]
        private void Magix_Publishing_GetLastChildrenPages(object sender, ActiveEventArgs e)
        {
            WebPart part = WebPart.SelectByID(e.Params["ID"].Get<int>());

            WebPage page = part.WebPage;

            // Doing it the hard way, in case there are 'millions' of pages .......
            foreach (WebPage idx in WebPage.Select(
                Criteria.ParentId(page.ID),
                Criteria.Range(0, 10, "Created", false)))
            {
                e.Params["Items"]["i" + idx.ID]["ID"].Value = idx.ID;
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.InjectPlugin")]
        private void Magix_Publishing_InjectPlugin(object sender, ActiveEventArgs e)
        {
            WebPart page = WebPart.SelectByID(e.Params["ID"].Get<int>());

            Node ch = new Node();

            ch["ModuleName"].Value = page.Container.ModuleName;
            ch["Container"].Value = page.Container.ViewportContainer;
            ch["WebPartID"].Value = page.ID;

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

                node["CssClass"].Value = "web-part" + " " + page.Container.CssClass;

                if (page.Container.Overflow)
                    node["OverflowWebPart"].Value = true;

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

                string cssClass = "web-part";
                if (page.Container.Overflow)
                    cssClass += " web-part-overflow";
                cssClass += " " + page.Container.CssClass;
                node["CssClass"].Value = cssClass;

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

























