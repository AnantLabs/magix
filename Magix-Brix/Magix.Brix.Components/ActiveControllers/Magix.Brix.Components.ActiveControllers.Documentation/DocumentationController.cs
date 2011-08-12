/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Doxygen.NET;
using System.Reflection;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveControllers.Documentation
{
    [ActiveController]
    public class DocumentationController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Admin"]["Caption"].Value = "Admin";

            e.Params["Items"]["Admin"]["Items"]["DoxDotNet"]["Caption"].Value = "Doxygent.NET ...";
            e.Params["Items"]["Admin"]["Items"]["DoxDotNet"]["Event"]["Name"].Value = "Magix.MetaType.ViewDoxygenFiles";
        }

        [ActiveEvent(Name = "Magix.MetaType.ViewDoxygenFiles")]
        protected void Magix_MetaType_ViewDoxygenFiles(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            foreach (Namespace idx in Docs.Namespaces)
            {
                Node outerMostNamespace = AddNamespaceToNode(idx, node);
                foreach (Class idxC in idx.Classes)
                {
                    AddClassToNode(idxC, outerMostNamespace);
                }
            }

            node["Width"].Value = 9;
            node["MarginBottom"].Value = 10;
            node["Top"].Value = 1;
            node["Last"].Value = false;
            node["FullTypeName"].Value = typeof(Namespace).FullName;
            node["ItemSelectedEvent"].Value = "Magix.Doxygen.ViewNamespaceOrClass";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Tree",
                "content3",
                node);
        }

        private void AddDelegateToNode(Doxygen.NET.Delegate cls, Node outerMostNamespace)
        {
            outerMostNamespace["Items"][cls.FullName]["ID"].Value = cls.FullName.Replace(".", "_XX_").Replace("+", "_XQ_");
            outerMostNamespace["Items"][cls.FullName]["Name"].Value = cls.Name;
            outerMostNamespace["Items"][cls.FullName]["ToolTip"].Value = cls.FullName;
            outerMostNamespace["Items"][cls.FullName]["CssClass"].Value = "tree-item-delegate";
        }

        [ActiveEvent(Name = "Magix.Doxygen.ViewNamespaceOrClass")]
        protected void Magix_Doxygen_ViewNamespaceOrClass(object sender, ActiveEventArgs e)
        {
            string fullName = e.Params["SelectedItemID"].Get<string>().Replace("_XX_", ".").Replace("_XQ_", "+");


            Namespace nSpc = Docs.Namespaces.Find(
                delegate(Namespace idx)
                {
                    return idx.FullName == fullName;
                });
            if(nSpc != null)
            {
                // Namespace
                ShowNamespaceInfo(nSpc);
            }
            else
            {
                // Class 
                Class cls = Docs.GetTypeByName(fullName) as Class;
                ShowClassInfo(cls);
            }
        }

        private void ShowClassInfo(Class cls)
        {
        }

        private void ShowNamespaceInfo(Namespace nSpc)
        {
            Node node = new Node();

            node["IsDelete"].Value = false;
            node["NoIdColumn"].Value = true;
            node["Container"].Value = "content4";
            node["Width"].Value = 9;
            node["MarginBottom"].Value = 10;
            node["Top"].Value = 1;
            node["Last"].Value = true;
            node["IsCreate"].Value = false;
            node["ReuseNode"].Value = true;

            node["FullTypeName"].Value = typeof(DocumentationController).FullName + "-META";

            node["Namespace"].Value = nSpc.FullName;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        private void AddClassToNode(Class cls, Node outerMostNamespace)
        {
            outerMostNamespace["Items"][cls.FullName]["ID"].Value = cls.FullName.Replace(".", "_XX_").Replace("+", "_XQ_");
            outerMostNamespace["Items"][cls.FullName]["Name"].Value = cls.Name;
            outerMostNamespace["Items"][cls.FullName]["ToolTip"].Value = cls.FullName;
            outerMostNamespace["Items"][cls.FullName]["CssClass"].Value = "tree-item-class";
        }

        private Node AddNamespaceToNode(Namespace idx, Node node)
        {
            string fullName = idx.FullName;
            string[] splits = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Node tmp = node;
            string tmpFullNs = "";
            while (splits.Length > 0)
            {
                string currentNamespace = splits[0];
                if (!string.IsNullOrEmpty(tmpFullNs))
                    tmpFullNs += ".";
                tmpFullNs += currentNamespace;

                tmp = tmp["Items"][currentNamespace];

                AddMicroNamespaceToNode(
                    idx,
                    currentNamespace, 
                    tmp, 
                    tmpFullNs, 
                    fullName.Substring(currentNamespace.Length).Length == 0);

                fullName = fullName.Substring(currentNamespace.Length);

                if (fullName.Length == 0)
                    break;
                else 
                    fullName = fullName.Substring(1);

                splits = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return tmp;
        }

        private void AddMicroNamespaceToNode(
            Namespace nameSp,
            string currentNamespace, 
            Node node, 
            string fullName, 
            bool last)
        {
            node["ID"].Value = fullName.Replace(".", "_XX_").Replace("+", "_XQ_");
            node["Name"].Value = currentNamespace;
            if (last)
            {
                if (!string.IsNullOrEmpty(nameSp.Description))
                    node["ToolTip"].Value = nameSp.Description;
                else
                    node["ToolTip"].Value = fullName;
            }
            else
                node["ToolTip"].Value = fullName;
            node["CssClass"].Value = "tree-item-namespace";
        }

        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == 
                typeof(DocumentationController).FullName + "-META")
            {
                e.Params["WhiteListColumns"]["Name"].Value = true;
                e.Params["WhiteListColumns"]["ForcedWidth"].Value = 9;
                e.Params["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
                e.Params["Type"]["Properties"]["Name"]["Header"].Value = "File";
                e.Params["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
            }
        }

        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == 
                typeof(DocumentationController).FullName + "-META")
            {
                List<Assembly> asm = new List<Assembly>();
                foreach (Assembly idx in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (idx.FullName.StartsWith("System") ||
                        idx.FullName.Contains("mscorlib") ||
                        idx.FullName.StartsWith("Microsoft"))
                        continue;
                    foreach (System.Type idxT in idx.GetTypes())
                    {
                        if (idxT.FullName.StartsWith("Microsoft"))
                            continue;
                        if (idxT.Namespace == null)
                            continue;
                        if (idxT.Namespace.Contains(e.Params["Namespace"].Get<string>()))
                        {
                            if (!asm.Exists(
                                delegate(Assembly idxA)
                                {
                                    return idxA.FullName == idxT.Namespace;
                                }))
                                asm.Add(idx);
                        }
                    }
                }
                int idxNo = 1;
                foreach (Assembly idx in asm)
                {
                    e.Params["Objects"]["o-" + idx.FullName]["ID"].Value = idxNo;
                    e.Params["Objects"]["o-" + idx.FullName]["Properties"]["Name"].Value = idx.FullName.Split(',')[0] + ".dll";
                    e.Params["Objects"]["o-" + idx.FullName]["Properties"]["Name"]["ToolTip"].Value = idx.FullName;
                    idxNo += 1;
                }
            }
        }

        public Docs Docs
        {
            get
            {
                // Loading it ONCE and caching it for ALL users. VERY expensive operation ...!
                if (Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] == null)
                {
                    lock (typeof(DocumentationController))
                    {
                        if (Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] == null)
                        {
                            Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] =
                                new Docs(Page.MapPath("~/DoxFiles"));
                        }
                    }
                }
                return (Docs)Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"];
            }
        }
    }
}
