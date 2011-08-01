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
    public class Clickable : ActiveModule, IModule
    {
        protected Button btn;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load += delegate
            {
                btn.Text = node["Text"].Get<string>();
                if (node.Contains("ButtonCssClass"))
                    btn.CssClass = node["ButtonCssClass"].Get<string>();
                if (node.Contains("Enabled"))
                    btn.Enabled = node["Enabled"].Get<bool>();
                if (node.Contains("ToolTip"))
                    btn.ToolTip = node["ToolTip"].Get<string>();
            };
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            RaiseSafeEvent(
                DataSource["Event"].Get<string>(),
                DataSource["Event"]);
        }

        [ActiveEvent(Name = "Magix.Core.EnabledClickable")]
        protected void Magix_Core_EnabledClickable(object sender, ActiveEventArgs e)
        {
            if (DataSource["Seed"].Get<string>() == e.Params["Seed"].Get<string>())
                btn.Enabled = e.Params["Enabled"].Get<bool>();
        }
    }
}



