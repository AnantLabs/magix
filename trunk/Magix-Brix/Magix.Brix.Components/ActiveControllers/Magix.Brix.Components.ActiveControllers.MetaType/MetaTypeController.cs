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
    public class MetaTypeController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("MetaType"))
            {
                e.Params["Items"]["Admin"]["Items"]["MetaType"]["Caption"].Value = "MetaTypes";
                e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Types"]["Caption"].Value = "View Objects ...";
                e.Params["Items"]["Admin"]["Items"]["MetaType"]["Items"]["Types"]["Event"]["Name"].Value = "Magix.MetaType.OpenMetaTypeDashboard";
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.OpenMetaTypeDashboard")]
        protected void Magix_MetaType_OpenMetaTypeDashboard(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaType).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Reference"].Value = true;
            node["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Created"].Value = true;
            node["WhiteListColumns"]["Created"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Values"].Value = true;
            node["WhiteListColumns"]["Values"]["ForcedWidth"].Value = 2;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaType.EditType";

            node["ReuseNode"].Value = true;
            node["CreateEventName"].Value = "Magix.MetaType.CreateType";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Values"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Values"]["NoFilter"].Value = true;

            node["Container"].Value = "content3";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.GetTypes")]
        protected void Magix_MetaType_GetTypes(object sender, ActiveEventArgs e)
        {
            // TODO: Populate with statistics and sch to show in dashboard of MetaModules project ....
        }

        [ActiveEvent(Name = "Magix.MetaType.CreateType")]
        protected void Magix_MetaType_CreateType(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType m = new MetaType();
                m.Name = "Default name, Please Change ...";
                m.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = m.ID;
                RaiseEvent(
                    "Magix.MetaType.EditType",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.GetTypeDeclaration")]
        protected void Magix_MetaType_GetTypeDeclaration(object sender, ActiveEventArgs e)
        {
            MetaType t = MetaType.SelectByID(e.Params["ID"].Get<int>());

            e.Params["ID"].Value = t.ID;
            e.Params["Name"].Value = t.Name;

            foreach (MetaType.Value idx in t.Values)
            {
                e.Params["Values"]["v-" + idx.ID]["ID"].Value = idx.ID;
                e.Params["Values"]["v-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Values"]["v-" + idx.ID]["Val"].Value = idx.Val;
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.EditType")]
        protected void Magix_MetaType_EditType(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["MarginBottom"].Value = 10;

            node["ID"].Value = e.Params["ID"].Value;

            RaiseEvent(
                "Magix.MetaType.GetTypeDeclaration",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaTypes.EditType",
                "content4",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.CreateValue")]
        protected void Magix_MetaType_CreateValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType t = MetaType.SelectByID(e.Params["ID"].Get<int>());

                MetaType.Value val = new MetaType.Value();
                val.Name = "Name";
                val.Val = "Default, please change ...";

                t.Values.Add(val);
                t.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(MetaType).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.ChangeName")]
        protected void Magix_MetaType_ChangeName(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType.Value val = MetaType.Value.SelectByID(e.Params["ID"].Get<int>());
                val.Name = e.Params["Name"].Get<string>();
                val.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.ChangeValue")]
        protected void Magix_MetaType_ChangeValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType.Value val = MetaType.Value.SelectByID(e.Params["ID"].Get<int>());
                val.Val = e.Params["Value"].Get<string>();
                val.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(MetaType).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.ChangeNameOfMetaType")]
        protected void Magix_MetaType_ChangeNameOfMetaType(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType t = MetaType.SelectByID(e.Params["ID"].Get<int>());
                t.Name = e.Params["Name"].Get<string>();

                t.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(MetaType).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.DeleteValue")]
        protected void Magix_MetaType_DeleteValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType.Value t = MetaType.Value.SelectByID(e.Params["ID"].Get<int>());

                t.Delete();

                tr.Commit();
            }

            Node node = new Node();
            node["ID"].Value = e.Params["ParentID"].Get<int>();
            RaiseEvent(
                "Magix.MetaType.EditType",
                node);

            node = new Node();
            node["FullTypeName"].Value = typeof(MetaType).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaType).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        [ActiveEvent(Name = "Magix.Meta.FindAction")]
        protected void Magix_Meta_FindAction(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "content5";
            node["Width"].Value = 21;
            node["Last"].Value = true;
            node["Padding"].Value = 3;
            node["MarginBottom"].Value = 10;
            node["PullTop"].Value = 8;
            node["IsDelete"].Value = false;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["EventName"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Description"]["ForcedWidth"].Value = 8;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Select";
            node["IDColumnValue"].Value = "Select";
            node["IDColumnEvent"].Value = "Magix.Meta.SelectAction";
            node["CreateEventName"].Value = "Magix.Meta.CreateAction";
            node["MetaTypeID"].Value = e.Params["ID"].Get<int>();

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["Header"].Value = "Action";
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = false;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Meta.SelectAction")]
        protected void Magix_Meta_AppendAction(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ID"].Get<int>());
            e.Params["Action"].Value = a.EventName;

            RaiseEvent(
                "Magix.Meta.AppendAction",
                e.Params);
        }
    }
}





















