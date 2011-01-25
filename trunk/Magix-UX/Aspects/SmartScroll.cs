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
    [ToolboxData("<{0}:BehaviorFingerScroll runat=\"server\" />")]
    public class AspectSmartScroll : AspectBase
    {
        public override string GetAspectRegistrationScript()
        {
            return string.Format(
                "new MUX.SmartScroll('{0}')",
                this.ClientID);
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(AspectSmartScroll),
                "Magix.UX.Js.SmartScroll.js");
            base.OnPreRender(e);
        }

        protected override string GetClientSideScript()
        {
            return "";
        }

        public void DispatchEvent(string name)
        {
        }
    }
}

