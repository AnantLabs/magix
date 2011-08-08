/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Effects;

namespace Magix.UX.Widgets
{
    /**
     * Items within a Menu. A MenuItem can have a Text property in addition to a SubMenu, 
     * which if defined will popup as a child of the MenuItem. The MenuItem class is 
     * basically the individual items of your menu.
     */
    public class MenuItem : Panel
    {
        private Label _text = new Label();
        private HyperLink _link = new HyperLink();
        private Label _icon = new Label();

        public MenuItem()
        {
            CssClass = "mux-menu-item";
            Tag = "li";
        }

        /**
         * The Caption of the MenuItem. This is the text displayed within your MenuItem.
         */
        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; _link.Text = value; }
        }

        /**
         * If you give this property a value, the MenuItem will display as a hyperlink,
         * and the URL property will be what it links to. The Text property will become
         * the anchor text of the URL.
         */
        public string URL
        {
            get { return _link.URL; }
            set { _link.URL = value; }
        }

        /**
         * Will return the SubMenu of the MenuItem, if any exists. Every MenuItem
         * that have child MenuItems, should create a SubMenu and put its sub 
         * MenuItems within the SubMenu portion of their markup, or instantiate
         * them within the SubMenu child control of those MenuItems, if the items
         * are being added programmatically.
         */
        public SubMenu SubMenu
        {
            get
            {
                foreach (Control idx in Controls)
                {
                    SubMenu retVal = idx as SubMenu;
                    if (retVal != null)
                        return retVal;
                }
                return null;
            }
        }

        /**
         * Will return the parent Menu of the MenuItem. This property will traverse
         * upwards in the ancestor hierarchy and find the first Menu instance, being
         * an ancestor of this particular MenuItem and return that object.
         */
        public Menu Menu
        {
            get
            {
                Control tmp = this.Parent;
                while (!(tmp is Menu))
                {
                    tmp = tmp.Parent;
                }
                return tmp as Menu;
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
            _icon.ID = "icon";
            _icon.CssClass = "mux-menu-item-icon";
            _icon.Text = "&gt;";
            Controls.AddAt(0, _icon);

            _text.ID = "capt";
            _text.CssClass = "mux-menu-item-text";
            Controls.AddAt(1, _text);

            _link.ID = "lnk";
            _link.CssClass = "mux-menu-item-url";
            Controls.AddAt(2, _link);

            if (SubMenu != null)
            {
                SubMenu.Style[Styles.display] = "none";
            }

            if (string.IsNullOrEmpty(URL) &&
                Menu.HasLeafMenuItemClickedEventHandler &&
                SubMenu == null)
            {
                Click += MenuItemClicked;
            }

            if (string.IsNullOrEmpty(URL) &&
                Menu.HasMenuItemClickedEventHandler)
            {
                Click += MenuItemClicked2;
            }
        }

        private void MenuItemClicked(object sender, EventArgs e)
        {
            Menu.RaiseClickedEventHandler(this);
            SubMenu sub = this.Parent as SubMenu;
            while (sub != null &&
                sub.Parent != null &&
                sub.Parent.Parent != null &&
                sub.Parent.Parent is SubMenu)
            {
                sub.Style[Styles.display] = "none";
                sub = sub.Parent.Parent as SubMenu;
            }
            if (sub != null)
            {
                sub.Style[Styles.display] = "block";
                new EffectRollUp(sub, 500, "visible")
                    .Render();
            }
        }

        private void MenuItemClicked2(object sender, EventArgs e)
        {
            Menu.RaiseClickedNoLeafEventHandler(this);
            SubMenu sub = this.Parent as SubMenu;
            while (sub != null &&
                sub.Parent != null &&
                sub.Parent.Parent != null &&
                sub.Parent.Parent is SubMenu)
            {
                sub.Style[Styles.display] = "none";
                sub = sub.Parent.Parent as SubMenu;
            }
            if (sub != null)
            {
                sub.Style[Styles.display] = "block";
                new EffectRollUp(sub, 500, "visible")
                    .Render();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            // Making sure either the link or the text is being made visible...
            _text.Visible = string.IsNullOrEmpty(URL);
            _link.Visible = !_text.Visible;

            if (SubMenu == null)
            {
                if (CssClass.Contains(" mux-menu-has-children"))
                {
                    CssClass = CssClass.Replace(" mux-menu-has-children", "");
                }
            }
            else
            {
                // This bugger has children...
                if (!CssClass.Contains(" mux-menu-has-children"))
                {
                    CssClass += " mux-menu-has-children";
                }
                Effect effect = new EffectRollDown(SubMenu, 500);
                effect.Condition =
                    string.Format(
                        "function(){{return MUX.$('{0}').getStyle('display') == 'none';}}", 
                        SubMenu.ClientID);
                switch (Menu.ExpansionMode)
                {
                    case Menu.ExpansionTrigger.Click:
                        ClickEffect = effect;
                        break;
                    case Menu.ExpansionTrigger.DblClick:
                        DblClickEffect = effect;
                        break;
                    case Menu.ExpansionTrigger.Hover:
                        MouseOverEffect = effect;
                        break;
                }
                MouseOutEffect = new EffectRollUp(SubMenu, 500, "visible");
                MouseOutEffect.Condition =
                    string.Format("function(){{return MUX.$('{0}').getStyle('display') != 'none';}}", 
                    SubMenu.ClientID);
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
