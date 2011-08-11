/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using Magix.UX.Core;
using Magix.UX.Widgets;
using Magix.UX.Helpers;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Aspects
{
    /**
     */
    public class AspectFixated : AspectBase
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AjaxManager.Instance.IncludeScriptFromResource("Effects.js");
        }
        
        public override string GetAspectRegistrationScript()
		{
			return string.Format("new MUX.AspectFixated('{0}')", this.ClientID);
		}
    }
}
