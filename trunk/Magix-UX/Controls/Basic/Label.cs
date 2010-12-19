/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * A 'text widget'. The basic purpose of this widget is purely to display
     * text and nothing else, though through the CssClass property and the Style
     * property you can easily manipulate this to do mostly anything you wish.
     * If a more 'complex widget' is needed, for instance to host other widgets,
     * then the Panel widget is more appropriate to use than the Label. Unless
     * the Tag property is changed, this widget will render as a &lt;span...
     */
    public class Label : BaseWebControl
    {
        /**
         * The text property of your label. This is what the user will see of your widget.
         */
        public string Text
        {
            get { return ViewState["Text"] == null ? "" : (string)ViewState["Text"]; }
            set
            {
                if (value != Text)
                    SetJsonValue("Text", value);
                ViewState["Text"] = value;
            }
        }

        /**
         * The HTML tag element type used to render your widget. You can set this property
         * to anything you wish, including 'address', 'p' or any other types of HTML tags
         * you wish to use to render your widget. If you need a 'div' HTML element though,
         * or you need to render a widget with child widgets, you should rather use the Panel.
         */
        public string Tag
        {
            get { return ViewState["Tag"] == null ? "span" : (string)ViewState["Tag"]; }
            set { ViewState["Tag"] = value; }
        }

        /**
         * Useful for associating a label with an HTML FORM input element, such as 
         * a CheckBox or a RadioButton etc. Notice that this property can only be
         * legally set if the Tag property is of type "label", which is NOT the 
         * default value. An exception will be thrown if you attempt at setting this
         * property without changing the Tag property to "span".
         */
        public string For
        {
            get { return ViewState["For"] == null ? "" : (string)ViewState["For"]; }
            set
            {
                if (value != For)
                    SetJsonGeneric("for", value.ToString());
                ViewState["For"] = value;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(For) && Tag != "label")
                throw new ArgumentException("You cannot set the For property of a Label without changing the Tag to \"label\".");
            base.OnPreRender(e);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                el.Write(Text);
                RenderChildren(builder.Writer as System.Web.UI.HtmlTextWriter);
            }
        }

        protected override void AddAttributes(Element el)
        {
            if (!string.IsNullOrEmpty(For))
                el.AddAttribute("for", For);
            base.AddAttributes(el);
        }
    }
}
