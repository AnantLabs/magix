/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * Image Ajax Widget. Useful for showing images that needs Ajax 
     * functionality somehow. Notice that most times it's more efficient
     * to display other types of widgets, such as the Panel or a Label
     * and set it to display an image through using something such as 
     * background-image through CSS or something similar. 
     */
    public class Image : BaseWebControl
    {
        /**
         * The URL of where your image is. This is the image that will be displayed to
         * the end user.
         */
        public string ImageUrl
        {
            get { return ViewState["ImageUrl"] == null ? "" : (string)ViewState["ImageUrl"]; }
            set
            {
                if (value != ImageUrl)
                    SetJsonGeneric("src", value);
                ViewState["ImageUrl"] = value;
            }
        }

        /**
         * This is the text that will be displayed if the link to the image is broken.
         * It is also the text that most screen-readers will read up loud to the end
         * user.
         */
        public string AlternateText
        {
            get { return ViewState["AlternateText"] == null ? "" : (string)ViewState["AlternateText"]; }
            set
            {
                if (value != AlternateText)
                    SetJsonGeneric("alt", value);
                ViewState["AlternateText"] = value;
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("img"))
            {
                AddAttributes(el);
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("src", ImageUrl);
            el.AddAttribute("alt", AlternateText);
            base.AddAttributes(el);
        }
    }
}
