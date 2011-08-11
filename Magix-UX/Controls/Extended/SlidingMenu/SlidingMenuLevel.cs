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
     * Child control of a SlidingMenuItem. Will contain the SlidingMenuItems of a SlidingMenuItem.
     * If you have items which have children, then you must create an item of
     * type SlidingMenuItem and add those children inside of that object. This is true
     * both for .ASPX markup and SlidingMenu hierarchies created in code.
     */
    public class SlidingMenuLevel : ViewCollectionControl<SlidingMenuItem>
    {
        public SlidingMenuLevel()
        {
            CssClass = "mux-sliding-menu-level";
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
