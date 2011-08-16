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

namespace Magix.Brix.Components.ActiveModules.Menu
{
    // TODO: Create a common SlidingMenu which can be shared as both the plugin one
    // and the admin one ...
    /**
     * Level2: Contains the UI for our SlidingMenu module, used in the administrator dashboard. Takes
     * a recursive 'Items' structure containing 'Caption' and 'Event' which will be raised
     * when clicked ['Event']
     */
    [ActiveModule]
    public class Slider : ActiveModule
    {
        protected SlidingMenu slid;
        protected SlidingMenuLevel root;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

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
            if (node.Contains("Selected") &&
                node["Selected"].Get<bool>())
                item.CssClass += " selected";
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

            SlidingMenuItem old = Selector.SelectFirst<SlidingMenuItem>(
                root,
                delegate(Control idx)
                {
                    return (idx is BaseWebControl) &&
                        (idx as BaseWebControl).CssClass.Contains(" selected");
                });
            if (old != null)
                old.CssClass = old.CssClass.Replace(" selected", "");
            item.CssClass += " selected";
        }
    }
}



