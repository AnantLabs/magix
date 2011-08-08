/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * Menu control. A Menu is basically a collection of MenuItems. Mimicks
     * a Menu the way you're used to seeing them on desktop systems, or at other
     * web applications. The Magix UX menu have support for any number of child
     * menus, and are very customizable in regards to how it looks and behaves.
     */
    public class Menu : ViewCollectionControl<MenuItem>
    {
        /**
         * What to trigger the expansion. You can select Click, Hover or DblClick
         * to be the 'trigger' that expands the expansion of your menu.
         */
        public enum ExpansionTrigger
        {
            /**
             * Click will trigger the expansion of the item
             */
            Click,

            /**
             * MouseOver will trigger the expansion of the item
             */
            Hover,

            /**
             * Double-click will trigger the expansion of the item
             */
            DblClick
        };

        /**
         * Raised when a Leaf MenuItem, meaning a MenuItem with noe children
         * have been clicked. Normally these are the MenuItems you need to handle
         * on the server, while the ones with children are basically just 'groupings'
         * of other MenuItems.
         */
        public event EventHandler LeafMenuItemClicked;

        /**
         * Raised when any MenuItem have been clicked.
         */
        public event EventHandler MenuItemClicked;

        public Menu()
        {
            CssClass = "mux-menu";
            Tag = "ul";
        }

        /**
         * How to animate items when opened, or what to trigger the opening of the item.
         * Your choises here are Click, DblClick and Hover.
         */
        public ExpansionTrigger ExpansionMode
        {
            get { return ViewState["ExpansionMode"] == null ? ExpansionTrigger.Click : (ExpansionTrigger)ViewState["ExpansionMode"]; }
            set { ViewState["ExpansionMode"] = value; }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }

        internal bool HasLeafMenuItemClickedEventHandler
        {
            get { return LeafMenuItemClicked != null; }
        }

        internal bool HasMenuItemClickedEventHandler
        {
            get { return MenuItemClicked != null; }
        }

        internal void RaiseClickedEventHandler(MenuItem item)
        {
            if (LeafMenuItemClicked != null)
            {
                LeafMenuItemClicked(item, new EventArgs());
            }
        }

        internal void RaiseClickedNoLeafEventHandler(MenuItem item)
        {
            if (MenuItemClicked != null)
            {
                MenuItemClicked(item, new EventArgs());
            }
        }
    }
}
