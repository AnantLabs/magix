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
     * A single sliding menu item. One instance. A SlidingMenu are basically composed out
     * of a whole hierarchy of these types of widgets.
     */
    public class SlidingMenuItem : Panel
    {
        readonly LinkButton _text = new LinkButton();
        readonly Label _icon = new Label();

        public SlidingMenuItem()
        {
            CssClass = "mux-sliding-menu-item";
            Tag = "li";
        }

        /**
         * The (visible) text of your menu item.
         */
        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; }
        }

        /**
         * The Keyboard Shortcut key for the Widget
         */
        public string AccessKey
        {
            get { return _text.AccessKey; }
            set { _text.AccessKey = value; }
        }

        /**
         * If you give this property a value, the item will display as a hyperlink,
         * and the URL property will be what it links to. The Text property will become
         * the anchor text of the URL.
         */
        public string URL
        {
            get { return ViewState["URL"] == null ? "" : (string)ViewState["URL"]; }
            set { ViewState["URL"] = value; }
        }

        /**
         * Will return the SlidingMenuLevel of the MenuItem, if any exists. Every item
         * that have child items, should create a SlidingMenuLevel and put its sub 
         * items within the SlidingMenuLevel portion of their markup, or instantiate
         * them within the SlidingMenuLevel child control of those items, if the items
         * are being added programmatically.
         */
        public SlidingMenuLevel SlidingMenuLevel
        {
            get
            {
                foreach (Control idx in Controls)
                {
                    SlidingMenuLevel lev = idx as SlidingMenuLevel;
                    if (lev != null)
                        return lev;
                }
                return null;
            }
        }

        /**
         * Will return the parent SlidingMenu of this item. This property will traverse
         * upwards in the ancestor hierarchy and find the first SlidingMenu instance, being
         * an ancestor of this particular item and return that object.
         */
        private SlidingMenu SlidingMenu
        {
            get
            {
                Control idx = this.Parent;
                while (!(idx is SlidingMenu))
                {
                    idx = idx.Parent;
                }
                return idx as SlidingMenu;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            CreateCompositeControl();
        }

        private void CreateCompositeControl()
        {
            _text.ID = "txt";
            _text.CssClass = "mux-sliding-menu-item-text";
            _icon.ID = "icon";
            _icon.Text = "&gt;";
            _icon.CssClass = "mux-sliding-menu-item-icon";
            if (SlidingMenuLevel != null)
                SlidingMenuLevel.Style[Styles.display] = "none";

            if (SlidingMenu.SlideOnIcon)
            {
                _icon.Click += MenuItemClicked;
                _text.Click += MenuItemClicked2;
            }
            else
            {
                _text.Click += MenuItemClicked;
            }
            
            _text.Controls.Add(_icon);
            Controls.AddAt(0, _text);
        }

        protected void MenuItemClicked(object sender, EventArgs e)
        {
            // Raising event handler for item clicked...
            SlidingMenu.RaiseMenuItemClicked(this, SlidingMenuLevel == null ? null : SlidingMenuLevel);
        }

        protected void MenuItemClicked2(object sender, EventArgs e)
        {
            SlidingMenu.noSlide = true;
            // Raising event handler for item clicked...
            SlidingMenu.RaiseMenuItemClicked(this, null);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SlidingMenuLevel == null)
            {
                if (CssClass.Contains(" mux-sliding-menu-has-children"))
                {
                    CssClass = CssClass.Replace(" mux-sliding-menu-has-children", "");
                }
            }
            else
            {
                // This bugger has children...
                if (!CssClass.Contains(" mux-sliding-menu-has-children"))
                {
                    CssClass += " mux-sliding-menu-has-children";
                }
            }
            if (SlidingMenu.NoEventHandlerDefined)
            {
                bool shouldRemove = true;
                if (!SlidingMenu.NoEmptyEventHandlerDefined)
                    shouldRemove = SlidingMenuLevel != null;
                if (shouldRemove)
                {
                    _text.Click -= MenuItemClicked;
                    if (SlidingMenuLevel != null)
                    {
                        // Client-side effects...
                        int noLevels = 0;
                        Control tmp = Parent;
                        while (tmp.ID != SlidingMenu.ID)
                        {
                            if (tmp is SlidingMenuLevel)
                                noLevels += 1;
                            tmp = tmp.Parent;
                        }
                        _text.ClickEffect =
                            new EffectSlide(
                                    SlidingMenu.SlidingMenuLevel,
                                    500,
                                    -noLevels)
                                .JoinThese(
                                    new EffectFadeIn(
                                            SlidingMenuLevel,
                                            500));
                    }
                }
            }
            if (!string.IsNullOrEmpty(URL))
            {
                _text.Visible = false;
            }
            if (SlidingMenuLevel == null)
            {
                _icon.Visible = false;
            }
            else
            {
                _icon.Visible = true;
            }
            base.OnPreRender(e);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }
    }
}
