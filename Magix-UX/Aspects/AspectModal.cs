/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using Magix.UX.Helpers;

namespace Magix.UX.Aspects
{
    /**
     * Aspect useful for making 'modal' controls. A Modal control will obscur the 
     * contents behind it by creating a div that is semi-transparent (most often)
     * and fill the entire viewport (browser area). This effect makes it impossible
     * to click controls behind it on the page.
     */
    public class AspectModal : AspectBase
	{
        /**
         * The transparency of the control. If this value is 1 the 'modality-div' created
         * will be 100% opaque. If 0.5 it will be 50% transparent, if 0 it will be 100%
         * transparent, but still actually do its job to some extent.
         */
        public decimal Opacity
        {
            get { return ViewState["Opacity"] == null ? 0.5M : (decimal)ViewState["Opacity"]; }
            set
            {
                if (value < 0.0M || value > 1.0M)
                    throw new ArgumentException("Opacity value must be between 0.0 and 1.0");
                if (value != Opacity)
                    SetJsonValue("Opacity", value);
                ViewState["Opacity"] = value;
            }
        }

        /**
         * The color of the 'modality-div'. The default value is 'Black'. If you also set its
         * BottomColor property, then a gradient will be created between the Color (top parts) 
         * and the BottomColor (bottom) and it will gradient between those starting from the top.
         */
        public Color Color
        {
            get { return ViewState["Color"] == null ? System.Drawing.Color.Black : (Color)ViewState["Color"]; }
            set
            {
                if (value != Color)
                    SetJsonValue("Color", value);
                ViewState["Color"] = value;
            }
        }

        /**
         * The color of the 'modality-div'. The default value is 'Black'. If you also set its
         * BottomColor property, then a gradient will be created between the Color (top parts) 
         * and the BottomColor (bottom) and it will gradient between those starting from the top.
         */
        public Color BottomColor
        {
            get { return ViewState["BottomColor"] == null ? System.Drawing.Color.Black : (Color)ViewState["BottomColor"]; }
            set
            {
                if (value != Color)
                    SetJsonValue("BottomColor", value);
                ViewState["BottomColor"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AjaxManager.Instance.IncludeScriptFromResource("Effects.js");
        }

        public override string GetAspectRegistrationScript()
		{
			string options = string.Empty;
            if (Color != System.Drawing.Color.Black)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("color:'{0}'", System.Drawing.ColorTranslator.ToHtml(Color)));
            }
            if (BottomColor != System.Drawing.Color.Black)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("bottomColor:'{0}'", System.Drawing.ColorTranslator.ToHtml(Color)));
            }
            if (Opacity != 0.5M)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("opacity:'{0}'", Opacity.ToString(CultureInfo.InvariantCulture)));
            }
            options = StringHelper.ConditionalAdd(
                options,
                "",
                "}",
                "");
            return string.Format("new MUX.AspectModal('{0}'{1})", this.ClientID, options);
		}

        protected override string GetClientSideScript()
		{
			return "";
		}
	}
}
