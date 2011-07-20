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
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Caption"].Value = "View Views ...";
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Event"]["Name"].Value = "Magix.MetaView.ViewMetaViews";
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
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 6;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["TypeName"]["MaxLength"].Value = 50;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaView.EditMetaView";
            node["CreateEventName"].Value = "Magix.MetaView.CreateMetaView";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type";

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
                view.Name = "Default name";

                view.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = view.ID;
                RaiseEvent(
                    "Magix.MetaView.EditMetaView",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.EditMetaView")]
        protected void Magix_MetaView_EditMetaView(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["ID"].Value = m.ID;

            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["Properties"].Value = true;
            node["WhiteListColumns"]["Created"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Created"]["Header"].Value = "When";
            node["Type"]["Properties"]["Properties"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Properties"]["Header"].Value = "Props";

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["Container"].Value = "content4";
            node["MarginBottom"].Value = 10;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
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

        [ActiveEvent(Name = "Magix.MetaView.GetViewData")]
        protected void Magix_MetaView_GetViewData(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            e.Params["MetaViewTypeName"].Value = m.TypeName;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                e.Params["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                e.Params["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                e.Params["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                e.Params["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaViewCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["Header"].Value = "Views";
            e.Params["Object"]["Properties"]["MetaViewCount"].Value = MetaView.Count.ToString();
        }
    }
}






















