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
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    [ActiveController]
    public class MetaTypeController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Caption"].Value = "MetaTypes";
            e.Params["Items"]["MetaType"]["Items"]["Types"]["Caption"].Value = "View Objects ...";
            e.Params["Items"]["MetaType"]["Items"]["Types"]["Event"]["Name"].Value = "Magix.MetaType.ViewMetaObjectsRaw";
        }

        [ActiveEvent(Name = "Magix.MetaType.ViewMetaObjectsRaw")]
        protected void Magix_MetaType_ViewMetaObjectsRaw(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (e.Params != null && 
                e.Params.Contains("ReUseNodeX") &&
                e.Params["ReUseNodeX"].Get<bool>())
            {
                node = e.Params;
            }

            node["FullTypeName"].Value = typeof(MetaObject).FullName;
            if (!node.Contains("Container"))
                node["Container"].Value = "content3";
            if (!node.Contains("Width"))
                node["Width"].Value = 18;
            if (!node.Contains("Last"))
                node["Last"].Value = true;

            if (!node.Contains("WhiteListColumns"))
            {
                node["WhiteListColumns"]["Name"].Value = true;
                node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
                node["WhiteListColumns"]["Reference"].Value = true;
                node["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 3;
                node["WhiteListColumns"]["Created"].Value = true;
                node["WhiteListColumns"]["Created"]["ForcedWidth"].Value = 3;
                node["WhiteListColumns"]["Values"].Value = true;
                node["WhiteListColumns"]["Values"]["ForcedWidth"].Value = 2;
                node["WhiteListColumns"]["Copy"].Value = true;
                node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 2;
            }

            if (!node.Contains("FilterOnId"))
            {
                node["FilterOnId"].Value = false;
                node["IDColumnName"].Value = "Edit";
                node["IDColumnEvent"].Value = "Magix.MetaType.EditObjectRaw";
                node["DeleteColumnEvent"].Value = "Magix.MetaType.DeleteObjectRaw";

                node["ReuseNode"].Value = true;
                node["CreateEventName"].Value = "Magix.MetaType.CreateObject";
            }

            if (!node.Contains("Type"))
            {
                node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["Created"]["NoFilter"].Value = true;
                node["Type"]["Properties"]["Values"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["Values"]["NoFilter"].Value = true;
                node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
                node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.MetaType.GetCopyMetaTypeTemplateColumn";
            }
            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.GetCopyMetaTypeTemplateColumn")]
        protected void Magix_MetaType_GetCopyMetaTypeTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Fetching specific user
            // Creating our SelectList
            LinkButton ls = new LinkButton();
            ls.Text = "Copy";
            ls.Click +=
                delegate
                {
                    CopyMetaObject(id);
                };


            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        private void CopyMetaObject(int id)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject n = MetaObject.SelectByID(id).Clone();
                n.Save();

                tr.Commit();

                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);

                node = new Node();
                node["ID"].Value = n.ID;

                RaiseEvent(
                    "Magix.MetaType.EditObjectRaw",
                    node);

                node = new Node();
                node["ID"].Value = n.ID;
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "DBAdmin.Grid.SetActiveRow",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.CreateObject")]
        protected void Magix_MetaType_CreateObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject m = new MetaObject();
                m.Name = "[Anonymous-Coward]";
                m.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = m.ID;
                RaiseEvent(
                    "Magix.MetaType.EditObjectRaw",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.EditObjectRaw")]
        protected void Magix_MetaType_EditObjectRaw(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["MarginBottom"].Value = 10;
            node["Container"].Value = "content4";

            node["PropertyName"].Value = "Values";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject).FullName;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Val"].Value = true;
            node["WhiteListColumns"]["Val"]["ForcedWidth"].Value = 7;

            node["Type"]["Properties"]["Name"].Value = null; // just to touch it ...
            node["Type"]["Properties"]["Val"]["Header"].Value = "Value";

            node["ID"].Value = e.Params["ID"].Value;
            node["NoIdColumn"].Value = true;
            node["ReUseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.DeleteObjectRaw")]
        protected void Magix_MetaType_DeleteObjectRaw(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);
            Node node = e.Params;
            if (node == null)
            {
                node = new Node();
                node["ForcedSize"]["width"].Value = 550;
                node["WindowCssClass"].Value =
                    "mux-shaded mux-rounded push-5 down-2";
            }
            node["Caption"].Value = @"
Please confirm deletion of " + typeName + " with ID of " + id;
            node["Text"].Value = @"
<p>Are you sure you wish to delete this object? 
Deletion is permanent, and cannot be undone! 
Deletion of this object <span style=""color:Red;font-weight:bold;"">will also trigger 
deletion of several other objects</span>, since it may 
have relationships towards other instances in your database.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.MetaType.DeleteObjectRaw-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.DeleteObjectRaw-Confirmed")]
        protected void Magix_MetaType_DeleteObjectRaw_Confirmed(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject m = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                m.Delete();

                tr.Commit();

                ActiveEvents.Instance.RaiseClearControls("content4");
                ActiveEvents.Instance.RaiseClearControls("child");

                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        [ActiveEvent(Name = "DBAdmin.Common.CreateObjectAsChild")]
        protected void DBAdmin_Common_CreateObjectAsChild(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject.Value).FullName)
            {
                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaTypesCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaTypesCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaTypesCount"]["Header"].Value = "Objects";
            e.Params["Object"]["Properties"]["MetaTypesCount"].Value = MetaObject.Count.ToString();
        }
    }
}





















