/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Magic.UX.Builder;
using Magic.UX.Widgets.Core;

namespace Magic.UX.Widgets
{
    /**
     * A multiple line type of 'give me some text input' type of widget. It wraps the
     * &lt;textarea HTML element. If you only need single lines of input, you should
     * probably rather use the TextBox widget. However this widget is useful for cases
     * when you need multiple lines of text input. See also the RichEdit widget if
     * you need rich formatting of your text.
     */
    public class TextArea : BaseWebControlFormElementInputText
    {
        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("textarea"))
            {
                AddAttributes(el);
                el.Write(Text);
            }
        }

        public string PlaceHolder
        {
            get { return ViewState["PlaceHolder"] == null ? "" : (string)ViewState["PlaceHolder"]; }
            set
            {
                if (value != PlaceHolder)
                    SetJsonGeneric("placeholder", value);
                ViewState["PlaceHolder"] = value;
            }
        }

        protected override bool ShouldAddValue
        {
            get { return false; }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("name", ClientID);
            if (!string.IsNullOrEmpty(PlaceHolder))
                el.AddAttribute("placeholder", PlaceHolder);
            base.AddAttributes(el);
        }
    }
}
