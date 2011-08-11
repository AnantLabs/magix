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
	public class AspectScreenSaver : AspectBase
	{
        public int Delay
        {
            get { return ViewState["Delay"] == null ? 15000 : (int)ViewState["Delay"]; }
            set
            {
                if (value != Delay)
                {
                    SetJsonValue("Delay", value);
                    ViewState["Delay"] = value;
                }
            }
        }

        public string Images
        {
            get { return ViewState["Images"] == null ? "" : (string)ViewState["Images"]; }
            set
            {
                if (value != Images)
                {
                    SetJsonValue("Images", value);
                    ViewState["Images"] = value;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AjaxManager.Instance.IncludeScriptFromResource("Effects.js");
            AjaxManager.Instance.IncludeScriptFromResource("Aspects.js");
        }

		public override string GetAspectRegistrationScript()
		{
			string options = string.Empty;
            if (Delay != 15000)
            {
                options += string.Format(",{{delay:{0}", Delay);
            }
            if (Images != "")
            {
                options += string.Format(",images:'{0}'", Images);
            }
            if (!string.IsNullOrEmpty(options))
                options += "}";
            return string.Format("new MUX.AspectScreenSaver('{0}'{1})", 
                this.ClientID, 
                options);
		}

        protected override string GetClientSideScript()
		{
			return "";
		}
	}
}
