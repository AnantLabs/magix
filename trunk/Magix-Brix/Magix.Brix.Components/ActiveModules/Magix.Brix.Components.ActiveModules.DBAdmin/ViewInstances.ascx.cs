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
    public class ViewInstances : System.Web.UI.UserControl, IModule
    {
        protected Panel pnl;
        protected Button previous;
        protected Button next;

        void IModule.InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node["Objects"];
                    FullName = node["ActiveTypeFullName"].Get<string>();
                    previous.Text = string.Format("Previous {0} items", 
                        DataSource.Count);
                    next.Text = string.Format("Next {0} items, out of total {1}", 
                        DataSource.Count,
                        node["TotalCount"].Value);
                    TotalCount = node["TotalCount"].Get<int>();
                    CurrentIndex = node["Start"].Get<int>();
                    previous.Enabled = CurrentIndex > 0;
                    previous.Text = previous.Enabled ? string.Format("Previous {0} items", Settings.Instance.Get("NumberOfItemsInDatabaseManager", 50)) : "Previous";
                    next.Enabled = CurrentIndex + DataSource.Count < TotalCount;
                    next.Text = next.Enabled ? string.Format("Next {0} items", Math.Min(Settings.Instance.Get("NumberOfItemsInDatabaseManager", 50), TotalCount - CurrentIndex)) : "Next";
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindObjects();
        }

        private int CurrentIndex
        {
            get { return (int)ViewState["CurrentIndex"]; }
            set { ViewState["CurrentIndex"] = value; }
        }

        private int TotalCount
        {
            get { return (int)ViewState["TotalCount"]; }
            set { ViewState["TotalCount"] = value; }
        }

        protected void PreviousItems(object sender, EventArgs e)
        {
            if (CurrentIndex > 0)
            {
                Node node = new Node();
                node["FullTypeName"].Value = FullName;
                node["Start"].Value = CurrentIndex - DataSource.Count;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ViewAllInstances",
                    node);
            }
        }

        protected void NextItems(object sender, EventArgs e)
        {
            if (CurrentIndex < TotalCount)
            {
                Node node = new Node();
                node["FullTypeName"].Value = FullName;
                node["Start"].Value = CurrentIndex + DataSource.Count;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ViewAllInstances",
                    node);
            }
        }

        private string FullName
        {
            get { return ViewState["FullName"] as string; }
            set { ViewState["FullName"] = value; }
        }

        private void DataBindObjects()
        {
            if (DataSource.Count == 0)
                return;
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("class", "viewObjects");
            HtmlTableRow firstRow = new HtmlTableRow();
            firstRow.Attributes.Add("class", "header");
            foreach (Node idxCell in DataSource[0])
            {
                HtmlTableCell cell = new HtmlTableCell();
                if (idxCell.Name == "ID")
                    cell.InnerHtml = "ID";
                else
                {
                    cell.InnerHtml = string.Format(@"<span title=""{1}"">{2} (<span style=""color:#999;"">{0}</span>)</span>",
                        idxCell["Name"].Value,
                        idxCell["FullName"].Value,
                        idxCell["PropertyName"].Value);
                }
                firstRow.Cells.Add(cell);
            }
            table.Rows.Add(firstRow);
            foreach (Node idxRow in DataSource)
            {
                HtmlTableRow row = new HtmlTableRow();
                foreach (Node idxCell in idxRow)
                {
                    HtmlTableCell cell = new HtmlTableCell();
                    if (idxCell.Name == "ID")
                    {
                        // ID field...
                        cell.InnerHtml = idxCell.Value.ToString();
                    }
                    else
                    {
                        // Serializable property...
                        switch (idxCell["Name"].Get<string>())
                        {
                            case "String":
                            case "Boolean":
                            case "Int32":
                            case "Decimal":
                                InPlaceEdit edit = new InPlaceEdit();
                                edit.Text = idxCell["Value"].Value.ToString();
                                edit.Info =
                                    idxCell.Parent["ID"].Value.ToString() + "|" +
                                    idxCell["PropertyName"].Value;
                                edit.TextChanged +=
                                    delegate(object sender, EventArgs e)
                                    {
                                        InPlaceEdit ed = sender as InPlaceEdit;
                                        string info = ed.Info;
                                        Node node = new Node();
                                        string[] parts = info.Split('|');
                                        node["ID"].Value = int.Parse(parts[0]);
                                        node["PropertyName"].Value = parts[1];
                                        node["FullName"].Value = FullName;
                                        node["Value"].Value = ed.Text;
                                        ActiveEvents.Instance.RaiseActiveEvent(
                                            this,
                                            "ChangeDatabasePropertyValue",
                                            node);
                                    };
                                cell.Controls.Add(edit);
                                break;
                            default:
                                cell.InnerHtml = idxCell["Value"].Value.ToString();
                                break;
                        }
                    }
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }
            pnl.Controls.Add(table);
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




















