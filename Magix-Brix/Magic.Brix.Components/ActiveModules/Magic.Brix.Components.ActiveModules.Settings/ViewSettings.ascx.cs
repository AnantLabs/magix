using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.Settings
{
    [ActiveModule]
    public class ViewSettings : System.Web.UI.UserControl, IModule
    {
        protected global::Magix.Brix.Components.Grid grd;

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            node["Value"].Value = e.NewValue.ToString();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                EditEventName,
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DeleteEventName,
                node);
        }

        private string DeleteEventName
        {
            get { return ViewState["DeleteEventName"] as string; }
            set { ViewState["DeleteEventName"] = value; }
        }

        private string EditEventName
        {
            get { return ViewState["EditEventName"] as string; }
            set { ViewState["EditEventName"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    grd.DataSource = node["Grid"];
                    DeleteEventName = node["DeleteEventName"].Get<string>();
                    EditEventName = node["EditEventName"].Get<string>();
                };
        }
    }
}




