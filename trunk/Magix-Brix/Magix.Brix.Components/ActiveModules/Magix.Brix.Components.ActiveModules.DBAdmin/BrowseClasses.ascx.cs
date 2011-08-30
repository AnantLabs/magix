/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    // TODO: Replace with CommonModules.Tree
    /**
     * Level2: Contains UI for letting the end user browse the ActiveType classes within his system as
     * a Database Enterprise Management tool. Allows for seeings all classes in a Tree hierarchy
     * and letting him select classes, which again will trigger editing of records in that class.
     * Kind of like the Database Anterprise Management tool for Magix. Will raise
     * 'DBAdmin.Data.GetClassHierarchy', which you can handle if you have 'meta types' you wish
     * to display
     */
    [ActiveModule]
    public class BrowseClasses : Module, IModule
    {
        protected Label header;
        protected TreeView tree;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    DataSource = new Node(); // Discards input entirely ...
                    RaiseSafeEvent(
                        "DBAdmin.Data.GetClassHierarchy",
                        DataSource);
                    if (node.Contains("Header"))
                        header.Text = node["Header"].Get<string>();
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
