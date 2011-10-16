/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    /**
     * Level2: Encapsulates a module for selecting object, image, etc to link to
     */
    [ActiveModule]
    public class CreateLink : ActiveModule
    {
        protected SelectList lst;
        protected TextBox txt;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    InitializeListBoxItems();
                    new EffectTimeout(250)
                        .ChainThese(
                            new EffectFocusAndSelect(txt))
                        .Render();
                };
        }

        private void InitializeListBoxItems()
        {
            foreach (Node idx in DataSource["Items"])
            {
                ListItem li = new ListItem(idx["Name"].Get<string>(), idx["ID"].Value.ToString());
                lst.Items.Add(li);
            }
        }

        protected void lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt.Text = lst.SelectedItem.Value;
            lst.SelectedIndex = 0;
            txt.Select();
            txt.Focus();
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["URL"].Value = txt.Text;

            RaiseSafeEvent(
                "Magix.Core.UrlWasCreated",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");
        }
    }
}
