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
     * A wrapper around a hyper link or anchor HTML element (&lt;a ...)
     * Sometimes you will need to create links that might change or needs
     * changes after initially created. For such scenarios, this widget
     * is highly useful.
     */
    public class HyperLink : BaseWebControl
    {
        /**
         * The anchor text for your hyperlink. This is the text that will be visible in
         * the browser.
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
         * The URL for your link. This is where the user ends up if he clicks 
         * your anchor text.
         */
        public string URL
        {
            get { return ViewState["URL"] == null ? "" : (string)ViewState["URL"]; }
            set
            {
                if (value != Text)
                    this.SetJsonGeneric("href", value);
                ViewState["URL"] = value;
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("a"))
            {
                AddAttributes(el);
                el.Write(Text);
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("href", URL);
            base.AddAttributes(el);
        }
	}
}
