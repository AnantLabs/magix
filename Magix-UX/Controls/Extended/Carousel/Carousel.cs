/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;
using Magix.UX.Effects;

namespace Magix.UX.Widgets
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
                typeof(Carousel),
                "Magix.UX.Js.Carousel.js");
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Effect),
                "Magix.UX.Js.Effects.js");
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
