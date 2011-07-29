/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using System.Web;
using Magix.Brix.Components.ActiveTypes.Users;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class EditPagesController : ActiveController
    {
        #region [ -- ApplicationStartup, creation of some default objects -- ]

        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (WebPageTemplate.Count == 0)
                {
                    // Creating a default template ...
                    WebPageTemplate t1 = new WebPageTemplate();
                    t1.Name = "M+H+C";
                    
                    WebPartTemplate c1 = new WebPartTemplate();
                    c1.Name = "Menu";
                    c1.CssClass = "menu";
                    c1.ViewportContainer = "content1";
                    c1.Width = 6;
                    c1.Height = 20;
                    c1.MarginTop = 8;
                    c1.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.SliderMenu";
                    t1.Containers.Add(c1);

                    WebPartTemplate c2 = new WebPartTemplate();
                    c2.Name = "Header";
                    c2.CssClass = "header";
                    c2.ViewportContainer = "content2";
                    c2.Width = 18;
                    c2.Height = 7;
                    c2.Last = true;
                    c2.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Header";
                    t1.Containers.Add(c2);

                    WebPartTemplate c3 = new WebPartTemplate();
                    c3.Name = "Content";
                    c3.CssClass = "content";
                    c3.ViewportContainer = "content3";
                    c3.Width = 18;
                    c3.Height = 30;
                    c3.MarginBottom = 10;
                    c3.MarginTop = 2;
                    c3.Last = true;
                    c3.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Content";
                    t1.Containers.Add(c3);

                    t1.Save();
                }
                if (WebPage.Count == 0)
                {
                    // Creating a default page ...
                    WebPage o = new WebPage();
                    o.Name = "Default Home Page";
                    o.Template = WebPageTemplate.SelectFirst(Criteria.Eq("Name", "Menu Left"));

                    WebPart t1 = new WebPart();
                    t1.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Menu"));
                    o.WebParts.Add(t1);

                    WebPart t2 = new WebPart();
                    t2.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Header"));
                    WebPart.WebPartSetting s1 = new WebPart.WebPartSetting();
                    s1.Name = "Magix.Brix.Components.ActiveModules.Publishing.HeaderCaption";
                    s1.Value = "Welcome to Magix";
                    t2.Settings.Add(s1);
                    o.WebParts.Add(t2);

                    WebPart t3 = new WebPart();
                    t3.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Content"));
                    WebPart.WebPartSetting s2 = new WebPart.WebPartSetting();
                    s2.Name = "Magix.Brix.Components.ActiveModules.Publishing.ContentText";
                    s2.Value = "<p>By default a user has been created with the username/password of admin/admin</p>";
                    t3.Settings.Add(s2);
                    o.WebParts.Add(t3);

                    o.Save();
                }
                tr.Commit();
            }
        }

        #endregion

        [ActiveEvent(Name = "Magix.Publishing.EditPages")]
        protected void Magix_Publishing_EditPages(object sender, ActiveEventArgs e)
        {
            if (e.Params == null)
                e.Params = new Node();
            e.Params["NoClose"].Value = true;
            if (!e.Params.Contains("Width"))
                e.Params["Width"].Value = 18;
            if (!e.Params.Contains("Last"))
                e.Params["Last"].Value = true;
            if (!e.Params.Contains("Container"))
                e.Params["Container"].Value = "content3";
            if (!e.Params.Contains("CssClass"))
                e.Params["CssClass"].Value = "edit-pages-tree-view";

            RaiseEvent(
                "Magix.Publishing.GetEditPagesDataSource",
                e.Params);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.TreeViewOfPages",
                e.Params["Container"].Get<string>(),
                e.Params);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetEditPagesDataSource")]
        protected void Magix_Publishing_GetEditPagesDataSource(object sender, ActiveEventArgs e)
        {
            foreach (WebPage idx in WebPage.Select())
            {
                if (idx.Parent != null)
                    continue; // Only 'Root' objects ...

                e.Params["Pages"]["P" + idx.ID]["Caption"].Value = idx.Name;
                e.Params["Pages"]["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, e.Params["Pages"]["P" + idx.ID]["Pages"]);
            }
        }

        private void DoChildren(WebPage parent, Node node)
        {
            foreach (WebPage idx in parent.Children)
            {
                node["P" + idx.ID]["Caption"].Value = idx.Name;
                node["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, node["P" + idx.ID]["Pages"]);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangeWebPartSetting")]
        protected void Magix_Publishing_ChangeWebPartSetting(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPart.WebPartSetting s = WebPart.WebPartSetting.SelectByID(e.Params["ID"].Get<int>());

                s.Value = e.Params["Value"].Get<string>();
                s.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.EditSpecificPage")]
        protected void Magix_Publishing_EditSpecificPage(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["Top"].Value = 1;


            node["ID"].Value = p.ID;
            node["Header"].Value = p.Name;
            node["URL"].Value = p.URL;
            node["TemplateID"].Value = p.Template.ID;

            GetTemplates(node);

            GetTemplates(p, node);

            foreach (Role idx in Role.Select())
            {
                node["Roles"]["r-" + idx.ID]["Name"].Value = idx.Name;
                node["Roles"]["r-" + idx.ID]["ID"].Value = idx.ID;
            }

            RaiseEvent(
                "Magix.Publishing.GetRolesListForPage",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.EditSpecificPage",
                "content4",
                node);
        }

        private static void GetTemplates(WebPage p, Node node)
        {
            foreach (WebPart idx in p.WebParts)
            {
                foreach (WebPart.WebPartSetting idxI in idx.Settings)
                {
                    Type moduleType = Adapter.ActiveModules.Find(
                        delegate(Type idxT)
                        {
                            return idxT.FullName == idx.Container.ModuleName;
                        });
                    if (moduleType.GetProperty(
                        idxI.Name.Replace(moduleType.FullName, ""),
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.Instance) != null)
                    {
                        node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name].Value = idxI.Value;
                        PropertyInfo prop = moduleType.GetProperty(
                            idxI.Name.Replace(moduleType.FullName, ""),
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);
                        if (prop != null)
                        {
                            ModuleSettingAttribute[] atrs =
                                prop.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                                as ModuleSettingAttribute[];
                            if (atrs != null && atrs.Length > 0)
                            {
                                ModuleSettingAttribute atr = atrs[0];
                                if (!string.IsNullOrEmpty(atr.ModuleEditorName))
                                {
                                    node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name]["Editor"].Value = atr.ModuleEditorName;
                                }
                                else if (!string.IsNullOrEmpty(atr.ModuleEditorEventName))
                                {
                                    node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name]["EditorEvent"].Value = atr.ModuleEditorEventName;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetTemplates(Node node)
        {
            RaiseEvent(
                "Magix.Publishing.GetTemplates",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangePageProperty")]
        protected void Magix_Publishing_ChangePageProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params.Contains("Text"))
                    o.Name = e.Params["Text"].Get<string>();
                if (e.Params.Contains("URL"))
                    o.ChangeURL(e.Params["URL"].Get<string>());
                if (e.Params.Contains("PageTemplateID"))
                {
                    o.Template = WebPageTemplate.SelectByID(e.Params["PageTemplateID"].Get<int>());
                }
                o.Save();
                if (e.Params.Contains("PageTemplateID"))
                {
                    e.Params["PageTemplateID"].Value = o.Template.ID;
                }
                e.Params["URL"].Value = o.URL;

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.CreateChild")]
        protected void Magix_Publishing_CreateChild(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                WebPage n = new WebPage();
                n.Parent = o;
                o.Children.Add(n);

                o.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");

                Node node = new Node();
                node["ID"].Value = n.ID;

                RaiseEvent(
                    "Magix.Core.SetTreeSelectedID",
                    node);

                RaiseEvent(
                    "Magix.Publishing.EditSpecificPage",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.DeletePageObject")]
        protected void Magix_Publishing_DeletePageObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                if (o.Parent == null)
                {
                    throw new ArgumentException("You cannot delete the top most Page, only edit it ...");
                }

                o.Delete();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                Node node = new Node();
                node["ID"].Value = o.ID;
                RaiseEvent(
                    "Magix.Publishing.PageWasDeleted", 
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangeModuleSetting")]
        protected void Magix_Publishing_ChangeModuleSetting(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPartTemplate t = WebPartTemplate.SelectByID(e.Params["ID"].Get<int>());
                string propertyName = e.Params["PropertyName"].Get<string>();

                WebPage o = WebPage.SelectByID(e.Params["PageObjectID"].Get<int>());

                WebPart ot = o.WebParts.Find(
                    delegate(WebPart idx)
                    {
                        return idx.Container == t;
                    });

                WebPart.WebPartSetting set = ot.Settings.Find(
                    delegate(WebPart.WebPartSetting idx)
                    {
                        return propertyName == idx.Name;
                    });
                set.Value = e.Params["Value"].Get<string>();

                set.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.SavePageObjectIDSetting")]
        private void Magix_Publishing_SavePageObjectIDSetting(object sender, ActiveEventArgs e)
        {
            int objId = e.Params["Params"]["ID"].Get<int>();
            string propertyName = e.Params["Params"]["PropertyName"].Get<string>();
            int potId = e.Params["Params"]["PotID"].Get<int>();

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage po = WebPage.SelectByID(objId);
                WebPart pot = po.WebParts.Find(
                    delegate(WebPart idx)
                    {
                        return idx.Container.ID == potId;
                    });
                WebPart.WebPartSetting set = pot.Settings.Find(
                    delegate(WebPart.WebPartSetting idx)
                    {
                        return idx.Name == propertyName;
                    });
                set.Value = e.Params["Text"].Get<string>();
                set.Save();
                tr.Commit();
            }

            Node node = new Node();

            node["ID"].Value = objId;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Publishing.EditSpecificPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.WebPartTemplateWasModified")]
        protected void Magix_Publishing_WebPartTemplateWasModified(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // We must 're-touch' all Pages effected by the change of our template
                // This to create default values for newly inserted type of
                // modules and such ...
                List<WebPage> pages = new List<WebPage>();
                foreach (WebPart idx in
                    WebPart.Select(
                        Criteria.ExistsIn(e.Params["ID"].Get<int>(), false)))
                {
                    if (!pages.Exists(
                        delegate(WebPage idx2)
                        {
                            return idx.WebPage == idx2;
                        }))
                        pages.Add(idx.WebPage);
                }
                foreach (WebPage idx in pages)
                {
                    idx.Save();
                }

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.WebPageTemplateWasModified")]
        protected void Magix_Publishing_WebPageTemplateWasModified(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // We must 're-touch' all Pages effected by the change of our template
                // This to create default values for newly inserted type of
                // modules and such ...
                foreach (WebPage idx in 
                    WebPage.Select(
                        Criteria.ExistsIn(e.Params["ID"].Get<int>(), true)))
                {
                    idx.Save();
                }

                tr.Commit();
            }
        }
    }
}























