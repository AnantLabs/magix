/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class BrowseClasses : Module, IModule
    {
        protected TreeView tree;
        protected Window wnd;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    Node tmp = new Node();
                    RaiseSafeEvent(
                        "DBAdmin.Data.GetClassHierarchy",
                        tmp);
                    DataSource = tmp;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindWholeTree();
        }

        private void DataBindWholeTree()
        {
            DataBindTree(DataSource["Classes"], tree);
        }

        private void DataBindTree(Node tmp, System.Web.UI.Control ctrl)
        {
            foreach (Node idx in tmp)
            {
                if (idx["FullTypeName"].Value == null)
                    continue;
                TreeItem it = new TreeItem();
                it.ID = idx["FullTypeName"].Get<string>().Replace(".", "").Replace("+", "");
                if (idx["IsLeafClass"].Get<bool>())
                {
                    it.Info = "Leaf:" + idx["FullTypeName"].Get<string>();
                    it.CssClass += " tree-item-class";
                }
                else
                {
                    it.Info = idx["FullTypeName"].Get<string>();
                    it.CssClass += " tree-item-namespace";
                }
                it.Text = idx["Name"].Get<string>();
                DataBindTree(idx, it);
                if (ctrl is TreeItem)
                {
                    (ctrl as TreeItem).Content.Controls.Add(it);
                }
                else
                {
                    ctrl.Controls.Add(it);
                }
            }
        }

        protected void tree_SelectedItemChanged(object sender, EventArgs e)
        {
            TreeItem it = sender as TreeItem;
            string classFullName = it.Info;
            if (classFullName.IndexOf("Leaf:") == 0)
            {
                // Class ...!
                // Showing the ViewClassContens Form
                Node node = new Node();
                node["FullTypeName"].Value = classFullName.Replace("Leaf:", "");
                RaiseSafeEvent(
                    "DBAdmin.Form.ViewClass",
                    node);
            }
        }

        protected override void ReDataBind()
        {
        }
    }
}

