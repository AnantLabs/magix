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
        [ActiveEvent(Name = "DBAdmin.Load")]
        protected void DBAdmin_Load(object sender, ActiveEventArgs e)
        {
            LoadModule("Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses");
        }

        // Called by BrowseClasses to fetch the classes for DBAdmin
        [ActiveEvent(Name = "DBAdmin.BrowseClassHierarchy")]
        protected void DBAdmin_BrowseClassHierarchy(object sender, ActiveEventArgs e)
        {
            int idxNo = 0;
            foreach (Type idx in PluginLoader.Instance.ActiveTypes)
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

        // Returns the starting value, 0 if none given...
        private int GetStart(Node node)
        {
            int start = 0;
            if (node.Contains("Start"))
                start = node["Start"].Get<int>();
            return Math.Max(0, start);
        }

        // Returns the starting value, start + MaxItemsToShow from settings if none given...
        private int GetEnd(Node node, int start)
        {
            int end = start + Settings.Instance.Get("DBAdmin.MaxItemsToShow", 50);
            if (node.Contains("End"))
                end = Math.Max(start + 1, node["End"].Get<int>());
            return end;
        }

        // Returns the Active Type from the full Type Name
        private Type GetType(string fullTypeName)
        {
            Type type = (new List<Type>(PluginLoader.Instance.ActiveTypes)).Find(
                delegate(Type idx)
                {
                    return idx.FullName == fullTypeName;
                });
            return type;
        }

        private PropertyInfo[] GetProps(Type type)
        {
            return type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);
        }

        private Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> GetMethodInfos(
            PropertyInfo[] props)
        {
            Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> retVal = 
                new Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>>();
            foreach (PropertyInfo idx in props)
            {
                ActiveFieldAttribute[] attrs =
                    idx.GetCustomAttributes(
                        typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    // Serializable property...
                    retVal[idx.Name] = 
                        new Tuple<MethodInfo, ActiveFieldAttribute>(
                            idx.GetGetMethod(true), 
                            attrs[0]);
                }
            }
            return retVal;
        }

        // Returns an IEnumerable running Select towards the given Active Type...
        private System.Collections.IEnumerable SelectObjects(Type type)
        {
            MethodInfo retrieveAllObjects = type.GetMethod(
                "Select",
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.NonPublic);
            Criteria[] pars = new Criteria[0];
            System.Collections.IEnumerable enumerable =
                retrieveAllObjects.Invoke(
                    null,
                    new object[] { pars }) as System.Collections.IEnumerable;
            return enumerable;
        }

        private int GetID(object obj, Type type)
        {
            return (int)type.GetProperty("ID").GetGetMethod().Invoke(obj, null);
        }

        // Creates a list of Nodes according to how it's supposed to look 
        // like in the Node structure for lists
        private Node GetNodeList(
            System.Collections.IEnumerable objects, 
            Node node,
            Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters,
            Type type,
            int start,
            int end)
        {
            int idxNo = -1;
            node["TypeName"].Value = type.Name;
            node["FullTypeName"].Value = type.FullName;
            foreach (string idxKey in getters.Keys)
            {
                Tuple<MethodInfo, ActiveFieldAttribute> tuple = getters[idxKey];
                string typeName = tuple.Left.ReturnType.FullName;
                if (typeName.IndexOf("Magix.Brix.Types.LazyList") == 0)
                {
                    node["Type"]["Properties"][idxKey]["IsList"].Value = true;
                    string ls = "LazyList&lt;";
                    ls += tuple.Left.ReturnType.GetGenericArguments()[0].Name + "&gt;";
                    node["Type"]["Properties"][idxKey]["TypeName"].Value = ls;
                }
                else if (typeName.IndexOf("System.Collections.Generics.List") == 0)
                {
                    node["Type"]["Properties"][idxKey]["IsList"].Value = true;
                    string ls = "List&lt;";
                    ls += tuple.Left.ReturnType.GetGenericArguments()[0].Name + "&gt;";
                    node["Type"]["Properties"][idxKey]["TypeName"].Value = ls;
                }
                else
                {
                    node["Type"]["Properties"][idxKey]["IsList"].Value = false;
                    node["Type"]["Properties"][idxKey]["TypeName"].Value = tuple.Left.ReturnType.Name;
                }
                node["Type"]["Properties"][idxKey]["Name"].Value = tuple.Left.Name.Replace("get_", "");
                node["Type"]["Properties"][idxKey]["BelongsTo"].Value = tuple.Right.BelongsTo;
                node["Type"]["Properties"][idxKey]["IsOwner"].Value = tuple.Right.IsOwner;
                node["Type"]["Properties"][idxKey]["RelationName"].Value = tuple.Right.RelationName;
            }
            foreach (object idxObj in objects)
            {
                idxNo += 1;
                if (idxNo < start)
                    continue;
                if (idxNo >= end)
                    break;
                int id = GetID(idxObj, type);
                node["Objects"]["Obj" + id]["ID"].Value = id;
                GetObject(node["Objects"]["Obj" + id], getters, idxObj, id);
            }
            return node;
        }

        private static void GetObject(Node node, Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters, object idxObj, int id)
        {
            foreach (string idxKey in getters.Keys)
            {
                Tuple<MethodInfo, ActiveFieldAttribute> tuple = getters[idxKey];
                object value = tuple.Left.Invoke(idxObj, null);
                switch (tuple.Left.ReturnType.FullName)
                {
                    case "System.String":
                    case "System.Boolean":
                    case "System.Int32":
                    case "System.Byte[]": // TODO: Special treatment ...?
                        if (value == null)
                            value = "[null]";
                        node["Properties"][idxKey]["IsComplex"].Value = false;
                        node["Properties"][idxKey]["Value"].Value = value.ToString();
                        node["Properties"][idxKey]["TypeName"].Value = tuple.Left.ReturnType.Name;
                        break;
                    case "System.Decimal":
                        node["Properties"][idxKey]["Value"].Value =
                            ((decimal)value).ToString(CultureInfo.InvariantCulture);
                        node["Properties"][idxKey]["IsComplex"].Value = false;
                        node["Properties"][idxKey]["TypeName"].Value = tuple.Left.ReturnType.Name;
                        break;
                    case "System.DateTime":
                        node["Properties"][idxKey]["Value"].Value =
                            ((DateTime)value).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                        node["Properties"][idxKey]["IsComplex"].Value = false;
                        node["Properties"][idxKey]["TypeName"].Value = tuple.Left.ReturnType.Name;
                        break;
                    default:
                        string typeName = tuple.Left.ReturnType.FullName;
                        if (typeName.IndexOf("Magix.Brix.Types.LazyList") == 0)
                        {
                            node["Properties"][idxKey]["IsComplex"].Value = true;
                            node["Properties"][idxKey]["IsList"].Value = true;
                            string valueStr = value == null ? "[null]" : "[#" +
                                tuple.Left.ReturnType.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true).Invoke(value, null) + "]";
                            node["Properties"][idxKey]["Value"].Value = valueStr;

                            node["Properties"][idxKey]["PropertyName"].Value =
                                tuple.Left.Name.Replace("get_", "");
                            string ls = "LazyList&lt;";
                            ls += tuple.Left.ReturnType.GetGenericArguments()[0].Name + "&gt;";
                            node["Properties"][idxKey]["TypeName"].Value = ls;
                        }
                        else if (typeName.IndexOf("System.Collections.Generics.List") == 0)
                        {
                            node["Properties"][idxKey]["IsComplex"].Value = true;
                            node["Properties"][idxKey]["IsList"].Value = true;
                            string valueStr = value == null ? "[null]" : "[#" +
                                tuple.Left.ReturnType.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true).Invoke(value, null) + "]";
                            node["Properties"][idxKey]["Value"].Value = valueStr;

                            node["Properties"][idxKey]["PropertyName"].Value =
                                tuple.Left.Name.Replace("get_", "");
                            string ls = "List&lt;";
                            ls += tuple.Left.ReturnType.GetGenericArguments()[0].Name + "&gt;";
                            node["Properties"][idxKey]["TypeName"].Value = ls;
                        }
                        else
                        {
                            node["Properties"][idxKey]["IsComplex"].Value = true;
                            node["Properties"][idxKey]["IsList"].Value = false;
                            string valueStr = value == null ? "[null]" : "[@" + value.ToString() + "]";
                            node["Properties"][idxKey]["Value"].Value = valueStr;
                            node["Properties"][idxKey]["TypeName"].Value = tuple.Left.ReturnType.Name;
                        }
                        node["Properties"][idxKey]["BelongsTo"].Value = tuple.Right.BelongsTo;
                        node["Properties"][idxKey]["IsOwner"].Value = tuple.Right.IsOwner;
                        node["Properties"][idxKey]["RelationName"].Value = tuple.Right.RelationName;
                        break;
                }
                node["Properties"][idxKey]["PropertyName"].Value =
                    tuple.Left.Name.Replace("get_", "");
            }
        }

        private int GetCount(Type type)
        {
            return (int)type.GetProperty(
                "Count", 
                BindingFlags.FlattenHierarchy | 
                BindingFlags.Static | 
                BindingFlags.NonPublic | 
                BindingFlags.Public)
                .GetGetMethod(true)
                .Invoke(null, null);
        }

        // ViewContents Form
        [ActiveEvent(Name = "DBAdmin.ViewContents")]
        protected void DBAdmin_ViewContents(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            FillNodeWithViewContents(e, node);
            node["IsDelete"].Value = true;
            e.Params = node;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents",
                "child",
                node);
        }

        // UPDATE the ViewContents Form
        [ActiveEvent(Name = "DBAdmin.UpdateContents")]
        protected void DBAdmin_UpdateContents(object sender, ActiveEventArgs e)
        {
            FillNodeWithViewContents(e, e.Params);
            e.Params["IsDelete"].Value = true;
        }

        // Fill node with contents according to start/end of list view ...
        private void FillNodeWithViewContents(ActiveEventArgs e, Node node)
        {
            int start = GetStart(e.Params);
            int end = GetEnd(e.Params, start);
            Type type = GetType(e.Params["FullTypeName"].Get<string>());
            Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters =
                GetMethodInfos(GetProps(type));
            System.Collections.IEnumerable enumerable = SelectObjects(type);
            node = GetNodeList(enumerable, node, getters, type, start, end);
            node["Start"].Value = start;
            node["End"].Value = node["Objects"].Count + start;
            node["TotalCount"].Value = GetCount(type);
        }

        private object GetObject(Type type, int id)
        {
            return
                type.GetMethod(
                    "SelectByID",
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Static).Invoke(null, new object[] { id });
        }

        private System.Collections.IEnumerable GetList(object from, string propertyName, Type type)
        {
            System.Collections.IEnumerable retVal =
                type.GetProperty(propertyName).GetGetMethod(true).Invoke(from, null) as System.Collections.IEnumerable;
            return retVal;
        }

        // Called when a List of items should be displayed
        [ActiveEvent(Name = "DBAdmin.ViewList")]
        protected void DBAdmin_ViewList(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            Type type = GetType(fullTypeName);
            object parentObject = GetObject(type, id);
            System.Collections.IEnumerable enumerable = GetList(parentObject, propertyName, type);
            Node node = new Node();
            Type typeOfList = GetListType(type, propertyName);
            Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters = GetMethodInfos(GetProps(typeOfList));
            GetNodeList(
                enumerable,
                node,
                getters,
                typeOfList,
                0,
                Settings.Instance.Get("DBAdmin.MaxItemsToShow", 50));
            node["Start"].Value = 0;
            node["IsRemove"].Value = !e.Params["BelongsTo"].Get<bool>();
            node["IsAppend"].Value = !e.Params["BelongsTo"].Get<bool>();
            node["ParentPropertyName"].Value = propertyName;
            node["ParentType"].Value = parentObject.GetType().Name;
            node["ParentFullType"].Value = parentObject.GetType().FullName;
            node["ParentID"].Value = id;
            node["End"].Value = node["Objects"].Count;
            node["TotalCount"].Value = GetCountFromList(parentObject, propertyName, type);
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewListOfObjects",
                "child",
                node);
        }

        // Called when a List of items should be displayed
        [ActiveEvent(Name = "DBAdmin.UpdateList")]
        protected void DBAdmin_UpdateList(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string parentFullType = e.Params["ParentFullType"].Get<string>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            int parentId = e.Params["ParentID"].Get<int>();
            Type type = GetType(fullTypeName);
            Type parentType = GetType(parentFullType);
            object parentObject = GetObject(parentType, parentId);
            System.Collections.IEnumerable enumerable = GetList(parentObject, parentPropertyName, parentType);
            Node node = e.Params;
            Type typeOfList = GetListType(parentType, parentPropertyName);
            Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters = GetMethodInfos(GetProps(typeOfList));
            GetNodeList(
                enumerable,
                node,
                getters,
                typeOfList,
                0,
                Settings.Instance.Get("DBAdmin.MaxItemsToShow", 50));
            node["Start"].Value = 0;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentType"].Value = parentObject.GetType().Name;
            node["ParentFullType"].Value = parentObject.GetType().FullName;
            node["End"].Value = node["Objects"].Count;
            node["TotalCount"].Value = GetCountFromList(parentObject, parentPropertyName, parentType);
            e.Params = node;
        }

        private int GetCountFromList(object obj, string listPropName, Type typeOfObj)
        {
            object tmp = typeOfObj.GetProperty(listPropName).GetGetMethod().Invoke(obj, null);
            return (int)tmp.GetType().GetProperty("Count").GetGetMethod().Invoke(tmp, null);
        }

        private Type GetListType(Type type, string propertyName)
        {
            return type.GetProperty(propertyName).PropertyType.GetGenericArguments()[0];
        }

        private object GetPropertyObject(Type type, object parentObject, string propertyName)
        {
            PropertyInfo prop = type.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo get = prop.GetGetMethod(true);
            return get.Invoke(parentObject, null);
        }

        // Called when a List of items should be displayed
        [ActiveEvent(Name = "DBAdmin.ViewSingleInstance")]
        protected void DBAdmin_ViewSingleInstance(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            Type type = GetType(fullTypeName);
            object parentObject = GetObject(type, id);
            object propertyToEdit = GetPropertyObject(type, parentObject, propertyName);
            Node node = new Node();
            node["ParentPropertyName"].Value = propertyName;
            node["ParentType"].Value = type.Name;
            node["ParentFullType"].Value = type.FullName;
            node["ParentID"].Value = id;
            if (propertyToEdit != null)
            {
                Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters =
                    GetMethodInfos(GetProps(propertyToEdit.GetType()));
                GetObject(node["Object"], getters, propertyToEdit, id);
                node["Object"]["ID"].Value = GetID(propertyToEdit, propertyToEdit.GetType());
            }
            PropertyInfo prop = type.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo get = prop.GetGetMethod(true);
            node["FullTypeName"].Value = get.ReturnType.FullName;
            node["TotalCount"].Value = GetCount(get.ReturnType);
            node["EditingAllowed"].Value = !e.Params["BelongsTo"].Get<bool>();
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject",
                "child",
                node);
        }

        // Called when a List of items should be displayed
        [ActiveEvent(Name = "DBAdmin.UpdateSingleInstance")]
        protected void DBAdmin_UpdateSingleInstance(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string parentTypeName = e.Params["ParentFullType"].Get<string>();
            string propertyName = e.Params["ParentPropertyName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            int parentId = e.Params["ParentID"].Get<int>();

            Type type = GetType(fullTypeName);
            Type parentType = GetType(parentTypeName);

            object parentObject = GetObject(parentType, parentId);
            object propertyToEdit = GetPropertyObject(parentType, parentObject, propertyName);

            if (propertyToEdit != null)
            {
                Dictionary<string, Tuple<MethodInfo, ActiveFieldAttribute>> getters =
                    GetMethodInfos(GetProps(propertyToEdit.GetType()));
                GetObject(e.Params["Object"], getters, propertyToEdit, id);
            }
            e.Params["Object"]["ID"].Value = id;
            e.Params["TotalCount"].Value = GetCount(type);
        }

        // Called when a List of items should be displayed
        [ActiveEvent(Name = "DBAdmin.ChangeValue")]
        protected void DBAdmin_ChangeValue(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string typeName = e.Params["FullTypeName"].Get<string>();
            string propertyName = e.Params["PropertyName"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            Type type = GetType(typeName);
            MethodInfo setter = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).GetSetMethod(true);
            object parent = GetObject(type, id);
            object valueChanged = Convert.ChangeType(value, setter.GetParameters()[0].ParameterType);
            setter.Invoke(parent, new object[] { valueChanged });
            type.GetMethod("Save", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(parent, null);
        }

        // Called when in View Single Object one chooses to replace the current object
        // with another instance.
        [ActiveEvent(Name = "DBAdmin.ChangeComplexInstance")]
        protected void DBAdmin_ChangeComplexInstance(object sender, ActiveEventArgs e)
        {
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentType = e.Params["ParentType"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();

            Node node = new Node();
            FillNodeWithViewContents(e, node);

            // Special nodes to signal to the form how it's supposed to do a "selection"
            // and where to put its "results"...
            node["IsSelect"].Value = true;
            node["SelectEventToFire"].Value = "ComplexInstanceChanged";
            node["IsListAppend"].Value = false;
            node["ParentID"].Value = parentId;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentType"].Value = parentType;

            node["IsDelete"].Value = true;
            e.Params = node;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents",
                "child",
                node);
        }

        // Called when in View List Of Objects one chooses to append a new object into the list
        [ActiveEvent(Name = "DBAdmin.AppendComplexInstance")]
        protected void DBAdmin_AppendComplexInstance(object sender, ActiveEventArgs e)
        {
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentType = e.Params["ParentType"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();

            Node node = new Node();
            FillNodeWithViewContents(e, node);

            // Special nodes to signal to the form how it's supposed to do a "selection"
            // and where to put its "results"...
            node["IsSelect"].Value = true;
            node["SelectEventToFire"].Value = "ComplexObjectAppended";
            node["IsListAppend"].Value = false;
            node["ParentID"].Value = parentId;
            node["ParentPropertyName"].Value = parentPropertyName;
            node["ParentType"].Value = parentType;

            node["IsDelete"].Value = true;
            e.Params = node;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents",
                "child",
                node);
        }

        // Called when in View Single Object one chooses to replace the current object
        // with another instance.
        [ActiveEvent(Name = "DBAdmin.ComplexInstanceChanged")]
        protected void DBAdmin_ComplexInstanceChanged(object sender, ActiveEventArgs e)
        {
            bool isListAppend = e.Params["IsListAppend"].Get<bool>();
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentTypeName = e.Params["ParentType"].Get<string>();
            int newObjectID = e.Params["NewObjectID"].Get<int>();
            string newObjectType = e.Params["NewObjectType"].Get<string>();

            Type parentType = GetType(parentTypeName);
            Type objectType = GetType(newObjectType);
            object parentObject = GetObject(parentType, parentId);
            object newChild = GetObject(objectType, newObjectID);
            MethodInfo setter = parentType.GetProperty(
                parentPropertyName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance)
                .GetSetMethod(true);
            setter.Invoke(parentObject, new object[] { newChild });
            parentType.GetMethod("Save").Invoke(parentObject, null);
        }

        [ActiveEvent(Name = "DBAdmin.ComplexObjectAppended")]
        protected void DBAdmin_ComplexObjectAppended(object sender, ActiveEventArgs e)
        {
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentTypeName = e.Params["ParentType"].Get<string>();
            int newObjectID = e.Params["NewObjectID"].Get<int>();
            string newObjectType = e.Params["NewObjectType"].Get<string>();

            Type parentType = GetType(parentTypeName);
            Type objectType = GetType(newObjectType);
            object parentObject = GetObject(parentType, parentId);
            object newChild = GetObject(objectType, newObjectID);
            MethodInfo getter = parentType.GetProperty(
                parentPropertyName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance)
                .GetGetMethod(true);
            object list = getter.Invoke(parentObject, null);
            list.GetType().GetMethod("Add").Invoke(list, new object[] { newChild });
            parentType.GetMethod("Save").Invoke(parentObject, null);
        }

        // Called when an object wants to nullify one of its complex properties
        [ActiveEvent(Name = "DBAdmin.RemoveReference")]
        protected void DBAdmin_RemoveReference(object sender, ActiveEventArgs e)
        {
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentType = e.Params["ParentFullType"].Get<string>();
            Type type = GetType(parentType);
            object obj = GetObject(type, parentId);
            MethodInfo method = type.GetProperty(
                parentPropertyName,
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public)
                .GetSetMethod(true);
            method.Invoke(obj, new object[] { null });
            type.GetMethod("Save").Invoke(obj, null);
        }

        // Called when in View Single Object one chooses to replace the current object
        // with another instance.
        [ActiveEvent(Name = "DBAdmin.ComplexInstanceRemoved")]
        protected void DBAdmin_ComplexInstanceRemoved(object sender, ActiveEventArgs e)
        {
            int parentId = e.Params["ParentID"].Get<int>();
            string parentPropertyName = e.Params["ParentPropertyName"].Get<string>();
            string parentTypeName = e.Params["ParentType"].Get<string>();
            int objectToRemove = e.Params["ObjectToRemoveID"].Get<int>();
            string objectToRemoveType = e.Params["ObjectToRemoveType"].Get<string>();

            Type parentType = GetType(parentTypeName);
            Type objectType = GetType(objectToRemoveType);
            object parentObject = GetObject(parentType, parentId);
            object childToRemove = GetObject(objectType, objectToRemove);
            MethodInfo getter = parentType.GetProperty(
                parentPropertyName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance)
                .GetGetMethod(true);
            object enumerable = getter.Invoke(parentObject, null);
            Type typeOfEnumerable = enumerable.GetType();
            typeOfEnumerable.GetMethod(
                "Remove",
                BindingFlags.Public |
                BindingFlags.Instance)
                .Invoke(
                    enumerable,
                    new object[] { childToRemove });
            parentType.GetMethod("Save").Invoke(parentObject, null);
        }

        // Called when in View Contents of Class one chooses to delete an object.
        [ActiveEvent(Name = "DBAdmin.ComplexInstanceDeleted")]
        protected void DBAdmin_ComplexInstanceDeleted(object sender, ActiveEventArgs e)
        {
            int objectToRemoveID = e.Params["ObjectToDeleteID"].Get<int>();
            string objectToRemoveType = e.Params["ObjectToDeleteType"].Get<string>();

            Type objectType = GetType(objectToRemoveType);
            object objectToRemove = GetObject(objectType, objectToRemoveID);
            objectType.GetMethod("Delete").Invoke(objectToRemove, null);
        }

        // UPDATE the ViewContents Form
        [ActiveEvent(Name = "DBAdmin.ConfigureColumns")]
        protected void DBAdmin_ConfigureColumns(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["Caption"].Value = "Configure columns";
            node["Columns"] = e.Params["Columns"];
            node["FullTypeName"].Value = e.Params["FullTypeName"].Value;
            string fullTypeName = 
                "DBAdmin.VisibleColumns." + 
                e.Params["FullTypeName"].Get<string>();
            foreach (Node idx in node["Columns"])
            {
                string idxSettingName = fullTypeName + ":" + idx["Name"].Get<string>();
                idx["Visible"].Value = Settings.Instance.Get(idxSettingName, true);
            }
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureColumns",
                "child",
                node);
        }

        // Changes visibility of a columns for all ListView mode views ...
        [ActiveEvent(Name = "DBAdmin.ChangeVisibilityOfColumn")]
        protected void DBAdmin_ChangeVisibilityOfColumn(object sender, ActiveEventArgs e)
        {
            string columnName = e.Params["ColumnName"].Get<string>();
            string fullTypeName = "DBAdmin.VisibleColumns." + e.Params["FullTypeName"].Get<string>();
            bool visible = e.Params["Visible"].Get<bool>();
            string settingName = fullTypeName + ":" + columnName;
            Settings.Instance.Set(settingName, visible);
        }
    }
}
