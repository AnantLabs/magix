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
    public class ConfigureColumns : Module, IModule
    {
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    rep.DataSource = DataSource["Type"]["Properties"];
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
            RaiseSafeEvent(
                "DBAdmin.Data.ChangeVisibilityOfColumn",
                node);
        }

        protected override void ReDataBind()
        {
        }
    }
}




















