/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;


namespace Magix.UX.Widgets
{
    /**
     */
    public class Audio : BaseWebControl
    {
        /**
         * 
         */
        public string SoundFile
        {
            get { return ViewState["SoundFile"] == null ? "" : (string)ViewState["SoundFile"]; }
            set
            {
                if (value != SoundFile)
                    SetJsonGeneric("src", value);
                ViewState["SoundFile"] = value;
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("audio"))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }

        protected override void AddAttributes(Element el)
        {
            if (!string.IsNullOrEmpty(SoundFile))
                el.AddAttribute("src", SoundFile);
            el.AddAttribute("autoplay", "autoplay");
            base.AddAttributes(el);
        }
    }
}
