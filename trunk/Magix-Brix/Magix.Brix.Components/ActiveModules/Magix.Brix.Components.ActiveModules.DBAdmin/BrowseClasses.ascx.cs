/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class BrowseClasses : System.Web.UI.UserControl, IModule
    {
        protected TreeView tree;
        protected Window wnd;
        protected DynamicPanel child;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    Node tmp = new Node();
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "DBAdmin.BrowseClassHierarchy",
                        tmp);
                    DataBase = tmp;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataBase != null)
                DataBindWholeTree();
        }

        private void DataBindWholeTree()
        {
            DataBindTree(DataBase["Classes"], tree);
        }

        private Node DataBase
        {
            get { return ViewState["DataBase"] as Node; }
            set { ViewState["DataBase"] = value; }
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
                // Showing the ViewContens Form
                Node node = new Node();
                node["FullTypeName"].Value = classFullName.Replace("Leaf:", "");
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "DBAdmin.ViewContents",
                    node);
            }
        }
    }
}




















