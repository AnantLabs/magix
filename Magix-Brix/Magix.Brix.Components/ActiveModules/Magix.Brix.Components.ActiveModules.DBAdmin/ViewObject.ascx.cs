/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewObject : System.Web.UI.UserControl, IModule
    {
        protected Panel pnl;
        protected System.Web.UI.WebControls.Repeater rep;
        protected Window wnd;
        protected DynamicPanel child;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    DataBindGrid();
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BuildGridChildControls();
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

        private void DataBindGrid()
        {
            rep.DataSource = DataSource["Object"];
            rep.DataBind();
        }

        private void BuildGridChildControls()
        {
            foreach (Panel idx in Selector.Select<Panel>(rep,
                delegate(System.Web.UI.Control idx)
                {
                    return (idx is BaseWebControl) && (idx as BaseWebControl).CssClass == "templateField";
                }))
            {
                string[] infos = idx.Info.Split('|');
                string value = infos[0];
                string typeName = infos[1];
                string propertyName = infos[2];
                string fullTypeName = infos[3];
                switch (typeName)
                {
                    case "String":
                    case "Boolean":
                    case "Int32":
                    case "Decimal":
                        InPlaceEdit edit = new InPlaceEdit();
                        edit.Text = value;
                        edit.Info = DataSource["ID"].Value.ToString() + "|" + infos[2];
                        edit.TextChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                InPlaceEdit ed = sender as InPlaceEdit;
                                string[] infs = ed.Info.Split('|');
                                int id = int.Parse(infs[0]);
                                string fullName = DataSource["FullName"].Get<string>();
                                Node node = new Node();
                                node["ID"].Value = id;
                                node["FullName"].Value = fullName;
                                node["PropertyName"].Value = propertyName;
                                node["Value"].Value = ed.Text;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "ChangeDatabasePropertyValue",
                                    node);
                            };
                        idx.Controls.Add(edit);
                        break;
                    default:
                        LinkButton btn = new LinkButton();
                        btn.Info = propertyName + "|" + fullTypeName;
                        btn.Text = value;
                        bool isList = typeName.Contains("LazyList&lt;") || typeName.Contains("List&lt;");
                        btn.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton b = sender as LinkButton;
                                string[] infos2 = b.Info.Split('|');
                                string parentPropertyName = infos2[0];
                                string fullTypeName2 = infos2[1];
                                Node node = new Node();
                                node["IDOfParent"].Value = DataSource["ID"].Get<int>();
                                node["PropertyName"].Value = parentPropertyName;
                                if (isList)
                                {
                                    node["ParentFullName"].Value = DataSource["FullName"].Get<string>();
                                    node["ListGenericArgument"].Value = fullTypeName2;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "EditObjectInstances",
                                        node);
                                }
                                else
                                {
                                    if (b.Text != "[null]")
                                        node["ID"].Value = int.Parse(b.Text);
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "EditObjectInstance",
                                        node);
                                }
                            };
                            idx.Controls.Add(btn);
                            break;
                }
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




















