/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * This widget makes it possible for you to create selector kind of widgets
     * that mimicks the representation of a tree kind-of hierarchical structure
     * for displaying things. Useful for displaying for instance folder structures
     * and such.
     */
    public class TreeView : ViewCollectionControl<TreeItem>
    {
        /**
         * Raised when a new item was selected. Use the SelectedItem property to
         * see which item was actually selected.
         */
        public event EventHandler SelectedItemChanged;

        public TreeView()
        {
            CssClass = "mux-tree-view";
        }

        /**
         * Returns or sets the currently selected item.
         */
        public TreeItem SelectedItem
        {
            get
            {
                return ViewState["SelectedItem"] == null 
                    ? null : 
                    Selector.FindControl<TreeItem>(this, (string)ViewState["SelectedItem"]);
            }
            set
            {
                if (value == null)
                    ViewState["SelectedItem"] = null;
                else
                    ViewState["SelectedItem"] = value.ID;
            }
        }

        public bool NoCollapseOfItems
        {
            get { return ViewState["NoCollapseOfItems"] == null ? false : (bool)ViewState["NoCollapseOfItems"]; }
            set { ViewState["NoCollapseOfItems"] = value; }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                using (Element children = builder.CreateElement("ul"))
                {
                    children.AddAttribute("class", "mux-tree-level");
                    RenderChildren(builder.Writer);
                }
            }
        }

        internal void RaiseSelectedItemChangedEvent(TreeItem item)
        {
            SelectedItem = item;
            if (SelectedItemChanged != null)
                SelectedItemChanged(item, new EventArgs());
        }

        internal bool HasSelectedItemChangedEventHandler
        {
            get { return SelectedItemChanged != null; }
        }
    }
}
