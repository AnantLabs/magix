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

            Button b = Selector.SelectFirst<Button>(ctrls);
            if (b != null)
                ctrls.DefaultWidget = b.ID;
        }

        private void DataBindCtrls()
        {
            bool lastWasButton = false;
            foreach (Node idx in DataSource["Properties"])
            {
                lastWasButton = false;
                if (!string.IsNullOrEmpty(idx["Action"].Get<string>()))
                {
                    CreateActionControl(idx);
                    lastWasButton = true;
                }
                else if (idx["ReadOnly"].Get<bool>())
                {
                    CreateReadOnlyControl(idx, true);
                }
                else
                {
                    CreateReadWriteControl(idx, true);
                }
            }
        }

        private void CreateActionControl(Node idx)
        {
            Button b = new Button();
            b.Text = idx["Name"].Get<string>();
            b.ID = "b-" + idx["ID"].Get<int>();
            b.CssClass = "span-4 action-button";
            b.Info = idx["Name"].Get<string>();
            b.ToolTip = idx["Description"].Get<string>();
            b.Click +=
                delegate
                {
                    DataSource["ActionEventName"].Value = b.Text;

                    GetPropertyValues();

                    foreach (string idxS in idx["Action"].Get<string>().Split('|'))
                    {
                        // Settings Event Specific Features ...
                        DataSource["EventTime"].Value = DateTime.Now;
                        DataSource["EventReference"].Value = idxS;

                        string eventName = idxS;

                        if (eventName.Contains("("))
                        {
                            int eventId = int.Parse(eventName.Split('(')[1].Split(')')[0]);
                            eventName = eventName.Substring(0, eventName.IndexOf("("));

                            DataSource["ActionID"].Value = eventId;

                            RaiseSafeEvent(
                                eventName,
                                DataSource);
                        }
                        else
                        {
                            RaiseSafeEvent(
                                eventName,
                                DataSource);
                        }
                    }
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
                        DataSource["PropertyValues"][w.Info]["Name"].Value = w.Info;
                    }
                    else if (w is Label)
                    {
                        DataSource["PropertyValues"][w.Info]["Value"].Value = (w as Label).Text;
                        DataSource["PropertyValues"][w.Info]["Name"].Value = w.Info;
                    }
                }
            }
        }

        private void CreateReadOnlyControl(Node idx, bool shouldClear)
        {
            Label lbl = new Label();
            lbl.ToolTip = idx["Name"].Get<string>();
            lbl.Info = idx["Name"].Get<string>();
            lbl.Text = idx["Description"].Get<string>();
            lbl.CssClass = "meta-view-form-element meta-view-form-label";
            if (shouldClear)
                lbl.CssClass += " clear-both";
            ctrls.Controls.Add(lbl);
        }

        private void CreateReadWriteControl(Node idx, bool shouldClear)
        {
            TextBox b = new TextBox();
            b.PlaceHolder = idx["Description"].Get<string>();
            b.ToolTip = b.PlaceHolder;
            b.Info = idx["Name"].Get<string>();
            if (shouldClear)
                b.CssClass += " clear-both";
            b.CssClass = "meta-view-form-element meta-view-form-textbox";
            ctrls.Controls.Add(b);
        }

        [ActiveEvent(Name = "Magix.Meta.Actions.EmptyForm")]
        protected void Magix_Meta_Actions_EmptyForm(object sender, ActiveEventArgs e)
        {
            foreach (Control idx in ctrls.Controls)
            {
                BaseWebControl b = idx as BaseWebControl;
                if (b != null)
                {
                    TextBox t = b as TextBox;
                    if (t != null)
                    {
                        t.Text = "";
                    }
                }
            }
        }
    }
}



