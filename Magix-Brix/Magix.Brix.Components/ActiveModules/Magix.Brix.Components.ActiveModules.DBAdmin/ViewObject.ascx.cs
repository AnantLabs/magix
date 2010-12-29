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
                                string propertyName = infos[2];
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
                        btn.Text = value;
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




















