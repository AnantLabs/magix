/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using System.Reflection;
using Magix.UX.Effects;
using Magix.UX.Aspects;

namespace Magix.Brix.Components.ActiveModules.ClipBoard
{
    /**
     * Level2: Will display a list of all of your ClipBoard items
     */
    [ActiveModule]
    public class Clips : ActiveModule
    {
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    rep.DataSource = DataSource["ClipBoard"];
                    rep.DataBind();
                };
        }

        protected void PasteClipboardItem(object sender, EventArgs e)
        {
            LinkButton b = sender as LinkButton;
            int id = int.Parse(b.Info);

            Node node = new Node();

            node["ID"].Value = id;

            RaiseSafeEvent(
                "Magix.ClipBoard.PrepareToPasteNode",
                node);
        }

        protected void DeleteClipboardItem(object sender, EventArgs e)
        {
            LinkButton b = sender as LinkButton;
            int id = int.Parse(b.Info);

            Node node = new Node();

            node["ID"].Value = id;

            RaiseSafeEvent("Magix.ClipBoard.RemoveItemFromClipBoard");
        }

        protected void close_Click(object sender, EventArgs e)
        {
            RaiseSafeEvent("Magix.ClipBoard.CloseClipBoard");
        }
    }
}
