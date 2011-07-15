/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;

namespace Magix.Brix.Components.ActiveModules.MetaView
{
    [ActiveModule]
    public class EditView : ActiveModule
    {
        protected InPlaceTextAreaEdit type;
        protected SelectList lst;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    type.Text = node["MetaTypeName"].Get<string>();
                    if (node["IsList"].Get<bool>())
                        lst.SelectedIndex = 0;
                    else
                        lst.SelectedIndex = 1;
                };
        }

        protected void lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["IsList"].Value = lst.SelectedIndex == 0;

            RaiseSafeEvent(
                "Magix.Publishing.ChangeTypeOfMetaView",
                node);
        }

        protected void type_TextChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["MetaTypeName"].Value = type.Text;

            RaiseSafeEvent(
                "Magix.Publishing.ChangeTypeOfMetaView",
                node);
        }
    }
}



