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
                            idxProp["PropertyName"].Get<string>();
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton bt = sender as LinkButton;
                                string[] infos = bt.Info.Split('|');
                                string parentPropertyName = infos[1];
                                int parentId = int.Parse(infos[0]);
                                string parentType = DataSource["FullTypeName"].Get<string>();
                                Node node = new Node();
                                node["FullTypeName"].Value = parentType;
                                node["ID"].Value = parentId;
                                node["PropertyName"].Value = parentPropertyName;
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
                            idxProp["PropertyName"].Get<string>();
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton bt = sender as LinkButton;
                                string[] infos = bt.Info.Split('|');
                                string parentPropertyName = infos[1];
                                int parentId = int.Parse(infos[0]);
                                string parentType = DataSource["FullTypeName"].Get<string>();
                                Node node = new Node();
                                node["FullTypeName"].Value = parentType;
                                node["ID"].Value = parentId;
                                node["PropertyName"].Value = parentPropertyName;
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
    }
}
