/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * A DropDown list type of control, although it can also be set into a non-drop 
     * down mode. Basically multiple choices type of widget. To some extent, it 
     * overlaps the logical functionality of the RadioButton widget, although the
     * SelectList is more useful for cases where you have a massive number of choices,
     * like for instance choose one out of 300 different languages, while the RadioButton 
     * is more useful for cases where you have fewer choices, such as choose 'coffee, tea
     * or water'. Add up your choices by adding up ListItems inside of your SelectList.
     */
    [ParseChildren(true, "Items")]
    public class SelectList : BaseWebControlListFormElement
    {
        /**
         * If this property is anything higher than 1, which is the default value,
         * the select list will no longer be a 'drop down' type of multiple
         * choices widget, but will show as a 'panel' showing all the choices and making
         * it possible to scroll, if needed, to see more items further down.
         * This property is the number of items that the select list will show
         * at one time. Anything higher than 1, will change its appearence completely
         * and make it into a non-drop-down type of widget.
         */
        public int Size
        {
            get { return ViewState["Size"] == null ? 1 : (int)ViewState["Size"]; }
            set
            {
                if (value != Size)
                    SetJsonGeneric("size", value.ToString());
                ViewState["Size"] = value;
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("select"))
            {
                AddAttributes(el);
                foreach (ListItem idx in Items)
                {
                    using (Element l = builder.CreateElement("option"))
                    {
                        l.AddAttribute("value", idx.Value);
                        if (!idx.Enabled)
                            l.AddAttribute("disabled", "disabled");
                        if (idx.Selected)
                            l.AddAttribute("selected", "selected");
                        l.Write(idx.Text);
                    }
                }
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("name", ClientID);
            if (Size != -1)
                el.AddAttribute("size", Size.ToString());
            base.AddAttributes(el);
        }
    }
}
