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

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["ID"].Value = m.ID;
            node["MetaTypeName"].Value = m.TypeName;
            node["IsList"].Value = m.IsList;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaView.EditView",
                "content4",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangeTypeOfMetaView")]
        protected void Magix_Publishing_ChangeTypeOfMetaView(object sender, ActiveEventArgs e)
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
        }
    }
}






















