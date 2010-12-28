using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class Main : System.Web.UI.UserControl, IModule
    {
        protected TreeView tree;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    Node tmp = new Node();
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "GetDBAdminClasses",
                        tmp);
                    DataBase = tmp;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataBase != null)
                DataBindWholeTree();
        }

        private void DataBindWholeTree()
        {
            DataBindTree(DataBase["Classes"], tree);
        }

        private Node DataBase
        {
            get { return ViewState["DataBase"] as Node; }
            set { ViewState["DataBase"] = value; }
        }

        private void DataBindTree(Node tmp, System.Web.UI.Control ctrl)
        {
            foreach (Node idx in tmp)
            {
                if (idx.Name == "Count" || idx.Name == "FullTypeName" || idx.Name == "Name")
                    continue;
                TreeItem it = new TreeItem();
                it.ID = idx["FullTypeName"].Get<string>().Replace(".", "").Replace("+", "");
                it.Info = idx["FullTypeName"].Get<string>();
                it.Text = idx["Name"].Get<string>();
                DataBindTree(idx, it);
                if (ctrl is TreeItem)
                {
                    (ctrl as TreeItem).Content.Controls.Add(it);
                }
                else
                {
                    ctrl.Controls.Add(it);
                }
            }
        }

        protected void tree_SelectedItemChanged(object sender, EventArgs e)
        {
        }
    }
}




