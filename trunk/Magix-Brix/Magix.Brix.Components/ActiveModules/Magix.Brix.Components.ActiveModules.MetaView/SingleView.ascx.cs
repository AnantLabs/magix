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
    public class SingleView : ActiveModule
    {
        protected Panel ctrls;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindCtrls();
        }

        private void DataBindCtrls()
        {
            foreach (Node idx in DataSource["Properties"])
            {
                if (!string.IsNullOrEmpty(idx["Action"].Get<string>()))
                {
                    CreateActionControl(idx);
                }
                else if (idx["ReadOnly"].Get<bool>())
                {
                    CreateReadOnlyControl(idx);
                }
                else
                {
                    CreateReadWriteControl(idx);
                }
            }
        }

        private void CreateActionControl(Node idx)
        {
            Button b = new Button();
            b.Text = idx["Name"].Get<string>();
            b.CssClass = "span-4 action-button";
            b.Info = idx["Name"].Get<string>();
            b.Click +=
                delegate
                {
                    DataSource["ActionEventName"].Value = b.Text;

                    GetPropertyValues();

                    RaiseSafeEvent(
                        idx["Action"].Get<string>(),
                        DataSource);
                };
            ctrls.Controls.Add(b);
        }

        private void GetPropertyValues()
        {
            foreach (Control idx in ctrls.Controls)
            {
                BaseWebControl w = idx as BaseWebControl;
                if (w != null)
                {
                    if (w is TextBox)
                    {
                        DataSource["PropertyValues"][w.Info]["Value"].Value = (w as TextBox).Text;
                    }
                    else if (w is TextBox)
                    {
                        DataSource["PropertyValues"][w.Info]["Value"].Value = (w as Label).Text;
                    }
                }
                DataSource["PropertyValues"][w.Info]["Name"].Value = w.Info;
            }
        }

        private void CreateReadOnlyControl(Node idx)
        {
            Label lbl = new Label();
            lbl.Text = idx["Name"].Get<string>();
            lbl.Info = idx["Name"].Get<string>();
            lbl.ToolTip = idx["Description"].Get<string>();
            lbl.CssClass = "meta-view-form-element meta-view-form-label";
            ctrls.Controls.Add(lbl);
        }

        private void CreateReadWriteControl(Node idx)
        {
            TextBox b = new TextBox();
            b.PlaceHolder = idx["Description"].Get<string>();
            b.ToolTip = b.PlaceHolder;
            b.Info = idx["Name"].Get<string>();
            b.CssClass = "meta-view-form-element meta-view-form-textbox";
            ctrls.Controls.Add(b);
        }
    }
}



