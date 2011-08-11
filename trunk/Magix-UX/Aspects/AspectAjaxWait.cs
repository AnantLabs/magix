/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
     * Ajax Wait Aspect. Use this one whenever you've got an Ajax Control that 
     * you know will spend a lot of time on the server-side executing its logic.
     * You can either have the client-side logic create its own DOM element to
     * serve as a 'blackout' when it's supposed to display the Ajax Wait DOM 
     * element, or you can attach an existing in-visible DOM element to it by
     * using the Element property.
     */
	public class AspectAjaxWait : AspectBase
	{
        /**
         * The number of milliseconds that will pass after an Ajax Request has been
         * initiated before the Ajax Wait obscurer will be shown. The default
         * value is 500.
         */
        public int Delay
        {
            get { return ViewState["Delay"] == null ? 500 : (int)ViewState["Delay"]; }
            set
            {
                if (value != Delay)
                {
                    SetJsonValue("Delay", value);
                    ViewState["Delay"] = value;
                }
            }
        }

        /**
         * The opacity or transparency of the obscurer DOM element. The default value
         * is 0.5 which means 50% transparent.
         */
        public decimal Opacity
        {
            get { return ViewState["Opacity"] == null ? 0.5M : (decimal)ViewState["Opacity"]; }
            set
            {
                if (value < 0.0M || value > 1.0M)
                    throw new ArgumentException("Opacity value of BehaviorUpdater was not between 0.0 and 1.0");
                if (value != Opacity)
                {
                    SetJsonValue("Opacity", value);
                    ViewState["Opacity"] = value;
                }
            }
        }

        /**
         * The Element you've attached to this particular Ajax Wait. If none is
         * given, Magix UX will on the client-side automatically create a black
         * DOM element for you which will serve as the obscurer.
         */
        public string Element
        {
            get { return ViewState["Element"] == null ? "" : (string)ViewState["Element"]; }
            set
            {
                if (value != Element)
                {
                    SetJsonValue("Element", value);
                    ViewState["Element"] = value;
                }
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
            if (Delay != 500)
            {
                options += string.Format("delay:{0}", Delay);
            }
            if (Opacity != 0.5M)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("opacity:'{0}'",
                        Opacity.ToString(CultureInfo.InvariantCulture)));
            }
            if (!string.IsNullOrEmpty(Element))
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("element:'{0}'", Element));
            }
            if (!string.IsNullOrEmpty(options))
                options += "}";
            return string.Format("new MUX.AspectAjaxWait('{0}'{1})", 
                this.ClientID, 
                options);
		}

        protected override string GetClientSideScript()
		{
			return "";
		}
	}
}
