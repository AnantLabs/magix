/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Contains the ogic for the DBAdmin, which is also the Grid/CRUD-Foundation
     * system in Magix. Contains many useful methods and ActiveEvents for displaying
     * either Grids or editing Single Instances of Objects. Can react 100% 
     * transparently on ActiveTypes. Has support for Meta Types, meaning types where
     * you're more in 'control', but must do more of the 'arm wrestling directly'
     */
    [ActiveController]
    public class DBAdmin_Controller : ActiveController
    {
        /**
         * Loads up the DBAdmin BrowserClasses interface. Pass in a node for
         * positioning and such
         */
        [ActiveEvent(Name = "DBAdmin.Form.ViewClasses")]
        protected void DBAdmin_Form_ViewClasses(object sender, ActiveEventArgs e)
        {
            if (e.Params == null)
                e.Params = new Node();
            
            Node node = e.Params;
            string container = node["Container"].Get<string>();

            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses",
                container,
                node);
        }

        /**
         * Will return the Class Hierarchy of all ActiveTypes within the system in a 
         * tree hierarchy, according to namespace
         */
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

        /**
         * Will show all the objects of type 'FullTypeName' by using the passed in
         * node for settings in regards to positioning and such
         */
        [ActiveEvent(Name = "DBAdmin.Form.ViewClass")]
        protected void DBAdmin_Form_ViewClass(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            ShowViewClassForm(e.Params, fullTypeName, e.Params);
        }

        /*
         * Helper for above ++
         */
        private void ShowViewClassForm(Node node, string fullTypeName, Node xx)
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

            if (!node.Contains("SetCount") ||
                !node.Contains("LockSetCount") ||
                !node["LockSetCount"].Get<bool>())
            {
                int count = Data.Instance.GetCount(fullTypeName, pars.ToArray(), xx);
                node["SetCount"].Value = count;
            }

            string container = "child";

            if (node.Contains("Container"))
                container = node["Container"].Get<string>();

            LoadModule(
                node.Contains("IsFind") && node["IsFind"].Get<bool>() ? "Magix.Brix.Components.ActiveModules.DBAdmin.FindObject" : "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents",
                container,
                node);
        }

        /**
         * Will return a range of objects type 'FullTypeName' depending upon the 'Start' and 'End' 
         * parameter. Requires a "FullTypeName" parameter, and "Start" + "End"
         */
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
            if (!e.Params.Contains("SetCount") ||
                !e.Params.Contains("LockSetCount") ||
                !e.Params["LockSetCount"].Get<bool>())
            {
                int count = Data.Instance.GetCount(fullTypeName, pars.ToArray(), e.Params);
                e.Params["SetCount"].Value = count;
            }
        }

        /**
         * Will change a 'Simple Property Value' [property of some sort belonging to the object, e.g. 
         * a DateTime/Birthday property]
         * Needs 'FullTypeName', 'ID', 'PropertyName', 'NewValue' to work. NewValue must be of type
         * of property.
         */
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

        /**
         * Will show either a Child object or a List of children depending upon the 'IsList' parameter
         */
        [ActiveEvent(Name = "DBAdmin.Form.ViewListOrComplexPropertyValue")]
        protected void DBAdmin_Form_ViewListOrComplexPropertyValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            bool isList = e.Params["IsList"].Get<bool>();

            Node node = new Node();
            if (e.Params.Contains("ReUseNode") && e.Params["ReUseNode"].Get<bool>())
                node = e.Params;

            node["ParentID"].Value = id;
            node["ParentFullTypeName"].Value = fullTypeName;
            node["ParentPropertyName"].Value = propertyName;

            if (e.Params.Contains("Append"))
                node["Append"].Value = e.Params["Append"].Value;

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
                    (e.Params.Contains("Container") ? e.Params["Container"].Get<string>() :  "child"),
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

        /**
         * Returns a Range of Child Objects belonging to the 'ParentID'. Needs 'ParentFullTypeName',
         * 'ParentPropertyName', 'Start', 'End' and 'ParentID' to function
         */
        [ActiveEvent(Name = "DBAdmin.Data.GetListFromObject")]
        protected void DBAdmin_UpdateComplexValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ParentID"].Get<int>();
            string fullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            string propertyName = e.Params["ParentPropertyName"].Get<string>();
            bool isList = e.Params.Contains("Start");
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

        /**
         * Will return the object with the given 'ID' being of 'FullTypeName' as a Value/Key pair
         */
        [ActiveEvent(Name = "DBAdmin.Data.GetObject")]
        protected void DBAdmin_Data_GetObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            Node objNode = new Node("Object");

            Data.Instance.GetObjectNode(
                objNode,
                id,
                fullTypeName,
                e.Params);

            if (objNode.Count > 0)
                e.Params.Add(objNode);

            Data.Instance.GetObjectTypeNode(fullTypeName, e.Params);
        }

        /**
         * Will return a single instance of a complex [ActiveType normally, unless 'meta'] child object
         */
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

        /**
         * Vill show one Complex object with the 'ID' and 'FullTypeName'
         */
        [ActiveEvent(Name = "DBAdmin.Form.ViewComplexObject")]
        protected void DBAdmin_ShowComplexObject(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<String>();

            e.Params["IsChange"].Value = false;
            e.Params["IsRemove"].Value = false;
            e.Params["IsDelete"].Value = false;

            ShowComplexObject(id, fullTypeName, e.Params);
        }

        /*
         * Helper for above ...
         */
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

        /**
         * Will open up a 'Configure Filter' dialogue from which the user can change, edit or remove
         * any existing filters or create new ones
         */
        [ActiveEvent(Name = "DBAdmin.Form.ConfigureFilterForColumn")]
        protected void DBAdmin_Form_ConfigureFilterForColumn(object sender, ActiveEventArgs e)
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

        /**
         * Will load up 'Configure Columns to View form' to end user
         */
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

        /**
         * Changes the visibility setting of a specific Column for a specific type
         */
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

        /**
         * Will delete the given 'ID' ActiveType within the 'FullTypeName' namespace/name after
         * user has been asked to confirm deletion
         */
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
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Default implementation of 'deletion of object was confirmed by user' logic
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Data_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string typeName = e.Params["FullTypeName"].Get<string>();
            Data.Instance.DeleteObject(id, typeName);

            // Closing top-most Window, which should be our Message Box ...
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Flushes the Container containing the MessageBox
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed")]
        protected void DBAdmin_Data_ComplexInstanceDeletedNotConfirmed(object sender, ActiveEventArgs e)
        {
            // Closing top-most Window, which should be our Message Box ...
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Will create a new object of type 'FullTypeName'
         */
        [ActiveEvent(Name = "DBAdmin.Common.CreateObject")]
        protected void DBAdmin_Common_CreateObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();

            int id = Data.Instance.CreateObject(fullTypeName, e.Params);

            if (id == 0)
                throw new ApplicationException(
                    @"Couldn't create object, something went wrong in your
model while trying to create object, and it was never created for some reasons.");
        }

        /**
         * Will create a new object of type 'ParentFullTypeName' and append it to the 'ParentID'
         * 'ParentPropertyName' property which must be of type 'FullTypeName'
         */
        [ActiveEvent(Name = "DBAdmin.Common.CreateObjectAsChild")]
        protected void DBAdmin_Common_CreateObjectAsChild(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();

            int id = Data.Instance.CreateObject(fullTypeName, e.Params);

            if (id == 0)
                throw new ApplicationException(
                    @"Couldn't create object, something went wrong in your
model while trying to create object, and it was never created for some reasons.");

            Data.Instance.AppendObjectToParentPropertyList(
                id, 
                fullTypeName, 
                parentId, 
                parentPropertyName, 
                parentFullTypeName);
        }

        /**
         * Will show a list of objects of type 'FullTypeName' and allow the user
         * to pick one to append into 'ParentID' 'ParentPropertyName' with the given
         * 'ParentFullTypeName'
         */
        [ActiveEvent(Name = "DBAdmin.Form.AppendObject")]
        protected void DBAdmin_Form_AppendObject(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentFullTypeName = e.Params["ParentFullTypeName"].Get<string>();
            Node node = new Node();

            if (e.Params.Contains("ReUseNode") &&
                e.Params["ReUseNode"].Get<bool>())
                node = e.Params;

            node["FullTypeName"].Value = fullTypeName;
            node["ParentID"].Value = parentId;
            node["IsSelect"].Value = true;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentFullTypeName"].Value = parentFullTypeName;
            node["IsList"].Value = true;

            ShowViewClassForm(node, fullTypeName, e.Params);
        }

        /**
         * Will append an object to a list of objects in 'ParentID' ParentPropertyName collection and
         * save the 'ParentID' object
         */
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

        /**
         * Will change a single instance object reference between 'ParentID' and 'ID' in the
         * 'ParentPropertyName' of 'ParentFullTypeName'. Flushes child container
         */
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

        /**
         * Removes a referenced object without deleting it [taking it out of its parent collection]
         * Notice that if the Parent object is the 'Owner' of the object, it may still be deleted.
         * Will ask for confirmation from end user before operation is performed
         */
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
            node["OK"]["Event"].Value = "DBAdmin.Form.RemoveObjectFromParentPropertyList-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Data.DoNotRemoveObjectFromParentPropertyList";
            node["ForcedSize"]["width"].Value = 530;
            node["WindowCssClass"].Value =
                "mux-shaded mux-rounded";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Removes an object out of its 'ParentID' 'ParentPropertyName' collection of type
         * 'ParentFullTypeName'
         */
        [ActiveEvent(Name = "DBAdmin.Form.RemoveObjectFromParentPropertyList-Confirmed")]
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

        /**
         * Will show the 'Change Single-Object Reference' form to the end user
         */
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

            ShowViewClassForm(node, fullTypeName, e.Params);
        }

        /**
         * Removes a single-object reference from the 'ParentID' object
         */
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

        /**
         * Returns the filters for different columns in the Grid system
         */
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

        /**
         * Changes the filter for a specific 'Key'/'Value' for a specific type
         */
        [ActiveEvent(Name = "DBAdmin.Data.SetFilter")]
        protected void DBAdmin_Data_SetFilter(object sender, ActiveEventArgs e)
        {
            string key = e.Params["Key"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            Settings.Instance.Set(key,value);
        }
    }
}














