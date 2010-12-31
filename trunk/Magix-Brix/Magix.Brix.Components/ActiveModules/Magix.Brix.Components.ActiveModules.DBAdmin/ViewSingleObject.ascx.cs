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
    public class ViewSingleObject : DataBindableSingleInstanceControl, IModule
    {
        protected Panel pnl;
        protected Button change;
        protected Button remove;

        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    UpdateCaption();
                    if (node["EditingAllowed"].Get<bool>())
                    {
                        remove.Enabled = true;
                        change.Enabled = true;
                    }
                    else
                    {
                        remove.Enabled = false;
                        change.Enabled = false;
                    }
                };
        }

        protected void change_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ParentID"].Value = ParentID;
            node["ParentPropertyName"].Value = ParentPropertyName;
            node["ParentType"].Value = ParentFullType;
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.ChangeComplexInstance",
                node);
        }

        protected override void ReDataBind()
        {
            DataSource["ID"].Value = DataSource["Object"]["ID"].Value;
            DataSource["Object"].UnTie();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.UpdateSingleInstance",
                DataSource);
            pnl.Controls.Clear();
            UpdateCaption();
            DataBindObjects();
            pnl.ReRender();
        }

        protected void remove_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ParentID"].Value = ParentID;
            node["ParentPropertyName"].Value = ParentPropertyName;
            node["ParentType"].Value = ParentFullType;
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.RemoveReference",
                node);
            Node node2 = new Node();
            node2["ClientID"].Value = this.ClientID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ClearControlsForSpecificDynamic",
                node2);
        }

        protected override void DataBindObjects()
        {
            if (DataSource["Object"]["ID"].Get<int>() == 0)
                return;
            HtmlTable tb = new HtmlTable();
            tb.Attributes.Add("class", "viewObjects");

            // Header rows
            tb.Rows.Add(CreateHeaderRow());
            tb.Rows.Add(CreateIDRow());
            foreach (Node idxProp in DataSource["Object"]["Properties"])
            {
                tb.Rows.Add(CreateRow(idxProp));
            }
            pnl.Controls.Add(tb);
        }

        private HtmlTableRow CreateRow(Node idxProp)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell c1 = new HtmlTableCell();
            c1.InnerHtml = idxProp.Name;
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            c1.InnerHtml = idxProp["TypeName"].Get<string>();
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            if (idxProp["IsComplex"].Get<bool>())
            {
                LinkButton ed = new LinkButton();
                ed.Text = idxProp["Value"].Get<string>();
                ed.Info = idxProp["PropertyName"].Get<string>() + "|" + idxProp["IsList"].Value;
                ed.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        LinkButton lb = sender as LinkButton;
                        string propertyName = lb.Info.Split('|')[0];
                        bool isList = bool.Parse(lb.Info.Split('|')[1]);
                        Node node = new Node();
                        node["ID"].Value = ActiveID;
                        node["FullTypeName"].Value = FullTypeName;
                        node["PropertyName"].Value = propertyName;
                        if (isList)
                        {
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "DBAdmin.ViewList",
                                node);
                        }
                        else
                        {
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "DBAdmin.ViewSingleInstance",
                                node);
                        }
                    };
                c1.Controls.Add(ed);
            }
            else
            {
                TextAreaEdit ed = new TextAreaEdit();
                ed.Text = idxProp["Value"].Get<string>();
                ed.Info = idxProp["PropertyName"].Get<string>();
                ed.TextChanged +=
                    delegate(object sender, EventArgs e)
                    {
                        TextAreaEdit edit = sender as TextAreaEdit;
                        Node node = new Node();
                        node["ID"].Value = ActiveID;
                        node["FullTypeName"].Value = FullTypeName;
                        node["PropertyName"].Value = edit.Info;
                        node["Value"].Value = edit.Text;
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "DBAdmin.ChangeValue",
                            node);
                    };
                c1.Controls.Add(ed);
            }
            row.Cells.Add(c1);
            return row;
        }

        private HtmlTableRow CreateIDRow()
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell c1 = new HtmlTableCell();
            c1.InnerHtml = "ID";
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            c1.InnerHtml = "Int32";
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            c1.InnerHtml = this.ActiveID.ToString();
            row.Cells.Add(c1);
            return row;
        }

        private static HtmlTableRow CreateHeaderRow()
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("class", "header");
            HtmlTableCell c1 = new HtmlTableCell();
            c1.InnerHtml = "Name";
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            c1.InnerHtml = "Type";
            row.Cells.Add(c1);
            c1 = new HtmlTableCell();
            c1.InnerHtml = "Value";
            row.Cells.Add(c1);
            return row;
        }
    }
}




















