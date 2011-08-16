/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level1: Will show an excerpt of all its children, sorted with newest first, showing a maximum of
     * PagesCount items. Kind of like a front page to a blog or a news website or something
     */
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

                    n["ID"].Value = node["OriginalWebPartID"].Value;
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

        /**
         * Level1: How many of the latest pages will be shown in the excerpt
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.Publishing.GetTemplateColumnSelectChildExcerptNo", DefaultValue="10")]
        public int PagesCount
        {
            get { return ViewState["PagesCount"] == null ? 10 : (int)ViewState["PagesCount"]; }
            set { ViewState["PagesCount"] = value; }
        }
    }
}



