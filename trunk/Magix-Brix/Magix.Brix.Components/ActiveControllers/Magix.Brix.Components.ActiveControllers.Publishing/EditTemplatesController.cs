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
    public class EditTemplatesController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.EditTemplates")]
        protected void Magix_Publishing_EditTemplates(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(PageTemplate).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "large-bottom-margin";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Containers"].Value = true;
            node["WhiteListColumns"]["Containers"]["ForcedWidth"].Value = 4;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Publishing.EditTemplate";
            node["CreateEventName"].Value = "Magix.Publishing.CreateTemplate";

            node["ReuseNode"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Containers"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Containers"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Containers"]["TemplateColumnEvent"].Value = "Magix.Publisher.GetContainersTemplateColumn";

            node["Container"].Value = "content3";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.CreateTemplate")]
        protected void Magix_Publishing_CreateTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                PageTemplate p = new PageTemplate();
                p.Name = "Template...";

                PageTemplateContainer c = new PageTemplateContainer();
                c.Width = 6;
                c.Height = 6;
                c.Name = "Name";
                c.ModuleName = Adapter.ActiveModules.Find(
                    delegate(Type idx)
                    {
                        PublisherPluginAttribute[] atrs = idx.GetCustomAttributes(typeof(PublisherPluginAttribute), true) as PublisherPluginAttribute[];
                        return atrs != null && atrs.Length > 0;
                    }).FullName;
                c.ViewportContainer = "content1";
                p.Containers.Add(c);
                c.PageTemplate = p;

                p.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.EditTemplate")]
        protected void Magix_Publishing_EditTemplate(object sender, ActiveEventArgs e)
        {
            PageTemplate t = PageTemplate.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            // Populating with Templates ...
            RaiseEvent(
                "Magix.Publishing.GetTemplates",
                node);

            node["ID"].Value = t.ID;
            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["CssClass"].Value = "yellow-background";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.EditSpecificTemplate",
                "content4",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetTemplates")]
        protected void Magix_Publishing_GetTemplates(object sender, ActiveEventArgs e)
        {
            foreach (PageTemplate idx in PageTemplate.Select())
            {
                e.Params["Templates"]["t-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Templates"]["t-" + idx.ID]["ID"].Value = idx.ID;

                Node tmp = e.Params["Templates"]["t-" + idx.ID]["Containers"];
                foreach (PageTemplateContainer idxC in idx.Containers)
                {
                    tmp["i-" + idxC.ID]["ID"].Value = idxC.ID;
                    tmp["i-" + idxC.ID]["Name"].Value = idxC.Name;
                    tmp["i-" + idxC.ID]["Height"].Value = idxC.Height;
                    tmp["i-" + idxC.ID]["Last"].Value = idxC.Last;
                    tmp["i-" + idxC.ID]["Padding"].Value = idxC.Padding;
                    tmp["i-" + idxC.ID]["Top"].Value = idxC.Top;
                    tmp["i-" + idxC.ID]["Width"].Value = idxC.Width;
                    tmp["i-" + idxC.ID]["Push"].Value = idxC.Push;
                    tmp["i-" + idxC.ID]["BottomMargin"].Value = idxC.BottomMargin;
                    tmp["i-" + idxC.ID]["ModuleName"].Value = idxC.ModuleName;
                    tmp["i-" + idxC.ID]["CssClass"].Value = idxC.CssClass;
                }
            }
            foreach (Type idx in Adapter.ActiveModules)
            {
                PublisherPluginAttribute[] atrs =
                    idx.GetCustomAttributes(typeof(PublisherPluginAttribute), true)
                        as PublisherPluginAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    e.Params["AllModules"][idx.FullName]["ModuleName"].Value = idx.FullName;
                    e.Params["AllModules"][idx.FullName]["ShortName"].Value = idx.Name;
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangeModuleForTemplate")]
        protected void Magix_Publishing_ChangeModuleForTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                PageTemplateContainer t = PageTemplateContainer.SelectByID(e.Params["ID"].Get<int>());
                t.ModuleName = e.Params["ModuleName"].Get<string>();
                t.Save();
                foreach (PageObject idx in PageObject.Select(
                        Criteria.ExistsIn(t.PageTemplate.ID, true)))
                {
                    // Forcing a 'rethink' of PageObjects beloning to this template
                    // to force [among other things] Defaut settings and such into the
                    // objects, and have them correctly numbered and aligned and such ...
                    idx.Save();
                }
                tr.Commit();

                Node node = new Node();
                node["ID"].Value = t.ID;

                RaiseEvent(
                    "Magix.Publishing.TemplateWasModified",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangeTemplateProperty")]
        protected void Magix_Publishing_ChangeTemplateProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                PageTemplateContainer t = PageTemplateContainer.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params["Action"].Get<string>() == "IncreaseWidth")
                    t.Width = Math.Min(24, t.Width + 1);
                else if (e.Params["Action"].Get<string>() == "DecreaseWidth")
                    t.Width = Math.Max(6, t.Width - 1);
                else if (e.Params["Action"].Get<string>() == "IncreaseHeight")
                    t.Height = Math.Min(30, t.Height + 1);
                else if (e.Params["Action"].Get<string>() == "DecreaseHeight")
                    t.Height = Math.Max(6, t.Height - 1);
                else if (e.Params["Action"].Get<string>() == "IncreaseDown")
                    t.Top = Math.Min(30, t.Top + 1);
                else if (e.Params["Action"].Get<string>() == "DecreaseDown")
                    t.Top = Math.Max(0, t.Top - 1);
                else if (e.Params["Action"].Get<string>() == "IncreaseBottom")
                    t.BottomMargin = Math.Min(30, t.BottomMargin + 1);
                else if (e.Params["Action"].Get<string>() == "DecreaseBottom")
                    t.BottomMargin = Math.Max(0, t.BottomMargin - 1);
                else if (e.Params["Action"].Get<string>() == "IncreaseLeft")
                    t.Push = Math.Min(18, t.Push + 1);
                else if (e.Params["Action"].Get<string>() == "DecreaseLeft")
                    t.Push = Math.Max(0, t.Push - 1);
                else if (e.Params["Action"].Get<string>() == "IncreasePadding")
                    t.Padding = Math.Min(18, t.Padding + 1);
                else if (e.Params["Action"].Get<string>() == "DecreasePadding")
                    t.Padding = Math.Max(0, t.Padding - 1);
                else if (e.Params["Action"].Get<string>() == "ChangeName")
                    t.Name = e.Params["Action"]["Value"].Get<string>();
                else if (e.Params["Action"].Get<string>() == "ChangeCssClass")
                    t.CssClass = e.Params["Value"].Get<string>();
                else if (e.Params["Action"].Get<string>() == "ChangeLast")
                    t.Last = e.Params["Value"].Get<bool>();
                t.Save();
                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Publisher.GetContainersTemplateColumn")]
        protected void Magix_Publisher_GetContainersTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            int id = e.Params["ID"].Get<int>();

            PageTemplate pt = PageTemplate.SelectByID(id);

            InPlaceTextAreaEdit txt = new InPlaceTextAreaEdit();
            txt.Text = pt.Containers.Count.ToString();
            txt.Info = id.ToString();
            txt.TextChanged +=
                delegate(object sender2, EventArgs e2)
                {
                    InPlaceTextAreaEdit t2 = sender2 as InPlaceTextAreaEdit;
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        PageTemplate t = PageTemplate.SelectByID(int.Parse(t2.Info));
                        int countInt = int.Parse(t2.Text);
                        if (countInt > 7 || countInt < 1)
                            throw new ArgumentException("Between 1 and 7 please ...");
                        int count = Math.Max(1, Math.Min(7, countInt));
                        if (t.Containers.Count < count)
                        {
                            while (t.Containers.Count < count)
                            {
                                PageTemplateContainer c = new PageTemplateContainer();
                                c.Name = "Default Name";
                                c.Width = 6;
                                c.Height = 6;
                                c.ViewportContainer = "content" + (t.Containers.Count + 1);
                                c.ModuleName = Adapter.ActiveModules.Find(
                                    delegate(Type idx)
                                    {
                                        PublisherPluginAttribute[] atrs = idx.GetCustomAttributes(typeof(PublisherPluginAttribute), true) as PublisherPluginAttribute[];
                                        return atrs != null && atrs.Length > 0;
                                    }).FullName;
                                t.Containers.Add(c);
                            }
                        }
                        else if (t.Containers.Count > count)
                        {
                            while (t.Containers.Count - count > 0)
                            {
                                t.Containers.RemoveAt(t.Containers.Count - 1);
                            }
                        }
                        t.Save();
                        tr.Commit();

                        Node node = new Node();
                        node["ID"].Value = t.ID;

                        RaiseEvent(
                            "Magix.Publishing.TemplateWasModified", 
                            node);
                    }

                };
            e.Params["Control"].Value = txt;
        }
    }
}























