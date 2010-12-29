/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class Main : System.Web.UI.UserControl, IModule
    {
        protected TreeView tree;
        protected Window wnd;
        protected DynamicPanel popup;

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

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "popup")
            {
                wnd.Visible = true;
                int growX = -1;
                int growY = -1;
                if (e.Params["Parameters"].Contains("GrowX"))
                    growX = e.Params["Parameters"]["GrowX"].Get<int>();
                if (e.Params["Parameters"].Contains("GrowY"))
                    growY = e.Params["Parameters"]["GrowY"].Get<int>();
                if (growX > -1 || growY > -1)
                {
                    // Smoothingly animating size of window ...
                    new EffectSize(wnd, 500, growX, growY)
                        .ChainThese(
                            new EffectSize(popup, 500, growX - 50, growY - 50))
                        .Render();
                }
                if (e.Params["Parameters"].Contains("Caption"))
                {
                    wnd.Caption = e.Params["Parameters"]["Caption"].Get<string>();
                }
                if (true.Equals(e.Params["Parameters"].Contains("Append")))
                    popup.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(popup);
                    popup.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        protected void dynamic_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
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

        protected void wnd_Closed(object sender, EventArgs e)
        {
            ClearControls(popup);
        }

        private void ClearControls(DynamicPanel dynamic)
        {
            foreach (System.Web.UI.Control idx in dynamic.Controls)
            {
                ActiveEvents.Instance.RemoveListener(idx);
            }
            dynamic.ClearControls();
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
                if (idx["FullTypeName"].Value == null)
                    continue;
                TreeItem it = new TreeItem();
                it.ID = idx["FullTypeName"].Get<string>().Replace(".", "").Replace("+", "");
                if (idx["IsLeafClass"].Get<bool>())
                {
                    it.Info = "Leaf:" + idx["FullTypeName"].Get<string>();
                }
                else
                {
                    it.Info = idx["FullTypeName"].Get<string>();
                }
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
            TreeItem it = sender as TreeItem;
            string classFullName = it.Info;
            if (classFullName.IndexOf("Leaf:") == 0)
            {
                // Class ...!
                Node node = new Node();
                node["ClassName"].Value = classFullName.Replace("Leaf:", "");
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ViewClassDetails",
                    node);
            }
        }
    }
}




















