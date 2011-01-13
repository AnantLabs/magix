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

namespace Magix.Brix.Components.ActiveModules.Menu
{
    [ActiveModule]
    public class Slider : UserControl, IModule
    {
        protected SlidingMenu slid;
        protected SlidingMenuLevel root;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindMenu();
        }

        private void DataBindMenu()
        {
            foreach (Node idx in DataSource["Items"])
            {
                CreateSingleItem(root, idx);
            }
        }

        private void CreateSingleItem(Control parent, Node node)
        {
            string caption = node["Caption"].Get<string>();
            string eventName = node["Event"]["Name"].Get<string>();
            SlidingMenuItem item = new SlidingMenuItem();
            item.Text = caption;
            item.Info = eventName;
            if (node.Contains("Items") && node["Items"].Count > 0)
            {
                SlidingMenuLevel level = new SlidingMenuLevel();
                foreach (Node idx in node["Items"])
                {
                    CreateSingleItem(level, idx);
                }
                item.Controls.Add(level);
            }
            parent.Controls.Add(item);
        }

        protected void slid_LeafMenuItemClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = sender as SlidingMenuItem;
            string eventName = item.Info;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName);
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}



