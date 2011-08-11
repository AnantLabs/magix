/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * Child control of a MenuItem. Will contain the SubMenuItems of a MenuItem.
     * If you have MenuItems which have children, then you must create an item of
     * type SubMenu and add those children inside of that object. This is true
     * both for .ASPX markup and Menu hierarchies created in code.
     */
    public class SubMenu : ViewCollectionControl<MenuItem>
    {
        public SubMenu()
        {
            CssClass = "mux-menu-submenu";
            Tag = "ul";
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
