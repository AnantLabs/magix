/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using Magix.Brix.Loader;
using Magix.Brix.Types;

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
                //int no = (int)idx.GetProperty(
                //    "Count",
                //    BindingFlags.Static |
                //    BindingFlags.FlattenHierarchy |
                //    BindingFlags.Public |
                //    BindingFlags.NonPublic)
                //    .GetGetMethod()
                //    .Invoke(null, null);
                //node["Count"].Value = no;
                idxNo += 1;
            }
        }
    }
}
