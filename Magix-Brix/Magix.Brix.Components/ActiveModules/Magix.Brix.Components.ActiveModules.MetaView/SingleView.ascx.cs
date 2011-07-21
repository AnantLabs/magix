/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.MetaView
{
    [ActiveModule]
    [PublisherPlugin]
    public class SingleView : ActiveModule
    {
        protected Panel ctrls;

        public override void InitialLoading(Node node)
        {
            if (!node.Contains("MetaViewTypeName"))
            {
                // Probably in 'production mode' and hence need to get our data ...
                node["MetaViewName"].Value = ViewName;

                RaiseSafeEvent(
                    "Magix.MetaView.GetViewData",
                    node);
            }

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
            foreach (Node idx in DataSource["Properties"])
            {
                if (!string.IsNullOrEmpty(idx["Action"].Get<string>()))
                {
                    CreateActionControl(idx);
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
            if (idx.Contains("ReadOnly"))
                b.Enabled = !idx["ReadOnly"].Get<bool>();
            b.CssClass = "action-button";
            b.Info = idx["Name"].Get<string>();
            b.ToolTip = idx["Description"].Get<string>();
            b.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ActionSenderName"].Value = b.Text;
                    node["MetaViewName"].Value = DataSource["MetaViewName"].Value;
                    node["MetaViewTypeName"].Value = DataSource["MetaViewTypeName"].Value;

                    GetPropertyValues(node);

                    foreach (string idxS in idx["Action"].Get<string>().Split('|'))
                    {
                        // Settings Event Specific Features ...
                        node["ActionName"].Value = idxS ;

                        string eventName = "Magix.Meta.RaiseEvent";

                        RaiseSafeEvent(
                            eventName,
                            node);
                    }
                };
            ctrls.Controls.Add(b);
        }

        private void GetPropertyValues(Node node)
        {
            foreach (Control idx in ctrls.Controls)
            {
                BaseWebControl w = idx as BaseWebControl;
                if (w != null)
                {
                    if (w is TextBox)
                    {
                        node["PropertyValues"][w.Info]["Value"].Value = (w as TextBox).Text;
                        node["PropertyValues"][w.Info]["Name"].Value = w.Info;
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
            b.CssClass = "meta-view-form-element meta-view-form-textbox";
            if (shouldClear)
                b.CssClass += " clear-both";
            ctrls.Controls.Add(b);
        }

        [ActiveEvent(Name = "Magix.Meta.GetContainerIDOfApplicationWebPart")]
        protected void Magix_Meta_GetContainerIDOfApplicationWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["PageObjectTemplateID"].Get<int>() == DataSource["PageObjectTemplateID"].Get<int>())
                e.Params["ID"].Value = this.Parent.ID;
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

        [ModuleSetting]
        public string ViewName
        {
            get { return ViewState["ViewName"] as string; }
            set { ViewState["ViewName"] = value; }
        }
    }
}



