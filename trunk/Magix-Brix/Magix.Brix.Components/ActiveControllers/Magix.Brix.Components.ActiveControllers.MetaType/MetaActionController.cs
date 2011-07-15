/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    [ActiveController]
    public class MetaActionController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
            }
            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Caption"].Value = "MetaTypes";

            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Types"]["Caption"].Value = "View Objects ...";
            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Types"]["Event"]["Name"].Value = "Magix.MetaType.OpenMetaTypeDashboard";

            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Actions"]["Caption"].Value = "View Actions ...";
            e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Actions"]["Event"]["Name"].Value = "Magix.MetaType.ViewActions";
        }

        [ActiveEvent(Name = "Magix.MetaType.ViewActions")]
        protected void Magix_MetaType_ViewActions(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Description"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["EventName"]["ForcedWidth"].Value = 4;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditAction";
            node["CreateEventName"].Value = "Magix.Meta.CreateAction";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 10;
            node["Type"]["Properties"]["EventName"]["Header"].Value = "Action";
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Description"]["MaxLength"].Value = 20;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Meta.CreateAction")]
        protected void Magix_Meta_CreateAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = new Action();
                a.Name = "Default, please change";
                a.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Meta.EditAction")]
        protected void Magix_Meta_EditAction(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ID"].Get<int>());
            EditActionItem(a, e.Params);
            EditActionItemParams(a, e.Params);
        }

        private void EditActionItemParams(Action a, Node e)
        {
            Node node = new Node();
            node["PropertyName"].Value = "Params";
            node["Container"].Value = "content5";
            node["ID"].Value = a.ID;
            node["Padding"].Value = 6;
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["MarginBottom"].Value = 10;
            node["PullTop"].Value = 8;
            node["ParentFullTypeName"].Value = typeof(Action).FullName;
            node["FullTypeName"].Value = typeof(Action).FullName;
            node["ReUseNode"].Value = true;
            node["IsList"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);
        }

        private void EditActionItem(Action a, Node node)
        {
            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["StripInput"].Value = true;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Params"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["StripInput"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;

            node["Padding"].Value = 6;
            node["Width"].Value = 18;
            node["Container"].Value = "content4";
            node["MarginBottom"].Value = 10;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }
    }
}





















