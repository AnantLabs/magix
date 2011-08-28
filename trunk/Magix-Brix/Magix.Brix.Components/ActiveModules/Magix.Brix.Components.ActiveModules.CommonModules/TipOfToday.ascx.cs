/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: Shows one tip of today with the option for the User to browse forward or
     * backward to read more. Raises 'Magix.Core.GetPreviousToolTip' and
     * 'Magix.Core.GetNextToolTip' to get its next and previous tips
     */
    [ActiveModule]
    public class TipOfToday : UserControl, IModule
    {
        protected Label lbl;
        protected Panel pnl;
        protected Button next;
        protected Button previous;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                SetText(node);
                if (node.Contains("ChildCssClass"))
                    pnl.CssClass += " " + node["ChildCssClass"].Get<string>();
            };
        }

        protected void previous_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.GetPreviousToolTip",
                node);

            SetText(node);

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

            SetText(node);

            new EffectHighlight(lbl, 500)
                .Render();
        }

        private void SetText(Node node)
        {
            if (!node.Contains("Text") ||
                string.IsNullOrEmpty(node["Text"].Get<string>()))
                throw new ArgumentException("Ooops, empty tooltip ...!");

            string str = node["Text"].Get<string>();
            if (!str.Contains("<p"))
                str = "<p>" + str + "</p>";
            lbl.Text = str;
        }
    }
}
