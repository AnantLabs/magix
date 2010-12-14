/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Aspects
{
    /**
     * Base class for all Aspects in Magix UX. Implements common functionality.
     */
	public abstract class AspectBase : BaseControl
	{
        protected override void OnInit(EventArgs e)
	    {
	        base.OnInit(e);
			AjaxManager.Instance.IncludeScriptFromResource("Aspects.js");
	    }

        /**
         * Helper methods for Aspect developers. Loads state from the client-side into 
         * the Control/Aspect. Used in e.g. AspectDraggable for changing the top/left 
         * property of the control when moved and its got a server-side binding 
         * (event handler)
         */
        public virtual void EnsureViewStateLoads()
        { }

        public override void RenderControl(HtmlTextWriter writer)
        {
			// We roughly only need to handle what happens for JSON changes in the Behaviors
            if (!DesignMode)
            {
                if (Visible)
                {
                    if (AjaxManager.Instance.IsCallback)
                    {
                        if (this.HasRendered)
                        {
                            RenderJson(writer);
                        }
                    }
                }
            }
        }

        protected override void RenderJson(HtmlTextWriter writer)
        {
            string json = SerializeJson();
            if (!string.IsNullOrEmpty(json))
            {
                AjaxManager.Instance.Writer.WriteLine(
                    "MUX.Aspect.$('{0}').JSON({1});", 
                    ClientID, 
                    json);
            }
        }

		public abstract string GetAspectRegistrationScript();
    }
}
