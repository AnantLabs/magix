/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewSingleObject : Module, IModule
    {
        protected Panel pnl;
        protected Button change;
        protected Button remove;
        protected Button focs;
        protected Panel changePnl;
        protected Panel removePnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    if (node.Contains("ChildCssClass"))
                    {
                        pnl.CssClass = node["ChildCssClass"].Get<string>();
                    }
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(focs))
                        .Render();
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBindObjects();
        }

        protected void change_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            node["ParentID"].Value = DataSource["ParentID"].Value;
            node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
            node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;
            RaiseSafeEvent(
                "DBAdmin.Form.ChangeObject",
                node);
        }

        protected void remove_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            node["ParentID"].Value = DataSource["ParentID"].Value;
            node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
            node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;
            RaiseSafeEvent(
                "DBAdmin.Data.RemoveObject",
                node);
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        protected void DataBindObjects()
        {
            if (DataSource["Object"]["ID"].Get<int>() != 0)
            {
                Label tb = new Label();
                tb.Tag = "table";
                tb.CssClass = "viewObjects singleInstance";

                // Header rows
                tb.Controls.Add(CreateHeaderRow());
                foreach (Node idxProp in DataSource["Object"]["Properties"])
                {
                    tb.Controls.Add(CreateRow(idxProp));
                }
                pnl.Controls.Add(tb);
            }
            DataBindDone();
        }

        protected void DataBindDone()
        {
            changePnl.Visible = DataSource["IsChange"].Get<bool>();
            removePnl.Visible = DataSource["IsRemove"].Get<bool>() &&
                DataSource.Contains("Object");
            string parentTypeName = 
                !DataSource.Contains("ParentFullTypeName") ? 
                "" : 
                DataSource["ParentFullTypeName"].Get<string>();

            string caption = "";
            if (DataSource.Contains("Caption"))
            {
                caption = DataSource["Caption"].Get<string>();
            }
            else
            {
                if (DataSource["ParentID"].Get<int>() > 0)
                {
                    parentTypeName =
                        parentTypeName.Substring(
                            parentTypeName.LastIndexOf(".") + 1);
                    if (DataSource.Contains("Object"))
                    {
                        caption = string.Format(
                            "{0}[{1}] of {2}[{3}]/{4}",
                            DataSource["TypeName"].Get<string>(),
                            DataSource["Object"]["ID"].Get<int>(),
                            parentTypeName.Substring(parentTypeName.LastIndexOf(".") + 1),
                            DataSource["ParentID"].Value,
                            DataSource["ParentPropertyName"].Value);
                    }
                }
                else
                {
                    if (DataSource.Contains("Object"))
                    {
                        caption = string.Format(
                            "{0}[{1}]",
                            DataSource["TypeName"].Get<string>(),
                            DataSource["Object"]["ID"].Get<int>());
                    }
                    else
                    {
                        caption = "Viewing an object...";
                    }
                }
            }
            if (Parent.Parent.Parent is Window)
            {
                (Parent.Parent.Parent as Window).Caption = caption;
            }
            else
            {
                Node node = new Node();
                node["Caption"].Value = caption;
                RaiseSafeEvent(
                    "Magix.Core.SetFormCaption",
                    node);
            }
        }

        private Label CreateRow(Node node)
        {
            Label row = new Label();
            row.Tag = "tr";

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Name") &&
                DataSource["WhiteListProperties"]["Name"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                c1.CssClass = "columnName";
                c1.Text = node.Name;
                row.Controls.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Type") &&
                DataSource["WhiteListProperties"]["Type"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                c1.CssClass = "columnType";
                c1.Text =
                    DataSource["Type"]["Properties"][node.Name]["TypeName"].Get<string>()
                        .Replace("<", "&lt;").Replace(">", "&gt;");
                row.Controls.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Attributes") &&
                DataSource["WhiteListProperties"]["Attributes"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                c1.CssClass = "columnType";
                string text = "";
                if (!DataSource["Type"]["Properties"][node.Name]["IsOwner"].Get<bool>())
                    text += "IsNotOwner ";
                if (DataSource["Type"]["Properties"][node.Name]["BelongsTo"].Get<bool>())
                    text += "BelongsTo ";
                if (!string.IsNullOrEmpty(DataSource["Type"]["Properties"][node.Name]["RelationName"].Get<string>()))
                    text += "'" + DataSource["Type"]["Properties"][node.Name]["RelationName"].Get<string>() + "'";
                c1.Text = text;
                row.Controls.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Value") &&
                DataSource["WhiteListProperties"]["Value"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                if (DataSource["Type"]["Properties"][node.Name]["IsComplex"].Get<bool>())
                {
                    LinkButton ed = new LinkButton();
                    ed.Text = node.Value.ToString();
                    ed.Info = node.Name;
                    if (DataSource["Type"]["Properties"][node.Name]["BelongsTo"].Get<bool>())
                        ed.CssClass = "belongsTo";
                    ed.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            LinkButton lb = sender as LinkButton;
                            Label ctrlOld = Magix.UX.Selector.SelectFirst<Label>(lb.Parent.Parent.Parent,
                                delegate(Control idxCtrl)
                                {
                                    BaseWebControl ctrl = idxCtrl as BaseWebControl;
                                    if (ctrl != null)
                                        return ctrl.CssClass == "grid-selected";
                                    return false;
                                });
                            if (ctrlOld != null)
                                ctrlOld.CssClass = "";
                            (lb.Parent.Parent as Label).CssClass = "grid-selected";
                            int id = DataSource["Object"]["ID"].Get<int>();
                            string column = lb.Info;
                            Node n = new Node();
                            n["ID"].Value = id;
                            n["PropertyName"].Value = column;
                            n["IsList"].Value = DataSource["Type"]["Properties"][column]["IsList"].Value;
                            n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                            RaiseSafeEvent(
                                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                                n);
                        };
                    c1.Controls.Add(ed);
                }
                else
                {
                    switch (DataSource["Type"]["Properties"][node.Name]["FullTypeName"].Get<string>())
                    {
                        default:
                            {
                                TextAreaEdit ed = new TextAreaEdit();
                                ed.TextLength = 500;
                                ed.Text = node.Value as string;
                                ed.CssClass += " larger";
                                ed.Info = node.Name;
                                ed.TextChanged +=
                                    delegate(object sender, EventArgs e)
                                    {
                                        TextAreaEdit edit = sender as TextAreaEdit;
                                        int id = DataSource["Object"]["ID"].Get<int>();
                                        string column = edit.Info;
                                        Node n = new Node();
                                        n["ID"].Value = id;
                                        n["PropertyName"].Value = column;
                                        n["NewValue"].Value = edit.Text;
                                        n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                                        RaiseSafeEvent(
                                            "DBAdmin.Data.ChangeSimplePropertyValue",
                                            n);
                                    };
                                c1.Controls.Add(ed);
                            } break;
                    }
                }
                row.Controls.Add(c1);
            }
            return row;
        }

        private HtmlTableRow CreateHeaderRow()
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("class", "header");

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Name") &&
                DataSource["WhiteListProperties"]["Name"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Name";
                c1.Attributes.Add("class", "wide-4");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Type") &&
                DataSource["WhiteListProperties"]["Type"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Type";
                c1.Attributes.Add("class", "wide-4");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Attributes") &&
                DataSource["WhiteListProperties"]["Attributes"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Attributes";
                c1.Attributes.Add("class", "wide-5");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Value") &&
                DataSource["WhiteListProperties"]["Value"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Value";
                c1.Attributes.Add("class", "wide-7");
                row.Cells.Add(c1);
            }
            return row;
        }

        protected override void ReDataBind()
        {
            if (DataSource["ParentID"].Get<int>() > 0)
            {
                DataSource["Object"].UnTie();
                DataSource["Type"].UnTie();
                if (RaiseSafeEvent(
                    "DBAdmin.Data.GetObjectFromParentProperty",
                    DataSource))
                {
                    pnl.Controls.Clear();
                    DataBindObjects();
                    pnl.ReRender();
                }
            }
            else
            {
                if (!DataSource.Contains("Object"))
                    return;
                DataSource["ID"].Value = DataSource["Object"]["ID"].Get<int>();
                DataSource["Object"].UnTie();
                DataSource["Type"].UnTie();
                if (RaiseSafeEvent(
                    "DBAdmin.Data.GetObject",
                    DataSource))
                {
                    pnl.Controls.Clear();
                    DataBindObjects();
                    pnl.ReRender();
                }
            }
            new EffectHighlight(Parent.Parent.Parent, 500)
                .Render();
        }
    }
}




















