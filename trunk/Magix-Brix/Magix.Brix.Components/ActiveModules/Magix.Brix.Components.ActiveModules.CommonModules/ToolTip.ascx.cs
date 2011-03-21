/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
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
    public class ToolTip : UserControl, IModule
    {
        protected Label lbl;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                lbl.Text = node["Text"].Get<string>();
            };
        }

        protected void previous_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.GetPreviousToolTip",
                node);
            lbl.Text = node["Text"].Get<string>();
            new EffectHighlight(lbl, 500)
                .Render();
        }

        protected void next_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.GetNextToolTip",
                node);
            lbl.Text = node["Text"].Get<string>();
            new EffectHighlight(lbl, 500)
                .Render();
        }
    }
}



