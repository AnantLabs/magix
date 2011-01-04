/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;
using System.Web.UI.HtmlControls;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ConfigureColumns : System.Web.UI.UserControl, IModule
    {
        protected System.Web.UI.WebControls.Repeater rep;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    rep.DataSource = DataSource["Columns"];
                    rep.DataBind();
                    CheckBox ch = Selector.SelectFirst<CheckBox>(rep);
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(ch))
                        .Render();
                };
        }

        protected void CheckedChange(object sender, EventArgs e)
        {
            CheckBox b = sender as CheckBox;
            string columnName = b.Info;
            Node node = new Node();
            node["ColumnName"].Value = columnName;
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            node["Visible"].Value = b.Checked;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.ChangeVisibilityOfColumn",
                node);
        }

        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




















