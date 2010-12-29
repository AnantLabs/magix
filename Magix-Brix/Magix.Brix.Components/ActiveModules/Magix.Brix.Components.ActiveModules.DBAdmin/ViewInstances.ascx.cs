using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using System.Web.UI.HtmlControls;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewInstances : System.Web.UI.UserControl, IModule
    {
        protected Panel pnl;

        void IModule.InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node["Objects"];
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
                        cell.InnerHtml = idxCell["Value"].Value.ToString();
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




















