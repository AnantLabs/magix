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
            PageObjectTemplate t = PageObjectTemplate.SelectByID(e.Params["ID"].Get<int>());
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
                    foreach (PageObjectTemplate.PageObjectTemplateSetting idxSet in t.Settings)
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

            PageObject p = PageObject.SelectFirst(Criteria.Eq("URL", url));

            Node node = new Node();
            node["ID"].Value = p.ID;

            RaiseEvent(
                "Magix.Publishing.OpenPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.OpenPage")]
        protected void Magix_Publishing_OpenPage(object sender, ActiveEventArgs e)
        {
            PageObject p = PageObject.SelectByID(e.Params["ID"].Get<int>());

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
                    throw new ArgumentException("You don't have access to this page ...");
                }
                else
                {
                    string lastModule = "content1";

                    foreach (PageObjectTemplate idx in p.ObjectTemplates)
                    {
                        if (idx.Container.ViewportContainer.CompareTo(lastModule) > 0)
                            lastModule = idx.Container.ViewportContainer;

                        // Checking to see if we're supposed to 'fall through'
                        // Some modules [e.g. Menu] doesn't need re-initialization
                        // upon loadups ...

                        Node ch = new Node();

                        ch["ModuleName"].Value = idx.Container.ModuleName;

                        RaiseEvent(
                            "Magix.Publishing.ShouldReloadWebPart",
                            ch);

                        if (!ch.Contains("Stop") || !ch["Stop"].Get<bool>())
                        {
                            Node node = new Node();

                            if (idx.Container.BottomMargin > 0)
                                node["BottomMargin"].Value = idx.Container.BottomMargin;
                            if (!string.IsNullOrEmpty(idx.Container.CssClass))
                                node["CssClass"].Value = idx.Container.CssClass;
                            if (idx.Container.Height > 0)
                                node["Height"].Value = idx.Container.Height;
                            if (idx.Container.Last)
                                node["Last"].Value = idx.Container.Last;
                            if (idx.Container.Padding > 0)
                                node["PushRight"].Value = idx.Container.Padding;
                            if (idx.Container.Push > 0)
                                node["PushLeft"].Value = idx.Container.Push;
                            if (idx.Container.BottomMargin > 0)
                                node["SpcBottom"].Value = idx.Container.BottomMargin;
                            if (idx.Container.Top > 0)
                                node["Top"].Value = idx.Container.Top;
                            if (idx.Container.Width > 0)
                                node["Width"].Value = idx.Container.Width;
                            node["ID"].Value = idx.ID;
                            node["ModuleInitializationEvent"].Value = "Magix.Publishing.InitializePublishingPlugin";

                            LoadModule(
                                idx.Container.ModuleName,
                                idx.Container.ViewportContainer,
                                node);
                        }
                        int cl = int.Parse(lastModule.Replace("content", ""));
                        if (cl < 7)
                            ActiveEvents.Instance.RaiseClearControls("content" + (cl + 1));
                    }
                }
            }
        }
    }
}

























