/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using System.Collections.Generic;

// TODO: Use TreeView.ascx in common modules instead ...
namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    public class TreeViewOfPages : ActiveModule, IModule
    {
        protected TreeView tree;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load += delegate
            {
                if (node.Contains("TreeCssClass"))
                    tree.CssClass += " " + node["TreeCssClass"].Get<string>();
                if (node.Contains("NoClose"))
                    tree.NoCollapseOfItems = node["NoClose"].Get<bool>();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateTreeNodes();
        }

        private void CreateTreeNodes()
        {
            if (DataSource == null || !DataSource.Contains("Pages"))
                return;

            foreach (Node idx in DataSource["Pages"])
            {
                CreateSingleItem(idx, tree);
            }
        }

        private void CreateSingleItem(Node idx, Control ixC)
        {
            TreeItem i = new TreeItem();

            i.Text = idx["Caption"].Get<string>();
            i.Info = idx["ID"].Get<int>().ToString();
            i.ID = "i" + idx["ID"].Get<int>().ToString();

            if (idx.Contains("Pages"))
            {
                foreach (Node idxChild in idx["Pages"])
                {
                    CreateSingleItem(idxChild, i);
                }
            }

            if (ixC is TreeView)
                ixC.Controls.Add(i);
            else
                (ixC as TreeItem).Content.Controls.Add(i);
        }

        protected void tree_SelectedItemChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = int.Parse(tree.SelectedItem.Info);

            RaiseEvent(
                "Magix.Publishing.EditSpecificPage",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.PageWasDeleted")]
        protected void Magix_Publishing_PageWasDeleted(object sender, ActiveEventArgs e)
        {
            ReDataBind();
        }

        [ActiveEvent(Name = "Magix.Core.SetTreeSelectedID")]
        protected void Magix_Core_SetTreeSelectedID(object sender, ActiveEventArgs e)
        {
            ChangeTreeViewSelectedNode(e.Params);
        }

        private void ChangeTreeViewSelectedNode(Node node)
        {
            tree.SelectedItem.Expanded = true;
            tree.SelectedItem.CssClass +=
                tree.SelectedItem.CssClass.Replace(" mux-tree-collapsed", " mux-tree-expanded");
            tree.SelectedItem.CssClass =
                tree.SelectedItem.CssClass.Replace(" mux-tree-selected", "");

            tree.SelectedItem = Selector.FindControl<TreeItem>(tree, "i" + node["ID"].Get<int>());

            tree.SelectedItem.CssClass += " mux-tree-selected";
        }

        [ActiveEvent(Name = "Magix.Publishing.PageWasUpdated")]
        protected void Magix_Publishing_PageWasUpdated(object sender, ActiveEventArgs e)
        {
            ReDataBind();
        }

        private void ReDataBind()
        {
            // Refetching items ...
            DataSource["Pages"].UnTie();

            RaiseEvent(
                "Magix.Publishing.GetEditPagesDataSource",
                DataSource);

            // Storing previously expanded items ...
            List<string> expandedItems = new List<string>();
            foreach (TreeItem idx in Selector.Select<TreeItem>(
                tree,
                delegate(Control idx)
                {
                    TreeItem x = idx as TreeItem;
                    if (x == null)
                        return false;
                    return x.Expanded;
                }))
            {
                expandedItems.Add(idx.ID);
            }

            tree.Controls.Clear();
            CreateTreeNodes();
            tree.ReRender();

            // Expanding back to previously selected item ...
            // Unfortunately we've got to do some 'handtracking' here to keep the expended treenode expanded
            // and selected since the TreeView is re-rendered, and some of the state hence becomes fucked up ...
            TreeItem it = tree.SelectedItem;
            if (it != null)
            {
                it.CssClass += " mux-tree-selected";
            }
            foreach (string idx in expandedItems)
            {
                TreeItem ti = Selector.FindControl<TreeItem>(tree, idx);
                if (ti != null)
                {
                    if (ti.Content.Controls.Count > 0)
                    {
                        ti.Expanded = true;
                        ti.CssClass = ti.CssClass.Replace(" mux-tree-collapsed", " mux-tree-expanded");
                    }
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.CreateChild")]
        protected void Magix_Publishing_CreateChild(object sender, ActiveEventArgs e)
        {
            // It really kind of 'have' to be the currently selected one, so we're shortcuting
            // a little bit here ...
            tree.SelectedItem.Expanded = true;
            tree.SelectedItem.CssClass = tree.SelectedItem.CssClass.Replace(" mux-tree-collapsed", " mux-tree-expanded");
        }
    }
}



