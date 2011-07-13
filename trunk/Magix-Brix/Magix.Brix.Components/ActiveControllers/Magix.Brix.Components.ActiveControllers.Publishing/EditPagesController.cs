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
                if (PageTemplate.Count == 0)
                {
                    // Creating a default template ...
                    PageTemplate t1 = new PageTemplate();
                    t1.Name = "Menu Left";
                    
                    PageTemplateContainer c1 = new PageTemplateContainer();
                    c1.Name = "Menu";
                    c1.ViewportContainer = "content1";
                    c1.Width = 6;
                    c1.Height = 20;
                    c1.Top = 8;
                    c1.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.SliderMenu";
                    t1.Containers.Add(c1);

                    PageTemplateContainer c2 = new PageTemplateContainer();
                    c2.Name = "Header";
                    c2.ViewportContainer = "content2";
                    c2.Width = 18;
                    c2.Height = 6;
                    c2.Last = true;
                    c2.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Header";
                    t1.Containers.Add(c2);

                    PageTemplateContainer c3 = new PageTemplateContainer();
                    c3.Name = "Content";
                    c3.ViewportContainer = "content3";
                    c3.Width = 18;
                    c3.Height = 30;
                    c3.BottomMargin = 10;
                    c3.Top = 2;
                    c3.Last = true;
                    c3.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Content";
                    t1.Containers.Add(c3);

                    t1.Save();
                }
                if (PageObject.Count == 0)
                {
                    // Creating a default page ...
                    PageObject o = new PageObject();
                    o.Name = "Defaut Home Page";
                    o.Template = PageTemplate.SelectFirst(Criteria.Eq("Name", "Menu Left"));

                    PageObjectTemplate t1 = new PageObjectTemplate();
                    t1.Container = PageTemplateContainer.SelectFirst(Criteria.Eq("Name", "Menu"));
                    o.ObjectTemplates.Add(t1);

                    PageObjectTemplate t2 = new PageObjectTemplate();
                    t2.Container = PageTemplateContainer.SelectFirst(Criteria.Eq("Name", "Header"));
                    PageObjectTemplate.PageObjectTemplateSetting s1 = new PageObjectTemplate.PageObjectTemplateSetting();
                    s1.Name = "Magix.Brix.Components.ActiveModules.Publishing.HeaderCaption";
                    s1.Value = "Welcome to Magix";
                    t2.Settings.Add(s1);
                    o.ObjectTemplates.Add(t2);

                    PageObjectTemplate t3 = new PageObjectTemplate();
                    t3.Container = PageTemplateContainer.SelectFirst(Criteria.Eq("Name", "Content"));
                    PageObjectTemplate.PageObjectTemplateSetting s2 = new PageObjectTemplate.PageObjectTemplateSetting();
                    s2.Name = "Magix.Brix.Components.ActiveModules.Publishing.ContentText";
                    s2.Value = "<p>Hello there world ...</p>";
                    t3.Settings.Add(s2);
                    o.ObjectTemplates.Add(t3);

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
            foreach (PageObject idx in PageObject.Select())
            {
                if (idx.Parent != null)
                    continue; // Only 'Root' objects ...

                e.Params["Pages"]["P" + idx.ID]["Caption"].Value = idx.Name;
                e.Params["Pages"]["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, e.Params["Pages"]["P" + idx.ID]["Pages"]);
            }
        }

        private void DoChildren(PageObject parent, Node node)
        {
            foreach (PageObject idx in parent.Children)
            {
                node["P" + idx.ID]["Caption"].Value = idx.Name;
                node["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, node["P" + idx.ID]["Pages"]);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.EditSpecificPage")]
        protected void Magix_Publishing_EditSpecificPage(object sender, ActiveEventArgs e)
        {
            PageObject p = PageObject.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["Top"].Value = 2;

            node["ID"].Value = p.ID;
            node["Header"].Value = p.Name;
            node["URL"].Value = p.URL;
            node["TemplateID"].Value = p.Template.ID;

            GetTemplates(node);

            foreach (PageObjectTemplate idx in p.ObjectTemplates)
            {
                foreach (PageObjectTemplate.PageObjectTemplateSetting idxI in idx.Settings)
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
                            }
                        }
                    }
                }
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.EditSpecificPage",
                "content4",
                node);
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
                PageObject o = PageObject.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params.Contains("Text"))
                    o.Name = e.Params["Text"].Get<string>();
                if (e.Params.Contains("URL"))
                    o.SetURL(e.Params["URL"].Get<string>());
                if (e.Params.Contains("PageTemplateID"))
                {
                    o.Template = PageTemplate.SelectByID(e.Params["PageTemplateID"].Get<int>());
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
                PageObject o = PageObject.SelectByID(e.Params["ID"].Get<int>());

                PageObject n = new PageObject();
                n.Parent = o;
                o.Children.Add(n);

                o.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.DeletePageObject")]
        protected void Magix_Publishing_DeletePageObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                PageObject o = PageObject.SelectByID(e.Params["ID"].Get<int>());

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
                PageTemplateContainer t = PageTemplateContainer.SelectByID(e.Params["ID"].Get<int>());
                string propertyName = e.Params["PropertyName"].Get<string>();

                PageObject o = PageObject.SelectByID(e.Params["PageObjectID"].Get<int>());

                PageObjectTemplate ot = o.ObjectTemplates.Find(
                    delegate(PageObjectTemplate idx)
                    {
                        return idx.Container == t;
                    });

                PageObjectTemplate.PageObjectTemplateSetting set = ot.Settings.Find(
                    delegate(PageObjectTemplate.PageObjectTemplateSetting idx)
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
                PageObject po = PageObject.SelectByID(objId);
                PageObjectTemplate pot = po.ObjectTemplates.Find(
                    delegate(PageObjectTemplate idx)
                    {
                        return idx.Container.ID == potId;
                    });
                PageObjectTemplate.PageObjectTemplateSetting set = pot.Settings.Find(
                    delegate(PageObjectTemplate.PageObjectTemplateSetting idx)
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

        [ActiveEvent(Name = "Magix.Publishing.TemplateWasModified")]
        protected void Magix_Publishing_TemplateWasModified(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                foreach (PageObject idx in 
                    PageObject.Select(
                        Criteria.ExistsIn(e.Params["ID"].Get<int>(), true)))
                {
                    idx.Save();
                }
                tr.Commit();
            }
        }
    }
}























