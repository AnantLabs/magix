/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    [ActiveModule]
    public class Header : UserControl, IModule
    {
        protected Label lbl;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
            };
        }

        [ActiveEvent(Name = "Magix.Core.SetFormCaption")]
        protected void Magix_Core_SetFormCaption(object sender, ActiveEventArgs e)
        {
            string caption = e.Params["Caption"].Get<string>();
            lbl.Text = caption;
        }
    }
}



