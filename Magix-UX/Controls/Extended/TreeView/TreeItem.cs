/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * One single item within a TreeView widget. A TreeView is basically nothing but
     * a collection of these types of controls. If you override its CSS class, you can
     * give your TreeItems specific icons and such, in which case you need to change 
     * the CSS background-image for those specific MenuItems, which really is out
     * of scope of this article.
     */
    public class TreeItem : CompositeViewCollectionControl<TreeItem>
    {
        private Label _text = new Label();
        private HyperLink _link = new HyperLink();
        private Label _floater = new Label();

        public TreeItem()
        {
            CssClass = "mux-tree-item";
            Tag = "li";
        }

        /**
         * The (visible) text of your TreeItem.
         */
        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; _link.Text = value; }
        }

        /**
         * If you give this property a value, the item will display as a hyperlink,
         * and the URL property will be what it links to. The Text property will become
         * the anchor text of the URL.
         */
        public string URL
        {
            get { return _link.URL; }
            set { _link.URL = value; }
        }

        /**
         * True if this particular TreeItem is actually expanded.
         */
        public bool Expanded
        {
            get { return Content.Style[Styles.display] != "none"; }
            set { Content.Style[Styles.display] = (value ? "" : "none"); }
        }

        /**
         * Will return the parent TreeView of this item. This property will traverse
         * upwards in the ancestor hierarchy and find the first TreeView instance, being
         * an ancestor of this particular item and return that object.
         */
        public TreeView TreeView
        {
            get
            {
                Control idx = this.Parent;
                while (!(idx is TreeView))
                {
                    idx = idx.Parent;
                }
                return idx as TreeView;
            }
        }

        private int NoLevels
        {
            get
            {
                Control idx = this.Parent;
                int noLevels = 0;
                while (!(idx is TreeView))
                {
                    if (idx is TreeItem)
                        noLevels += 1;
                    idx = idx.Parent;
                }
                return noLevels;
            }
        }

        private TreeItem ParentTreeItem
        {
            get { return Parent.Parent as TreeItem; }
        }

        private bool IsLast
        {
            get
            {
                bool foundThis = false;
                foreach (Control idx in this.Parent.Controls)
                {
                    if (foundThis && idx is TreeItem)
                        return false;
                    else if (idx.ID == ID)
                        foundThis = true;
                }
                return true;
            }
        }

        private bool ContentHasTreeItems
        {
            get
            {
                foreach (Control idx in Content.Controls)
                {
                    if (idx is TreeItem)
                        return true;
                }
                return false;
            }
        }

        protected override void CreateCompositeControl()
        {
            _text.ID = "txt";
            _text.CssClass = "mux-tree-text";
            Controls.Add(_text);

            _link.ID = "lnk";
            _link.CssClass = "mux-tree-url";
            Controls.Add(_link);

            _floater.ID = "flt";
            _floater.CssClass = "mux-tree-floater";
            if (TreeView.HasSelectedItemChangedEventHandler)
            {
                _floater.Click += ItemClicked;
            }
            Controls.Add(_floater);

            Content.CssClass = "mux-tree-level";
            Content.Tag = "ul";
            if (Content != null && Content.Style[Styles.display] == null)
                Content.Style[Styles.display] = "none";

            SetVisibility();
            base.CreateCompositeControl();
        }

        private void ItemClicked(object sender, EventArgs e)
        {
            if (ContentHasTreeItems)
            {
                Content.Visible = true;
                if (!TreeView.NoCollapseOfItems ||
                    !Expanded)
                {
                    new EffectToggle(Content, 500)
                        .JoinThese(
                            new EffectCssClass(this, "mux-tree-collapsed"),
                            new EffectCssClass(this, "mux-tree-expanded"))
                        .Render();
                }
            }
            TreeItem old = TreeView.SelectedItem;
            if (old != null)
            {
                new EffectCssClass(old, "mux-tree-selected")
                    .Render();
            }
            new EffectCssClass(this, "mux-tree-selected")
                .Render();
            TreeView.RaiseSelectedItemChangedEvent(this);
        }

        private void SetVisibility()
        {
            _text.Visible = string.IsNullOrEmpty(URL);
            _link.Visible = !_text.Visible;

            _floater.MouseOverEffect = new EffectCssClass(this, "mux-tree-hover");
            _floater.MouseOutEffect = new EffectCssClass(this, "mux-tree-hover");
            if (ContentHasTreeItems)
            {
                Content.Visible = true;
                if (!TreeView.HasSelectedItemChangedEventHandler)
                {
                    _floater.ClickEffect =
                        new EffectToggle(Content, 500)
                            .JoinThese(
                                new EffectCssClass(this, "mux-tree-collapsed"),
                                new EffectCssClass(this, "mux-tree-expanded"));
                }
            }
            else
            {
                Content.Visible = false;
            }
            if (Expanded)
            {
                if (!CssClass.Contains("mux-tree-expanded"))
                {
                    CssClass += " mux-tree-expanded";
                }
                if (CssClass.Contains(" mux-tree-collapsed"))
                {
                    CssClass = CssClass.Replace(" mux-tree-collapsed", "");
                }
            }
            else
            {
                if (!CssClass.Contains("mux-tree-collapsed"))
                {
                    CssClass += " mux-tree-collapsed";
                }
                if (CssClass.Contains(" mux-tree-item-expanded"))
                {
                    CssClass = CssClass.Replace(" mux-tree-item-expanded", "");
                }
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);

                // One additional level for the icon and another additional level for the first lines...
                int idxNoSpacers = NoLevels + 2;
                while (idxNoSpacers-- != 0)
                {
                    using (Element spacer = builder.CreateElement("span"))
                    {
                        if (idxNoSpacers == 0)
                        {
                            // Last spacer, which is our icon
                            spacer.AddAttribute("class", "mux-tree-icon");
                        }
                        else if (idxNoSpacers == 1)
                        {
                            // Second last one, here is our plus/minus sign - if any...
                            if (ContentHasTreeItems)
                            {
                                // This ones has children
                                if (IsLast)
                                    spacer.AddAttribute("class", "mux-tree-children-last");
                                else
                                    spacer.AddAttribute("class", "mux-tree-children");
                            }
                            else
                            {
                                // This one doesn't have children
                                if (IsLast)
                                    spacer.AddAttribute("class", "mux-tree-no-children-last");
                                else
                                    spacer.AddAttribute("class", "mux-tree-no-children");
                            }
                        }
                        else
                        {
                            // Not an Icon and not the plus / second last one
                            bool isSpace = true;
                            TreeItem itemIdx = this;
                            for (int idxNo = 0; idxNo < idxNoSpacers; idxNo++)
                            {
                                if (itemIdx == null)
                                    break;
                                isSpace = itemIdx.IsLast;
                                itemIdx = itemIdx.ParentTreeItem;
                            }
                            // Not the last nor the second last one, easy case...
                            if (isSpace)
                                spacer.AddAttribute("class", "mux-tree-space");
                            else
                                spacer.AddAttribute("class", "mux-tree-link");
                        }
                    }
                }
                _text.RenderControl(builder.Writer);
                _link.RenderControl(builder.Writer);
                _floater.RenderControl(builder.Writer);
                Content.RenderControl(builder.Writer);
            }
        }
    }
}
