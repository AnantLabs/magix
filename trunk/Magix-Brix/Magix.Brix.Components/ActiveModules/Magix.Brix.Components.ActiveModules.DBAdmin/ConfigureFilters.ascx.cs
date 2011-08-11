/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ConfigureFilters : Module, IModule
    {
        protected Panel pnl;
        protected TextBox equals;
        protected Button ok;
        private bool _isFirst;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            _isFirst = true;
            Load +=
                delegate
                {
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindFilters();
        }

        private void DataBindFilters()
        {
            if (DataSource["PropertyName"].Get<string>() == "ID")
            {
                CreateControlsForID();
            }
            else
            {
                CreateControlsForNonID();
            }
        }

        private void CreateControlsForNonID()
        {
            switch (DataSource["PropertyTypeName"].Get<string>())
            {
                case "System.String":
                case "System.Int32":
                case "System.Decimal":
                case "System.DateTime":
                case "System.Boolean":
                    CreateNormalCriteria();
                    break;
                default:
                    CreateComplexCriteria();
                    break;
            }
        }

        private void CreateComplexCriteria()
        {
            Label wrp = new Label();
            wrp.Tag = "p";

            Label l = new Label();
            l.Text = "In or has: ";
            wrp.Controls.Add(l);

            equals = new TextBox();
            equals.ID = "t";
            Node fNode = new Node();
            fNode["Key"].Value = "DBAdmin.Filter." +
                DataSource["FullTypeName"].Get<string>() +
                ":" +
                DataSource["PropertyName"].Value;
            fNode["Default"].Value = "";
            RaiseSafeEvent(
                "DBAdmin.Data.GetFilter",
                fNode);
            string filter = fNode["Filter"].Get<string>();
            equals.Text = filter;
            if (!string.IsNullOrEmpty(equals.Text))
                equals.Text = equals.Text.Split('|')[1];
            wrp.Controls.Add(equals);
            pnl.Controls.Add(wrp);

            l = new Label();
            l.Tag = "p";
            l.Text = @"Type in the ID of the 
Document you want to filter by. Empty string removes any existing filters.";
            pnl.Controls.Add(l);
            if (_isFirst)
            {
                new EffectTimeout(250)
                    .ChainThese(
                        new EffectFocusAndSelect(equals))
                    .Render();
            }
        }

        private void CreateNormalCriteria()
        {
            Label wrp = new Label();
            wrp.Tag = "p";

            SelectList ls = new SelectList();

            ListItem i = new ListItem();
            i.Value = "Lt";
            i.Text = "Less Than";
            ls.Items.Add(i);

            i = new ListItem();
            i.Value = "Gt";
            i.Text = "Greater Than";
            ls.Items.Add(i);

            i = new ListItem();
            i.Value = "Eq";
            i.Text = "Equals";
            ls.Items.Add(i);

            i = new ListItem();
            i.Value = "Like";
            i.Text = "Wildcard";
            ls.Items.Add(i);

            Node fNode = new Node();
            fNode["Key"].Value = 
                "DBAdmin.Filter." + 
                DataSource["FullTypeName"].Get<string>() +
                ":" +
                DataSource["PropertyName"].Get<string>();
            fNode["Default"].Value = "";
            RaiseSafeEvent(
                "DBAdmin.Data.GetFilter",
                fNode);
            string setting = fNode["Filter"].Get<string>();
            string[] filter = setting.Split('|');
            if (!string.IsNullOrEmpty(filter[0]))
            {
                switch (filter[0])
                {
                    case "Lt":
                        ls.SelectedIndex = 0;
                        break;
                    case "Gt":
                        ls.SelectedIndex = 1;
                        break;
                    case "Eq":
                        ls.SelectedIndex = 2;
                        break;
                    case "Like":
                        ls.SelectedIndex = 3;
                        break;
                }
            }

            wrp.Controls.Add(ls);

            TextBox t = new TextBox();
            if (!string.IsNullOrEmpty(filter[0]))
                t.Text = filter[1];
            wrp.Controls.Add(t);

            pnl.Controls.Add(wrp);

            Label l = new Label();
            l.Tag = "p";
            l.Text = @"Specify how you want your criteria to appear, 
empty string removes any existing Criteria...";
            pnl.Controls.Add(l);

            if (_isFirst)
            {
                if (string.IsNullOrEmpty(setting))
                    ls.SelectedIndex = 3;
                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(t))
                    .Render();
            }
        }

        private void CreateControlsForID()
        {
            Label wrp = new Label();
            wrp.Tag = "p";

            Label l = new Label();
            l.Text = "Equals: ";
            wrp.Controls.Add(l);

            equals = new TextBox();
            equals.ID = "t";
            Node fNode = new Node();
            fNode["Key"].Value =
                "DBAdmin.Filter." +
                DataSource["FullTypeName"].Get<string>() +
                ":" +
                DataSource["PropertyName"].Value;
            fNode["Default"].Value = "";
            RaiseSafeEvent(
                "DBAdmin.Data.GetFilter",
                fNode);
            string filter = fNode["Filter"].Get<string>();
            equals.Text = filter;
            wrp.Controls.Add(equals);
            pnl.Controls.Add(wrp);

            l = new Label();
            l.Tag = "p";
            l.Text = @"Type in a specific ID, integer value, 
or a list of comma separated IDs. Empty string removes any existing filters.";
            pnl.Controls.Add(l);
            if (_isFirst)
            {
                new EffectTimeout(250)
                    .ChainThese(
                        new EffectFocusAndSelect(equals))
                    .Render();
            }
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            string propertyName = DataSource["PropertyName"].Get<string>();
            string fullTypeName = DataSource["FullTypeName"].Get<string>();
            if (propertyName == "ID")
            {
                string filter = equals.Text;
                if (string.IsNullOrEmpty(filter))
                {
                    Node fNode = new Node();
                    fNode["Key"].Value = 
                        "DBAdmin.Filter." + 
                        fullTypeName + ":" + 
                        propertyName;
                    fNode["Value"].Value = "";
                    RaiseSafeEvent(
                        "DBAdmin.Data.SetFilter",
                        fNode);
                }
                else
                {
                    // Just to verify we've got only integers here ...
                    string[] ids = filter.Split(',');
                    foreach (string idx in ids)
                    {
                        int x = int.Parse(idx);
                    }
                    Node fNode = new Node();
                    fNode["Key"].Value =
                        "DBAdmin.Filter." +
                        fullTypeName + ":" +
                        propertyName;
                    fNode["Value"].Value = filter;
                    RaiseSafeEvent(
                        "DBAdmin.Data.SetFilter",
                        fNode);
                }
                ActiveEvents.Instance.RaiseClearControls("child");
            }
            else
            {
                switch (DataSource["PropertyTypeName"].Get<string>())
                {
                    case "System.String":
                    case "System.Int32":
                    case "System.Decimal":
                    case "System.Boolean":
                    case "System.DateTime":
                        {
                            string filter = Selector.SelectFirst<TextBox>(this).Text;
                            try
                            {
                                if (string.IsNullOrEmpty(filter))
                                {
                                    Node fNode = new Node();
                                    fNode["Key"].Value =
                                        "DBAdmin.Filter." +
                                        fullTypeName + ":" +
                                        propertyName;
                                    fNode["Value"].Value = "";
                                    RaiseSafeEvent(
                                        "DBAdmin.Data.SetFilter",
                                        fNode);
                                }
                                else
                                {
                                    int selIndex = Selector.SelectFirst<SelectList>(this).SelectedIndex;
                                    string set = "";
                                    switch (selIndex)
                                    {
                                        case 0:
                                            set = "Lt";
                                            break;
                                        case 1:
                                            set = "Gt";
                                            break;
                                        case 2:
                                            set = "Eq";
                                            break;
                                        case 3:
                                            set = "Like";
                                            if (!filter.Contains("*"))
                                                filter = "*" + filter + "*";
                                            break;
                                    }
                                    Node fNode = new Node();
                                    fNode["Key"].Value =
                                        "DBAdmin.Filter." +
                                        fullTypeName + ":" +
                                        propertyName;
                                    fNode["Value"].Value = set + "|" + filter;
                                    RaiseSafeEvent(
                                        "DBAdmin.Data.SetFilter",
                                        fNode);
                                }
                                ActiveEvents.Instance.RaiseClearControls("child");
                            }
                            catch (Exception err)
                            {
                                Node node = new Node();
                                node["Message"].Value = err.Message;
                                RaiseSafeEvent(
                                    "Magix.Core.ShowMessage",
                                    node);
                                equals.Select();
                                equals.Focus();
                            }
                        } break;
                    default:
                        {
                            try
                            {
                                string filter = Selector.SelectFirst<TextBox>(this).Text;
                                if (string.IsNullOrEmpty(filter))
                                {
                                    Node fNode = new Node();
                                    fNode["Key"].Value =
                                        "DBAdmin.Filter." +
                                        fullTypeName + ":" +
                                        propertyName;
                                    fNode["Value"].Value = "";
                                    RaiseSafeEvent(
                                        "DBAdmin.Data.SetFilter",
                                        fNode);
                                }
                                else
                                {
                                    Node fNode = new Node();
                                    fNode["Key"].Value =
                                        "DBAdmin.Filter." +
                                        fullTypeName + ":" +
                                        propertyName;
                                    fNode["Value"].Value = "In|" + filter;
                                    RaiseSafeEvent(
                                        "DBAdmin.Data.SetFilter",
                                        fNode);
                                }
                                ActiveEvents.Instance.RaiseClearControls("child");
                            }
                            catch (Exception err)
                            {
                                Node node = new Node();
                                node["Message"].Value = err.Message;
                                RaiseSafeEvent(
                                    "Magix.Core.ShowMessage",
                                    node);
                                equals.Select();
                                equals.Focus();
                            }
                        } break;
                }
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            try
            {
                (this.Parent.Parent.Parent as Window).CloseWindow();
            }
            catch (Exception err)
            {
                Node node = new Node();
                node["Message"].Value = err.Message;
                RaiseSafeEvent(
                    "Magix.Core.ShowMessage",
                    node);
                equals.Select();
                equals.Focus();
            }
        }

        protected override void ReDataBind()
        {
            ok.Focus();
        }
    }
}




















