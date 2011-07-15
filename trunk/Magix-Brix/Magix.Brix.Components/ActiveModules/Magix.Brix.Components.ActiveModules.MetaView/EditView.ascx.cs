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
        protected Panel properties;
        protected CheckBox hasSearch;
        protected Label lblS;
        protected InPlaceTextAreaEdit caption;

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
                    hasSearch.Checked = node["HasSearch"].Get<bool>();
                    caption.Text = node["Caption"].Get<string>();
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindProperties();
            lblS.For = hasSearch.ClientID;
        }

        private void DataBindProperties()
        {
            foreach (Node idx in DataSource["Properties"])
            {
                int id = idx["ID"].Get<int>();
                string name = idx["Name"].Get<string>();

                LinkButton act = new LinkButton();
                act.Text = "Action ...";
                act.CssClass = "span-2 delete clear-left";
                act.ToolTip = "Search for an Action for this property ...";
                act.Click +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["ParentID"].Value = DataSource["ID"].Get<int>();

                        RaiseSafeEvent(
                            "Magix.Meta.FindAction",
                            node);
                    };
                properties.Controls.Add(act);

                LinkButton btn = new LinkButton();
                btn.Text = "Delete";
                btn.CssClass = "span-2 delete";
                btn.ToolTip = "Will irrevocably delete your Property";
                btn.Click +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["ParentID"].Value = DataSource["ID"].Get<int>();

                        RaiseSafeEvent(
                            "Magix.MetaView.DeleteProperty",
                            node);
                    };
                properties.Controls.Add(btn);

                InPlaceTextAreaEdit lbl = new InPlaceTextAreaEdit();
                lbl.CssClass += " type-editor span-4 last";
                lbl.Text = name;
                lbl.ToolTip = "The Name of your Property, will normally map into a Name on your MetaTypes";
                lbl.TextChanged +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["Name"].Value = lbl.Text;

                        RaiseSafeEvent(
                            "Magix.MetaView.ChangePropertyName",
                            node);
                    };
                properties.Controls.Add(lbl);

                Panel p2 = new Panel();
                p2.CssClass = "span-3";

                CheckBox ch = new CheckBox();
                ch.ID = "i-" + idx["ID"].Get<int>();
                ch.Checked = idx["ReadOnly"].Get<bool>();
                ch.CheckedChanged +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["ReadOnly"].Value = ch.Checked;

                        RaiseSafeEvent(
                            "Magix.MetaView.ChangePropertyReadOnly",
                            node);
                    };
                p2.Controls.Add(ch);

                Label lbl2 = new Label();
                lbl2.ID = "l-" + idx["ID"].Get<int>();
                lbl2.Text = "Read Only";
                lbl2.ToolTip = "If true, this is a read only field. Which might be useful for conveying information about some specific field or something ...";
                lbl2.Tag = "label";
                p2.Controls.Add(lbl2);
                lbl2.For = ch.ClientID;

                properties.Controls.Add(p2);

                InPlaceTextAreaEdit ds = new InPlaceTextAreaEdit();
                ds.CssClass += " type-editor span-4";
                ds.Text = idx["Description"].Get<string>();
                ds.ToolTip = "Description, will somehow help the user to understand what type of data entry field this is through different mechanisms according to the view";
                ds.TextChanged +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["MetaViewDescription"].Value = ds.Text;

                        RaiseSafeEvent(
                            "Magix.MetaView.ChangePropertyDescription",
                            node);
                    };
                properties.Controls.Add(ds);

                InPlaceTextAreaEdit action = new InPlaceTextAreaEdit();
                action.CssClass += " type-editor span-4 last";
                action.Text = idx["Action"].Get<string>();
                action.ToolTip = "Name of Event Associated with Control. Will highly likely turn your control into a button'ish type of control.";
                action.TextChanged +=
                    delegate
                    {
                        Node node = new Node();
                        node["ID"].Value = id;
                        node["MetaViewAction"].Value = action.Text;

                        RaiseSafeEvent(
                            "Magix.MetaView.ChangePropertyAction",
                            node);
                    };
                properties.Controls.Add(action);
            }
        }

        protected void lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["IsList"].Value = lst.SelectedIndex == 0;

            RaiseSafeEvent(
                "Magix.MetaView.ChangeTypeOfMetaView",
                node);
        }

        protected void type_TextChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["MetaTypeName"].Value = type.Text;

            RaiseSafeEvent(
                "Magix.MetaView.ChangeTypeOfMetaView",
                node);
        }

        protected void caption_TextChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["MetaViewCaption"].Value = caption.Text;

            RaiseSafeEvent(
                "Magix.MetaView.ChangeCaptionOfMetaView",
                node);
        }

        protected void create_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = DataSource["ID"].Value;

            RaiseSafeEvent(
                "Magix.MetaView.CreateProperty",
                node);

            ReDataBind();
        }

        protected void hasSearch_CheckedChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["HasSearch"].Value = hasSearch.Checked;

            RaiseSafeEvent(
                "Magix.MetaView.SetSearch",
                node);
        }

        protected void view_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = DataSource["ID"].Get<int>();

            RaiseSafeEvent(
                "Magix.MetaView.LoadWysiwyg",
                node);
        }

        private void ReDataBind()
        {
            Node node = new Node();
            node["ID"].Value = DataSource["ID"].Get<int>();

            RaiseEvent(
                "Magix.MetaView.EditMetaView",
                node);
        }
    }
}



