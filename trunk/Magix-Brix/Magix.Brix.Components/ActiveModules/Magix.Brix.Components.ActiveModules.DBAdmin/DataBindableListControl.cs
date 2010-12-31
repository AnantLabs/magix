/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Effects;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using System.Web.UI.HtmlControls;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    public abstract class DataBindableListControl : NestedDynamic, IModule
    {
        protected int Start
        {
            get { return DataSource["Start"].Get<int>(); }
        }

        protected int End
        {
            get { return DataSource["End"].Get<int>(); }
        }

        protected int Count
        {
            get { return DataSource["Objects"].Count; }
        }

        protected int MaxItems
        {
            get { return Settings.Instance.Get("DBAdmin.MaxItemsToShow", 50); }
        }

        protected virtual void UpdateCaption()
        {
            (Parent.Parent.Parent as Window).Caption = string.Format(
@"{0}-{1}/{2} of {3}",
                Start,
                End,
                TotalCount,
                TypeName);
        }

        protected HtmlTable CreateTable()
        {
            if (DataSource["Objects"].Count == 0)
                return null;
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("class", "viewObjects");
            return table;
        }

        protected HtmlTableRow CreateHeader(HtmlTable table)
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("class", "header");
            if (DataSource["IsSelect"].Get<bool>())
            {
                HtmlTableCell cS = new HtmlTableCell();
                cS.InnerHtml = "Select";
                row.Cells.Add(cS);
            }
            if (DataSource["IsRemove"].Get<bool>())
            {
                HtmlTableCell cS = new HtmlTableCell();
                cS.InnerHtml = "Remove";
                row.Cells.Add(cS);
            }
            HtmlTableCell cId = new HtmlTableCell();
            cId.InnerHtml = "ID";
            row.Cells.Add(cId);
            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                string propertyName = idx["Name"].Get<string>();
                bool isList = idx["IsList"].Get<bool>();
                string typeName = idx["TypeName"].Get<string>();
                bool belongsTo = idx["BelongsTo"].Get<bool>();
                bool isOwner = idx["IsOwner"].Get<bool>();
                string relationName = idx["RelationName"].Get<string>();
                HtmlTableCell cell = new HtmlTableCell();
                cell.InnerHtml = string.Format(
@"{0} ({1} {2} {3} {4})", 
                    propertyName, 
                    typeName,
                    ((!isOwner) ? "IsNotOwner" : ""),
                    (belongsTo ? "BelongsTo" : ""),
                    !string.IsNullOrEmpty(relationName) ? relationName : "");
                row.Cells.Add(cell);
            }
            return row;
        }

        protected void CreateCells(HtmlTable tbl)
        {
            foreach (Node idxObj in DataSource["Objects"])
            {
                HtmlTableRow row = new HtmlTableRow();
                if (DataSource["IsSelect"].Get<bool>())
                {
                    HtmlTableCell cS = new HtmlTableCell();
                    LinkButton btn = new LinkButton();
                    btn.Text = "Select";
                    btn.Info = idxObj["ID"].Value.ToString();
                    btn.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            Node node = new Node();
                            node["IsListAppend"].Value = DataSource["IsListAppend"].Value;
                            node["ParentID"].Value = ParentID;
                            node["ParentPropertyName"].Value = ParentPropertyName;
                            node["ParentType"].Value = ParentType;
                            node["NewObjectID"].Value = int.Parse((sender as LinkButton).Info);
                            node["NewObjectType"].Value = DataSource["FullTypeName"].Value;
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "DBAdmin." +
                                    DataSource["SelectEventToFire"].Get<string>(),
                                node);
                            ClearControls(child);
                            string parId = Parent.ClientID;
                            Node node2 = new Node();
                            node2["ClientID"].Value = this.ClientID;
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "ClearControlsForSpecificDynamic",
                                node2);
                            this.Close();
                        };
                    cS.Controls.Add(btn);
                    row.Cells.Add(cS);
                }
                if (DataSource["IsRemove"].Get<bool>())
                {
                    HtmlTableCell cS = new HtmlTableCell();
                    LinkButton btn = new LinkButton();
                    btn.Text = "Remove";
                    btn.Info = idxObj["ID"].Value.ToString();
                    btn.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            Node node = new Node();
                            node["ParentID"].Value = ParentID;
                            node["ParentPropertyName"].Value = ParentPropertyName;
                            node["ParentType"].Value = ParentFullType;
                            node["ObjectToRemoveID"].Value = int.Parse((sender as LinkButton).Info);
                            node["ObjectToRemoveType"].Value = DataSource["FullTypeName"].Value;
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "DBAdmin.ComplexInstanceRemoved",
                                node);
                            ReDataBind();
                        };
                    cS.Controls.Add(btn);
                    row.Cells.Add(cS);
                }
                int id = idxObj["ID"].Get<int>();
                HtmlTableCell idC = new HtmlTableCell();
                idC.InnerHtml = id.ToString();
                row.Cells.Add(idC);
                foreach (Node idxProp in idxObj["Properties"])
                {
                    HtmlTableCell c = new HtmlTableCell();
                    if (idxProp["IsList"].Get<bool>())
                    {
                        LinkButton b = new LinkButton();
                        b.Text = idxProp["Value"].Get<string>();
                        b.Info = 
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" + 
                            idxProp["PropertyName"].Get<string>() + "|" +
                            idxProp["BelongsTo"].Value;
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton bt = sender as LinkButton;
                                string[] infos = bt.Info.Split('|');
                                string parentPropertyName = infos[1];
                                int parentId = int.Parse(infos[0]);
                                bool belongsTo = bool.Parse(infos[2]);
                                string parentType = DataSource["FullTypeName"].Get<string>();
                                Node node = new Node();
                                node["FullTypeName"].Value = parentType;
                                node["ID"].Value = parentId;
                                node["PropertyName"].Value = parentPropertyName;
                                node["BelongsTo"].Value = belongsTo;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin.ViewList",
                                    node);
                            };
                        c.Controls.Add(b);
                    }
                    else if (idxProp["IsComplex"].Get<bool>())
                    {
                        LinkButton b = new LinkButton();
                        b.Text = idxProp["Value"].Get<string>();
                        b.Info =
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" +
                            idxProp["PropertyName"].Get<string>() + "|" +
                            idxProp["BelongsTo"].Value;
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton bt = sender as LinkButton;
                                string[] infos = bt.Info.Split('|');
                                string parentPropertyName = infos[1];
                                int parentId = int.Parse(infos[0]);
                                string parentType = DataSource["FullTypeName"].Get<string>();
                                bool belongsTo = bool.Parse(infos[2]);
                                Node node = new Node();
                                node["FullTypeName"].Value = parentType;
                                node["ID"].Value = parentId;
                                node["PropertyName"].Value = parentPropertyName;
                                node["BelongsTo"].Value = belongsTo;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin.ViewSingleInstance",
                                    node);
                            };
                        c.Controls.Add(b);
                    }
                    else
                    {
                        InPlaceEdit ed = new InPlaceEdit();
                        ed.Text = idxProp["Value"].Get<string>();
                        ed.Info =
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" +
                            idxProp["PropertyName"].Get<string>();
                        ed.TextChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                InPlaceEdit edit = sender as InPlaceEdit;
                                Node node = new Node();
                                string[] infos = edit.Info.Split('|');
                                string parentPropertyName = infos[1];
                                int parentId = int.Parse(infos[0]);
                                node["ID"].Value = parentId;
                                node["FullTypeName"].Value = FullTypeName;
                                node["PropertyName"].Value = parentPropertyName;
                                node["Value"].Value = edit.Text;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin.ChangeValue",
                                    node);
                            };
                        c.Controls.Add(ed);
                    }
                    row.Cells.Add(c);
                }
                tbl.Rows.Add(row);
            }
        }

        private void Close()
        {
            ClearControls(child);
            wnd.Visible = false;
        }
    }
}
