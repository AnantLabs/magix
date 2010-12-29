/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class EditList : System.Web.UI.UserControl, IModule
    {
        protected Panel pnl;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                DataSource = node;
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindObjects();
        }

        private void DataBindObjects()
        {
            if (DataSource["Objects"].Count == 0)
                return;

            // Building header of table...
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("class", "viewObjects");
            HtmlTableRow firstRow = new HtmlTableRow();
            firstRow.Attributes.Add("class", "header");
            foreach (Node idxCell in DataSource["Objects"])
            {
                foreach (Node idxProp in idxCell)
                {
                    if (idxProp.Name == "FullTypeName")
                        continue;
                    HtmlTableCell cell = new HtmlTableCell();
                    if (idxProp.Name == "ID")
                        cell.InnerHtml = "ID";
                    else
                    {
                        cell.InnerHtml = string.Format(
    @"<span title=""{1}"">{2} ( <span style=""color:#999;"">{0}</span> {3} {4} {5})</span>",
                            idxProp["TypeName"].Value,
                            idxProp["FullTypeName"].Value,
                            idxProp["Name"].Value,
                            idxProp["BelongsTo"].Get<bool>() ? "BelongsTo" : "",
                            idxProp["IsOwner"].Get<bool>() ? "" : "IsNotOwner",
                            string.IsNullOrEmpty(idxProp["RelationName"].Get<string>()) ? "" : idxProp["RelationName"].Get<string>());
                    }
                    firstRow.Cells.Add(cell);
                }
            }
            table.Rows.Add(firstRow);

            // Building rows of table...
            foreach (Node idxCell in DataSource["Objects"])
            {
                if (idxCell.Name == "FullTypeName")
                    continue;
                HtmlTableRow row = new HtmlTableRow();
                foreach (Node idx in idxCell)
                {
                    HtmlTableCell cell = new HtmlTableCell();
                    if (idx.Name == "ID")
                        cell.InnerHtml = idx.Value.ToString();
                    else
                    {
                        switch (idx["TypeName"].Get<string>())
                        {
                            case "String":
                            case "Boolean":
                            case "Int32":
                            case "Decimal":
                                InPlaceEdit edit = new InPlaceEdit();
                                edit.Text = idx["Value"].Value.ToString();
                                edit.Info = idx["Name"].Value.ToString() + "|" +
                                    idx.Parent["ID"].Value;
                                edit.TextChanged +=
                                    delegate(object sender, EventArgs e)
                                    {
                                        InPlaceEdit ed = sender as InPlaceEdit;
                                        string info = ed.Info;
                                        Node node = new Node();
                                        string[] pars = info.Split('|');
                                        node["ID"].Value = int.Parse(pars[1]);
                                        node["FullName"].Value = DataSource["FullTypeName"].Get<string>();
                                        node["PropertyName"].Value = pars[0];
                                        node["Value"].Value = ed.Text;
                                        ActiveEvents.Instance.RaiseActiveEvent(
                                            this,
                                            "ChangeDatabasePropertyValue",
                                            node);
                                    };
                                cell.Controls.Add(edit);
                                break;
                            default:
                                cell.InnerHtml = string.Format(@"{0}", idx["Value"].Get<string>());
                                break;
                        }
                    }
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }

            // Adding table to Panel
            pnl.Controls.Add(table);
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




















