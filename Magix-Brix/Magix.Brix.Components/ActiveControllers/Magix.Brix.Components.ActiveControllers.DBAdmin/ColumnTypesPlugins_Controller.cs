/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Data.Internal;
using Magix.Brix.Components.ActiveTypes;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.DBAdmin
{
    /**
     * Level3: Contains some template columns for the Grid system for you to use in your
     * own Grids
     */
    [ActiveController]
    public class ColumnTypesPlugins_Controller : ActiveController
    {
        // TODO: Create more of these guys. SelectLists for instance are in HIGH demand ...!!
        /**
         * Level3: Creates a CheckBox type of column for the Grid System
         */
        [ActiveEvent(Name = "Magix.DataPlugins.GetTemplateColumns.CheckBox")]
        protected void Magix_DataPlugins_GetTemplateColumns_CheckBox(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();

            Panel pnl = new Panel();

            CheckBox ch = new CheckBox();
            ch.Style[Styles.floating] = "left";
            ch.Style[Styles.width] = "15px";
            ch.Style[Styles.display] = "block";
            ch.Checked = bool.Parse(e.Params["Value"].Value.ToString());
            ch.CheckedChanged +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = e.Params["ID"].Value;
                    node["FullTypeName"].Value = e.Params["FullTypeName"].Value;
                    node["NewValue"].Value = ch.Checked.ToString();
                    node["PropertyName"].Value = e.Params["Name"].Value;

                    RaiseEvent(
                        "DBAdmin.Data.ChangeSimplePropertyValue",
                        node);
                };
            pnl.Controls.Add(ch);

            Label l = new Label();
            l.Text = "&nbsp;";
            l.CssClass += "span-2";
            l.Tag = "label";
            l.Load +=
                delegate
                {
                    l.For = ch.ClientID;
                };
            pnl.Controls.Add(l);

            e.Params["Control"].Value = pnl;
        }
    }
}
