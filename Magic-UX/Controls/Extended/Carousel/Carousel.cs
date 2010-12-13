/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magic.UX.Widgets;
using Magic.UX.Aspects;
using Magic.UX.Builder;
using Magic.UX.Widgets.Core;
using Magic.UX.Effects;

namespace Magic.UX.Widgets
{
    public class Carousel : Panel
    {
        public Carousel()
        {
            CssClass = "mux-carousel";
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Timer),
                "Magic_UX.Js.Carousel.js");
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Timer),
                "Magic_UX.Js.Effects.js");
            base.OnPreRender(e);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            this.Style[Styles.display] = "none";
            base.RenderMuxControl(builder);
        }

        protected override string GetClientSideScriptType()
        {
            return "new MUX.Carousel";
        }
    }
}
