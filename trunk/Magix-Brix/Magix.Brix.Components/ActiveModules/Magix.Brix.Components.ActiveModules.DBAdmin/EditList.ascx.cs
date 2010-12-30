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
        protected Window wnd;
        protected DynamicPanel child;

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
            ClearControls(child);
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
            foreach (Node idxProp in DataSource["Objects"][0])
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
            table.Rows.Add(firstRow);

            // Building rows of table...
            foreach (Node idxCell in DataSource["Objects"])
            {
                if (idxCell.Name == "FullTypeName")
                    continue;
                HtmlTableRow row = new HtmlTableRow();
                foreach (Node idx in idxCell)
                {
                    if (idx.Name == "FullTypeName")
                        continue;
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
                                LinkButton btn = new LinkButton();
                                btn.Info = idx["Name"].Value.ToString() + "|" +
                                    idx.Parent["ID"].Value + "|" +
                                    idx["FullTypeName"].Get<string>();
                                btn.Text = idx["Value"].Value.ToString();
                                bool isList = 
                                    idx["TypeName"].Get<string>().Contains("LazyList&lt;") ||
                                    idx["TypeName"].Get<string>().Contains("List&lt;");
                                btn.Click +=
                                    delegate(object sender, EventArgs e)
                                    {
                                        LinkButton b = sender as LinkButton;
                                        string[] infos2 = b.Info.Split('|');
                                        string parentPropertyName = infos2[0];
                                        string fullTypeName2 = infos2[1];
                                        Node node = new Node();
                                        node["IDOfParent"].Value = int.Parse(infos2[1]);
                                        node["PropertyName"].Value = parentPropertyName;
                                        if (isList)
                                        {
                                            node["ParentFullName"].Value = infos2[2];
                                            node["ListGenericArgument"].Value = fullTypeName2;
                                            ActiveEvents.Instance.RaiseActiveEvent(
                                                this,
                                                "EditObjectInstances",
                                                node);
                                        }
                                        else
                                        {
                                            node["ParentFullName"].Value = infos2[2];
                                            node["FullName"].Value = infos2[2];
                                            if (b.Text != "[null]")
                                                node["ID"].Value = int.Parse(b.Text);
                                            ActiveEvents.Instance.RaiseActiveEvent(
                                                this,
                                                "EditObjectInstance",
                                                node);
                                        }
                                    };
                                cell.Controls.Add(btn);
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




















