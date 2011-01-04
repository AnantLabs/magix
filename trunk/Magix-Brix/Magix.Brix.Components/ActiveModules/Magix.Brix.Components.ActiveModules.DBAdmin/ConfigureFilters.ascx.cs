/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
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
    public class ConfigureFilters : System.Web.UI.UserControl, IModule
    {
        protected Panel pnl;
        protected TextBox equals;
        private bool _isFirst;

        public void InitialLoading(Node node)
        {
            _isFirst = true;
            Load +=
                delegate
                {
                    DataSource = node;
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
                case "String":
                case "Int32":
                case "Decimal":
                case "DateTime":
                case "Boolean":
                    CreateNormalCriteria();
                    break;
                default:
                    CreateNotSupportedText();
                    break;
            }
        }

        private void CreateNotSupportedText()
        {
            Label l = new Label();
            l.Text = "Criteria on Complex Objects are yet not supported...";
            pnl.Controls.Add(l);
            Button cancel = Selector.FindControl<Button>(this.Parent.Parent.Parent, "cancel");
            Button ok = Selector.FindControl<Button>(this.Parent.Parent.Parent, "ok");
            new EffectTimeout(500)
                .ChainThese(
                    new EffectFocusAndSelect(cancel))
                .Render();
            ok.Enabled = false;
        }

        private void CreateNormalCriteria()
        {
            Label l = new Label();
            l.Text = "<p>Specify how you want your criteria to appear, empty string removes any existing Criteria...</p>";
            pnl.Controls.Add(l);

            SelectList ls = new SelectList();
            ls.Style[Styles.width] = "110px";

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
            i.Text = "Like";
            ls.Items.Add(i);

            string setting =
                DataSource["FullTypeName"].Get<string>() +
                ":" +
                DataSource["PropertyName"].Get<string>();
            string[] filter = Settings.Instance.Get(setting, "").Split('|');
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

            pnl.Controls.Add(ls);

            TextBox t = new TextBox();
            if (!string.IsNullOrEmpty(filter[0]))
                t.Text = filter[1];
            pnl.Controls.Add(t);

            if (_isFirst)
            {
                if (string.IsNullOrEmpty(Settings.Instance.Get(setting, "")))
                    ls.SelectedIndex = 2;
                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(t))
                    .Render();
            }
        }

        private void CreateControlsForID()
        {
            Label l = new Label();
            l.Text = "Equals: ";
            pnl.Controls.Add(l);

            equals = new TextBox();
            equals.ID = "t";
            equals.Text = Settings.Instance.Get(DataSource["FullTypeName"].Get<string>() + ":" + DataSource["PropertyName"].Value, "");
            pnl.Controls.Add(equals);

            l = new Label();
            l.Text = "<p style=\"margin-top:10px;\">Type in a specific ID, integer value, or a list of comma separated IDs.</p>";
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
                try
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        Settings.Instance.Set(fullTypeName + ":" + propertyName, "");
                    }
                    else
                    {
                        // Just to verify we've got only integers here ...
                        string[] ids = filter.Split(',');
                        foreach (string idx in ids)
                        {
                            int x = int.Parse(idx);
                        }
                        Node node = new Node();
                        node["Type"].Value = fullTypeName;
                        Settings.Instance.Set(fullTypeName + ":" + propertyName, filter);
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "DBAdmin.FilterChanged",
                            node);
                    }
                    (this.Parent.Parent.Parent as Window).CloseWindow();
                }
                catch(Exception err)
                {
                    Node node = new Node();
                    node["Message"].Value = err.Message;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowMessage",
                        node);
                    equals.Select();
                    equals.Focus();
                }
            }
            else
            {
                string filter = Selector.SelectFirst<TextBox>(this).Text;
                try
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        Settings.Instance.Set(fullTypeName + ":" + propertyName, "");
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
                                break;
                        }
                        Settings.Instance.Set(fullTypeName + ":" + propertyName, set + "|" + filter);
                        Node node = new Node();
                        node["Type"].Value = fullTypeName;
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "DBAdmin.FilterChanged",
                            node);
                    }
                    (this.Parent.Parent.Parent as Window).CloseWindow();
                }
                catch (Exception err)
                {
                    Node node = new Node();
                    node["Message"].Value = err.Message;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowMessage",
                        node);
                    equals.Select();
                    equals.Focus();
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
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowMessage",
                    node);
                equals.Select();
                equals.Focus();
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




















