/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class ChildExcerpt : ActiveModule
    {
        protected Panel pnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    Node n = new Node();

                    n["ID"].Value = node["PageObjectTemplateID"].Value;
                    n["Count"].Value = PagesCount;

                    RaiseEvent(
                        "Magix.Publishing.GetLastChildrenPages",
                        n);
                    if (n.Contains("Items"))
                        node.Add(n["Items"].UnTie());
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null && DataSource.Contains("Items"))
                DataBindItems();
        }

        private void DataBindItems()
        {
            foreach (Node idx in DataSource["Items"])
            {
                int id = idx["ID"].Get<int>();

                Node node = new Node();
                node["ID"].Value = id;

                RaiseEvent(
                    "Magix.Publishing.BuildOneChildExcerptControl",
                    node);
                if (node.Contains("Control"))
                    pnl.Controls.Add(node["Control"].Value as Control);
            }
        }

        [ModuleSetting(ModuleEditorEventName = "Magix.Publishing.GetTemplateColumnSelectChildExcerptNo", DefaultValue="10")]
        public int PagesCount
        {
            get { return ViewState["PagesCount"] == null ? 10 : (int)ViewState["PagesCount"]; }
            set { ViewState["PagesCount"] = value; }
        }
    }
}



