/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaViews;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveControllers.MetaViews
{
    [ActiveController]
    public class MetaViewController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Views"]["Caption"].Value = "View Views ...";
            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Views"]["Event"]["Name"].Value = "Magix.MetaView.ViewMetaViews";
        }

        [ActiveEvent(Name = "Magix.MetaView.ViewMetaViews")]
        protected void Magix_MetaView_ViewMetaViews(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["IsList"].Value = true;
            node["WhiteListColumns"]["IsList"]["ForcedWidth"].Value = 2;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaView.EditMetaView";
            node["CreateEventName"].Value = "Magix.MetaView.CreateMetaView";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["IsList"]["ReadOnly"].Value = true;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.CreateMetaView")]
        protected void Magix_MetaView_CreateMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView view = new MetaView();
                view.Name = "Default name, Please Change ...";

                view.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.EditMetaView")]
        protected void Magix_MetaView_EditMetaView(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 21;
            node["Down"].Value = 2;
            node["Last"].Value = true;
            node["Padding"].Value = 3;
            node["ID"].Value = m.ID;
            node["MetaTypeName"].Value = m.TypeName;
            node["IsList"].Value = m.IsList;
            node["HasSearch"].Value = m.HasSearch;
            node["Caption"].Value = m.Caption;
            node["MarginBottom"].Value = 10;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                node["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                node["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                node["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                node["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                node["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaView.EditView",
                "content4",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangeTypeOfMetaView")]
        protected void Magix_MetaView_ChangeTypeOfMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params.Contains("IsList"))
                    m.IsList = e.Params["IsList"].Get<bool>();

                if (e.Params.Contains("MetaTypeName"))
                    m.TypeName = e.Params["MetaTypeName"].Get<string>();

                m.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(MetaView).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.CreateProperty")]
        protected void Magix_MetaView_CreateProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

                MetaView.MetaViewProperty t = new MetaView.MetaViewProperty();
                t.Name = "Default Property Name";
                m.Properties.Add(t);

                m.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(MetaView).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.DeleteProperty")]
        protected void Magix_MetaView_DeleteProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());

                p.Delete();

                tr.Commit();
            }

            Node node = new Node();
            node["ID"].Value = e.Params["ParentID"].Get<int>();

            RaiseEvent(
                "Magix.MetaView.EditMetaView",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangePropertyName")]
        protected void Magix_MetaView_ChangePropertyName(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());
                p.Name = e.Params["Name"].Get<string>();

                p.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangePropertyReadOnly")]
        protected void Magix_MetaView_ChangePropertyReadOnly(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());
                p.ReadOnly = e.Params["ReadOnly"].Get<bool>();

                p.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangePropertyAction")]
        protected void Magix_MetaView_ChangePropertyAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());
                p.Action = e.Params["MetaViewAction"].Get<string>();

                p.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.SetSearch")]
        protected void Magix_MetaView_SetSearch(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());
                m.HasSearch = e.Params["HasSearch"].Get<bool>();

                m.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangeCaptionOfMetaView")]
        protected void Magix_MetaView_ChangeCaptionOfMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());
                m.Caption = e.Params["MetaViewCaption"].Get<string>();

                m.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.ChangePropertyDescription")]
        protected void Magix_MetaView_ChangePropertyDescription(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty m = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());
                m.Description = e.Params["MetaViewDescription"].Get<string>();

                m.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
                ActiveEvents.Instance.RaiseClearControls("content4");
        }

        [ActiveEvent(Name = "Magix.MetaView.LoadWysiwyg")]
        protected void Magix_MetaView_LoadWysiwyg(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Padding"].Value = 6;
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Top"].Value = 2;
            node["MarginBottom"].Value = 10;
            node["PullTop"].Value = 8;
            node["CssClass"].Value = "yellow-background";
            node["MetaViewTypeName"].Value = m.TypeName;
            node["MetaViewName"].Value = m.Name;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                node["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                node["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                node["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                node["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                node["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaView.SingleView",
                "content5",
                node);
        }
    }
}






















