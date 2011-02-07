/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Data.Internal;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveControllers.DBAdmin
{
    [ActiveController]
    public class DBAdminController : ActiveController
    {
        // Loads up DBAdmin into the current default container...
        // Doesn't require any input...
        [ActiveEvent(Name = "DBAdmin.Form.ViewClasses")]
        protected void DBAdmin_Form_ViewClasses(object sender, ActiveEventArgs e)
        {
            string container = null;
            Node node = new Node();
            if (e.Params.Contains("container"))
                container = e.Params["container"].Get<string>();
            node["Padding"].Value = 7;
            if (e.Params.Contains("Padding"))
                node["Padding"].Value = e.Params["Padding"].Get<int>();
            node["Width"].Value = 10;
            if (e.Params.Contains("Width"))
                node["Width"].Value = e.Params["Width"].Get<int>();
            node["Top"].Value = 3;
            if (e.Params.Contains("Top"))
                node["Top"].Value = e.Params["Top"].Get<int>();
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses",
                container,
                node);
        }

        // Called by BrowseClasses to fetch the classes for DBAdmin
        // Doesn't require any input...
        [ActiveEvent(Name = "DBAdmin.Data.GetClassHierarchy")]
        protected void DBAdmin_Data_GetClassHierarchy(object sender, ActiveEventArgs e)
        {
            int idxNo = 0;
            foreach (Type idx in Adapter.ActiveTypes)
            {
                string fullTypeName = idx.FullName;
                List<string> entities = new List<string>(fullTypeName.Split('.'));
                string typeName = entities[entities.Count - 1];
                entities.RemoveAt(entities.Count - 1);
                Node node = e.Params["Classes"];
                string tmpTypeName = "";
                foreach (string idxEntity in entities)
                {
                    node = node[idxEntity];
                    if (!string.IsNullOrEmpty(tmpTypeName))
                        tmpTypeName += ".";
                    tmpTypeName += idxEntity;
                    node["FullTypeName"].Value = tmpTypeName;
                    node["Name"].Value = idxEntity;
                }
                node = node[typeName];
                node["Name"].Value = typeName;
                node["FullTypeName"].Value = fullTypeName;
                node["IsLeafClass"].Value = true;
                idxNo += 1;
            }
        }

        // Requires a "FullTypeName" parameter, and nothing else.
        [ActiveEvent(Name = "DBAdmin.Form.ViewClass")]
        protected void DBAdmin_Form_ViewClass(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            ShowViewClassForm(e.Params, fullTypeName);
        }

        // Requires a "FullTypeName" parameter, and "Start" + "End"
        [ActiveEvent(Name = "DBAdmin.Data.GetContentsOfClass")]
        protected void DBAdmin_Data_GetContentsOfClass(object sender, ActiveEventArgs e)
        {
            string fullTypeName = 
                e.Params["FullTypeName"].Get<string>();
            Data.Instance.GetObjectTypeNode(fullTypeName, e.Params);
            List<Criteria> pars = 
                Data.Instance.GetCriteria(fullTypeName, e.Params);
            Data.Instance.GetObjectsNode(
                fullTypeName,
                e.Params,
                e.Params["Start"].Get<int>(0),
                e.Params["End"].Get<int>(Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10)),
                pars.ToArray());
            e.Params["End"].Value = e.Params["Start"].Get<int>(0) + e.Params["Objects"].Count;
            int count = Data.Instance.GetCount(fullTypeName, pars.ToArray());
            e.Params["SetCount"].Value = count;
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            string newValue = e.Params["NewValue"].Get<string>();
            Data.Instance.ChangeValue(
                id,
                fullTypeName,
                propertyName,
                newValue);
        }

        [ActiveEvent(Name = "DBAdmin.Form.ViewListOrComplexPropertyValue")]
        protected void DBAdmin_Form_ViewListOrComplexPropertyValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            bool isList = e.Params["IsList"].Get<bool>();
            Node node = new Node();
            node["ParentID"].Value = id;
            node["ParentFullTypeName"].Value = fullTypeName;
            node["ParentPropertyName"].Value = propertyName;
            if (isList)
            {
                Data.Instance.GetComplexPropertyFromListObjectNode(
                    fullTypeName,
                    id,
                    propertyName,
                    node,
                    0,
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
                node["Start"].Value = 0;
                node["End"].Value = node["Objects"].Count;
                node["IsFilter"].Value = false;
                LoadModule(
                    "Magix.Brix.Components.ActiveModules.DBAdmin.ViewListOfObjects",
                    "child",
                    node);
            }
            else
            {
                Data.Instance.GetComplexPropertyFromObjectNode(
                    fullTypeName,
                    id,
                    propertyName,
                    node);
                LoadModule(
                    "Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject",
                    "child",
                    node);
            }
        }

        [ActiveEvent(Name = "DBAdmin.Data.GetListFromObject")]
        protected void DBAdmin_UpdateComplexValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ParentID"].Get<int>();
            string fullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            string propertyName = e.Params["ParentPropertyName"].Get<string>();
            bool isList = e.Params.Contains("Start");
            e.Params["ParentID"].Value = id;
            e.Params["ParentFullTypeName"].Value = fullTypeName;
            e.Params["ParentPropertyName"].Value = propertyName;
            if (isList)
            {
                Data.Instance.GetComplexPropertyFromListObjectNode(
                    fullTypeName,
                    id,
                    propertyName,
                    e.Params,
                    e.Params["Start"].Get<int>(),
                    e.Params["Start"].Get<int>() +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
                e.Params["Start"].Value = e.Params["Start"].Get<int>(0);
                e.Params["End"].Value = e.Params["Start"].Get<int>() + e.Params["Objects"].Count;
            }
            else
            {
                Data.Instance.GetComplexPropertyFromObjectNode(
                    fullTypeName,
                    id,
                    propertyName,
                    e.Params);
            }
        }

        [ActiveEvent(Name = "DBAdmin.Data.GetObject")]
        protected void DBAdmin_Data_GetObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            Data.Instance.GetObjectNode(
                e.Params["Object"],
                id,
                fullTypeName,
                e.Params);
            Data.Instance.GetObjectTypeNode(fullTypeName, e.Params);
        }

        [ActiveEvent(Name = "DBAdmin.Data.GetObjectFromParentProperty")]
        protected void DBAdmin_Data_GetObjectFromParentProperty(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            int id = e.Params["ParentID"].Get<int>();
            string propertyName = e.Params["ParentPropertyName"].Get<string>();
            Data.Instance.GetComplexPropertyFromObjectNode(
                fullTypeName,
                id,
                propertyName,
                e.Params);
        }

        [ActiveEvent(Name = "DBAdmin.Form.ViewComplexObject")]
        protected void DBAdmin_ShowComplexObject(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<String>();

            Node node = e.Params;
            node["IsChange"].Value = false;
            node["IsRemove"].Value = false;
            ShowComplexObject(id, fullTypeName, node);
        }

        private void ShowComplexObject(int id, string fullTypeName, Node node)
        {
            Data.Instance.GetObjectTypeNode(fullTypeName, node);
            Data.Instance.GetObjectNode(node["Object"], id, fullTypeName, node);

            string container = "child";
            if (node.Contains("Container"))
                container = node["Container"].Get<string>();

            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject",
                container,
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Form.GetFilterForColumn")]
        protected void DBAdmin_Form_GetFilterForColumn(object sender, ActiveEventArgs e)
        {
            string propertyName = e.Params["PropertyName"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();

            // TODO: Refactor to Data class, somehow ...
            Type type = Data.Instance.GetType(fullTypeName);
            PropertyInfo prop =
                type.GetProperty(
                    propertyName,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            Node node = new Node();
            node["PropertyTypeName"].Value = prop.PropertyType.FullName;
            node["FullTypeName"].Value = fullTypeName;
            node["PropertyName"].Value = propertyName;
            if (e.Params.Contains("WhiteListColumns"))
                node["WhiteListColumns"] = e.Params["WhiteListColumns"];
            node["Caption"].Value =
                string.Format(
                    @"Create filter for {0} [{2}] on {1}",
                    propertyName,
                    type.Name,
                    prop.PropertyType.Name);
            if (propertyName == "ID")
            {
                node["ForcedSize"]["width"].Value = 490;
                node["ForcedSize"]["height"].Value = 234;
            }
            else
            {
                node["ForcedSize"]["width"].Value = 530;
                node["ForcedSize"]["height"].Value = 234;
            }
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureFilters",
                "child",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Form.ShowAddRemoveColumns")]
        protected void DBAdmin_Form_ShowAddRemoveColumns(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            Node node = new Node();
            node["Caption"].Value = 
                string.Format("Configure visible columns for {0}", 
                    fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1));
            node["FullTypeName"].Value = fullTypeName;
            if (e.Params.Contains("WhiteListColumns"))
                node["WhiteListColumns"] = e.Params["WhiteListColumns"];
            Data.Instance.GetObjectTypeNode(fullTypeName, node);
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureColumns",
                "child",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeVisibilityOfColumn")]
        protected void DBAdmin_Data_ChangeVisibilityOfColumn(object sender, ActiveEventArgs e)
        {
            string columnName = e.Params["ColumnName"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            bool visible = e.Params["Visible"].Get<bool>();
            string settingsBaseValue =
                "DBAdmin.VisibleColumns." +
                fullTypeName + ":" + columnName;
            Settings.Instance.Set(settingsBaseValue, visible);
        }

        [ActiveEvent(Name = "DBAdmin.Data.DeleteObject")]
        protected void DBAdmin_Data_DeleteObject(object sender, ActiveEventArgs e)
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
Deletion of this object <span style=""color:Red;font-weight:bold;"">might also trigger 
deletion of several other objects</span>, since it may 
have relationships towards other instances in your database.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedConfirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Data_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string typeName = e.Params["FullTypeName"].Get<string>();
            Data.Instance.DeleteObject(id, typeName);
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed")]
        protected void DBAdmin_Data_ComplexInstanceDeletedNotConfirmed(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        [ActiveEvent(Name = "DBAdmin.Common.CreateObject")]
        protected void DBAdmin_Common_CreateObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = Data.Instance.CreateObject(fullTypeName);
            if (id == 0)
                throw new ApplicationException(
                    @"Couldn't create object, something went wrong in your
model while trying to create object, and it was never created for some reasons.");
            Node node = new Node();
            node["IsChange"].Value = false;
            node["IsRemove"].Value = false;
            node["FullTypeName"].Value = fullTypeName;
            node["ID"].Value = id;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        private void ShowViewClassForm(Node node, string fullTypeName)
        {
            Data.Instance.GetObjectTypeNode(fullTypeName, node);
            List<Criteria> pars =
                Data.Instance.GetCriteria(fullTypeName, node);
            Data.Instance.GetObjectsNode(
                fullTypeName,
                node,
                0,
                Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10),
                pars.ToArray());
            if (!node.Contains("IsDelete"))
                node["IsDelete"].Value = true;
            if (!node.Contains("IsCreate"))
                node["IsCreate"].Value = true;
            if (!node.Contains("IsFilter"))
                node["IsFilter"].Value = true;
            node["Start"].Value = 0;
            node["End"].Value = node["Objects"].Count;
            int count = Data.Instance.GetCount(fullTypeName, pars.ToArray());
            node["SetCount"].Value = count;
            string container = "child";
            if (node.Contains("Container"))
                container = node["Container"].Get<string>();
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents",
                container,
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Form.AppendObject")]
        protected void DBAdmin_Form_AppendObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            Node node = new Node();
            node["FullTypeName"].Value = fullTypeName;
            node["ParentID"].Value = parentId;
            node["IsSelect"].Value = true;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentFullTypeName"].Value = parentFullTypeName;
            node["IsList"].Value = true;
            ShowViewClassForm(node, fullTypeName);
        }

        [ActiveEvent(Name = "DBAdmin.Data.AppendObjectToParentPropertyList")]
        protected void DBAdmin_Data_AppendObjectToParentPropertyList(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            Data.Instance.AppendObjectToParentPropertyList(
                id,
                fullTypeName,
                parentId,
                parentPropertyName,
                parentFullTypeName);

            // Closing current form ...
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeObjectReference")]
        protected void DBAdmin_Data_ChangeObjectReference(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            Data.Instance.ChangeObjectReference(
                id,
                fullTypeName,
                parentId,
                parentPropertyName,
                parentFullTypeName);

            // Closing current form ...
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        [ActiveEvent(Name = "DBAdmin.Form.RemoveObjectFromParentPropertyList")]
        protected void DBAdmin_Form_RemoveObjectFromParentPropertyList(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);
            Node node = new Node();
            node["Caption"].Value = @"
Please confirm removal of " + typeName + " with ID of " + id;
            node["Text"].Value = @"
<p>Are you sure you wish to remove this object? 
Removal is permanent, and cannot be undone! 
Removal of this object <span style=""color:Red;font-weight:bold;"">might also trigger 
deletion of the object, and all its child objects</span>, since it may 
have relationships towards other instances in your database, and be owned by the 
collection you're removing it from.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["ParentID"].Value = parentId;
            node["OK"]["ParentPropertyName"].Value = parentPropertyName;
            node["OK"]["ParentFullTypeName"].Value = parentFullTypeName;
            node["OK"]["Event"].Value = "DBAdmin.Data.RemoveObjectFromParentPropertyList";
            node["Cancel"]["Event"].Value = "DBAdmin.Data.DoNotRemoveObjectFromParentPropertyList";
            node["ForcedSize"]["width"].Value = 530;
            node["WindowCssClass"].Value =
                "mux-shaded mux-rounded";
            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        [ActiveEvent(Name = "DBAdmin.Data.RemoveObjectFromParentPropertyList")]
        protected void DBAdmin_Data_RemoveObjectFromParentPropertyList(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            Data.Instance.RemoveObjectFromParentPropertyList(
                id,
                fullTypeName,
                parentId,
                parentPropertyName,
                parentFullTypeName);

            // Closing current form ...
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        [ActiveEvent(Name = "DBAdmin.Form.ChangeObject")]
        protected void DBAdmin_Form_ChangeObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            Node node = new Node();
            node["IsSelect"].Value = true;
            node["IsList"].Value = false;
            node["ParentID"].Value = parentId;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentFullTypeName"].Value = parentFullTypeName;
            ShowViewClassForm(node, fullTypeName);
        }

        [ActiveEvent(Name = "DBAdmin.Data.RemoveObject")]
        protected void DBAdmin_Form_RemoveObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            Data.Instance.RemoveObjectFromParentProperty(
                fullTypeName,
                parentId,
                parentPropertyName,
                parentFullTypeName);
        }

        [ActiveEvent(Name = "DBAdmin.Data.GetFilter")]
        protected void DBAdmin_Data_GetFilter(object sender, ActiveEventArgs e)
        {
            string key = e.Params["Key"].Get<string>();
            string defaultValue = e.Params["Default"].Get<string>();
            string idFilter =
                Settings.Instance.Get(
                    key,
                    defaultValue);
            e.Params["Filter"].Value = idFilter;
        }

        [ActiveEvent(Name = "DBAdmin.Data.SetFilter")]
        protected void DBAdmin_Data_SetFilter(object sender, ActiveEventArgs e)
        {
            string key = e.Params["Key"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            Settings.Instance.Set(key,value);
        }
    }
}














