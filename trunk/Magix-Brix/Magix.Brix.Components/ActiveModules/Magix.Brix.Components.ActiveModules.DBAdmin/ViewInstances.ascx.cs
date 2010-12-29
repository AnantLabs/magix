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
        protected Window wnd;
        protected DynamicPanel child;

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
                    previous.Text = 
                        previous.Enabled ? 
                        string.Format(
                            "Previous {0} items", 
                            Math.Min(
                                Settings.Instance.Get("NumberOfItemsInDatabaseManager", 50),
                                CurrentIndex)) :
                    "Previous";
                    next.Enabled = CurrentIndex + DataSource.Count < TotalCount;
                    next.Text = 
                        next.Enabled ? 
                            string.Format(
                                "Next {0} items", 
                                Math.Min(
                                    Settings.Instance.Get("NumberOfItemsInDatabaseManager", 50), 
                                    TotalCount - (CurrentIndex + DataSource.Count))) :
                            "Next";
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindObjects();
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "child" && child.Controls.Count == 0)
            {
                wnd.Visible = true;
                if (e.Params["Parameters"].Contains("Caption"))
                {
                    wnd.Caption = e.Params["Parameters"]["Caption"].Get<string>();
                }
                if (true.Equals(e.Params["Parameters"].Contains("Append")))
                    child.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(child);
                    child.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        protected void child_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
        {
            DynamicPanel dynamic = sender as DynamicPanel;
            System.Web.UI.Control ctrl = PluginLoader.Instance.LoadActiveModule(e.Key);
            if (e.FirstReload)
            {
                ctrl.Init +=
                    delegate
                    {
                        IModule module = ctrl as IModule;
                        if (module != null)
                        {
                            module.InitialLoading(e.Extra as Node);
                        }
                    };
            }
            dynamic.Controls.Add(ctrl);
        }

        private void ClearControls(DynamicPanel dynamic)
        {
            foreach (System.Web.UI.Control idx in dynamic.Controls)
            {
                ActiveEvents.Instance.RemoveListener(idx);
            }
            dynamic.ClearControls();
        }

        protected void wnd_Closed(object sender, EventArgs e)
        {
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
                    cell.InnerHtml = string.Format(@"<span title=""{1}"">{2} ( <span style=""color:#999;"">{0}</span> {3} {4} {5})</span>",
                        idxCell["Name"].Value,
                        idxCell["FullName"].Value,
                        idxCell["PropertyName"].Value,
                        idxCell["BelongsTo"].Get<bool>() ? "BelongsTo" : "",
                        idxCell["IsOwner"].Get<bool>() ? "" : "IsNotOwner",
                        string.IsNullOrEmpty(idxCell["RelationName"].Get<string>()) ? "" : idxCell["RelationName"].Get<string>());
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
                                LinkButton btn = new LinkButton();
                                btn.Info =
                                    idxCell.Parent["ID"].Value.ToString() + "|" +
                                    idxCell["PropertyName"].Value + "|" +
                                    idxCell["BelongsTo"].Get<bool>().ToString().ToLower();
                                btn.Text = idxCell["Value"].Value.ToString();
                                btn.Click +=
                                    delegate(object sender, EventArgs e)
                                    {
                                        LinkButton b = sender as LinkButton;
                                        Node node = new Node();
                                        string[] parts = b.Info.Split('|');
                                        node["IDOfParent"].Value = int.Parse(parts[0]);
                                        node["ID"].Value = int.Parse(b.Text);
                                        node["PropertyName"].Value = parts[1];
                                        node["BelongsTo"].Value = bool.Parse(parts[2]);
                                        node["FullName"].Value = FullName;

                                        ActiveEvents.Instance.RaiseActiveEvent(
                                            this,
                                            "EditObjectInstance",
                                            node);
                                    };
                                cell.Controls.Add(btn);
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




















