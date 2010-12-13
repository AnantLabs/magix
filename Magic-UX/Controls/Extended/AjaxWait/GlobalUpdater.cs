/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Globalization;
using System.ComponentModel;
using Magix.UX;
using Magix.UX.Widgets;

namespace Magix.UX.Widgets
{
    public class AjaxWait : Panel
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Style[Styles.display] = "none";
        }

        [DefaultValue(1000)]
        public int Delay
        {
            get { return ViewState["Delay"] == null ? 1000 : (int)ViewState["Delay"]; }
            set
            {
                if (value != Delay)
                    SetJsonValue("Delay", value);
                ViewState["Delay"] = value;
            }
        }

        [DefaultValue(1.0)]
        public decimal MaxOpacity
        {
            get { return ViewState["MaxOpacity"] == null ? 1.0M : (decimal)ViewState["MaxOpacity"]; }
            set
            {
                if (value != MaxOpacity)
                    SetJsonValue("MaxOpacity", value);
                ViewState["MaxOpacity"] = value;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(AjaxWait),
                "Magic_UX.Js.AjaxWait.js");
            AjaxManager.Instance.IncludeScriptFromResource(
                "Effects.js");
            base.OnPreRender(e);
        }

        protected override string GetClientSideScriptOptions()
        {
            string retVal = base.GetClientSideScriptOptions();
            if (Delay != 1000)
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += ",";
                retVal += string.Format("delay:{0}", Delay);
            }
            if (MaxOpacity != 1.0M)
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += ",";
                retVal += string.Format("maxOpacity:{0}", MaxOpacity.ToString(CultureInfo.InvariantCulture));
            }
            return retVal;
        }

        protected override string GetClientSideScriptType()
        {
            return "new MUX.AjaxWait";
        }
    }
}
