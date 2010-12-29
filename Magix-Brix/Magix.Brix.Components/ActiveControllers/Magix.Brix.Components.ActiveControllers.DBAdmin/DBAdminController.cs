﻿/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.DBAdmin
{
    [ActiveController]
    public class DBAdminController : ActiveController
    {
        [ActiveEvent(Name = "LoadDbAdmin")]
        protected void LoadDbAdmin(object sender, ActiveEventArgs e)
        {
            LoadModule("Magix.Brix.Components.ActiveModules.DBAdmin.Main");
        }

        [ActiveEvent(Name = "GetDBAdminClasses")]
        protected void GetDBAdminClasses(object sender, ActiveEventArgs e)
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

        [ActiveEvent(Name = "ViewClassDetails")]
        protected void ViewClassDetails(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            List<Type> types= new List<Type>(PluginLoader.Instance.ActiveTypes);
            Type type = types.Find(
                delegate(Type idx)
                {
                    return idx.FullName == e.Params["ClassName"].Get<string>();
                });
            node["Caption"].Value = "Details for ActiveType: " + type.Name;
            int no = (int)type.GetProperty(
                "Count",
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.NonPublic)
                .GetGetMethod()
                .Invoke(null, null);
            node["Count"].Value = no;
            node["FullTypeName"].Value = type.FullName;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassDetails",
                "popup",
                node);
        }

        [ActiveEvent(Name = "ViewAllInstances")]
        protected void ViewAllInstances(object sender, ActiveEventArgs e)
        {
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            List<Type> types = new List<Type>(PluginLoader.Instance.ActiveTypes);
            Type type = types.Find(
                delegate(Type idx)
                {
                    return idx.FullName == fullTypeName;
                });
            PropertyInfo[] props = type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
            Dictionary<string, MethodInfo> getters = new Dictionary<string, MethodInfo>();
            foreach (PropertyInfo idx in props)
            {
                ActiveFieldAttribute[] attrs = idx.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    // Serializable property...
                    getters[idx.Name] = idx.GetGetMethod(true);
                }
            }
            MethodInfo retrieveAllObjects = type.GetMethod(
                "Select",
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.NonPublic);
            Criteria[] pars = new Criteria[0];

            Node node = new Node();

            foreach (object idxObj in 
                retrieveAllObjects.Invoke(
                    null, 
                    new object[] { pars }) as System.Collections.IEnumerable)
            {
                int id = (int)type.GetProperty(
                    "ID",
                    BindingFlags.Instance |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)
                    .GetGetMethod(true)
                    .Invoke(idxObj, null);
                node["Objects"]["Object" + id]["ID"].Value = id;
                foreach (string idxMethodName in getters.Keys)
                {
                    object value = getters[idxMethodName].Invoke(idxObj, null);
                    if (getters[idxMethodName].ReturnType.FullName.IndexOf("Magix.Brix.Types.LazyList") != -1)
                    {
                        int noItems = (int)getters[idxMethodName].ReturnType.GetProperty(
                            "Count",
                            BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy |
                            BindingFlags.Public |
                            BindingFlags.NonPublic)
                            .GetGetMethod()
                            .Invoke(value, null);
                        node["Objects"]["Object" + id][idxMethodName]["Value"].Value = noItems;
                        node["Objects"]["Object" + id][idxMethodName]["Name"].Value = "LazyList&lt;" +
                            getters[idxMethodName].ReturnType.GetGenericArguments()[0].Name + "&gt;";
                    }
                    else if (getters[idxMethodName].ReturnType.FullName.IndexOf("System.Collections.Generic.List") != -1)
                    {
                        int noItems = (int)getters[idxMethodName].ReturnType.GetProperty(
                            "Count",
                            BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy |
                            BindingFlags.Public |
                            BindingFlags.NonPublic)
                            .GetGetMethod()
                            .Invoke(value, null);
                        node["Objects"]["Object" + id][idxMethodName]["Value"].Value = noItems;
                    }
                    else
                    {
                        if (value == null)
                            node["Objects"]["Object" + id][idxMethodName]["Value"].Value = "[null]";
                        else
                            node["Objects"]["Object" + id][idxMethodName]["Value"].Value = value.ToString();
                        node["Objects"]["Object" + id][idxMethodName]["Name"].Value =
                            getters[idxMethodName].ReturnType.Name;
                    }
                    node["Objects"]["Object" + id][idxMethodName]["FullName"].Value =
                        getters[idxMethodName].ReturnType.FullName;
                    node["Objects"]["Object" + id][idxMethodName]["PropertyName"].Value = idxMethodName;
                }
            }
            node["GrowX"].Value = 950;
            node["GrowY"].Value = 550;
            node["Caption"].Value = "Contents of ActiveType: " + type.FullName;
            LoadModule(
                "Magix.Brix.Components.ActiveModules.DBAdmin.ViewInstances",
                "popup",
                node);
        }
    }
}
