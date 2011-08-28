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
     * Level2: PublisherPlugin containing the UI for the Sliding Menu Publisher Plugin.
     * Meaning the default menu to the left in the front-web parts, which you can choose
     * to inject into a WebPartTemplate in one of your Templates if you wish. Basically
     * just loads the items once through raising the 'Magix.Publishing.GetSliderMenuItems'
     * event, which should return the items as a 'Items' list, containing 'Caption',
     * 'Event', 'Event' [Event name] and 'Event/WebPageURL' which normally will contain
     * the page's URL
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class SliderMenu : ActiveModule
    {
        protected SlidingMenu slid;
        protected SlidingMenuLevel root;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    RaiseEvent(
                        "Magix.Publishing.GetSliderMenuItems",
                        node);
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
            string eventName = 
                node["Event"].Get<string>() + 
                "|" +
                node["Event"]["WebPageURL"].Get<string>();

            SlidingMenuItem item = new SlidingMenuItem();
            item.ID = node.Name;
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

        protected void slid_MenuItemClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = sender as SlidingMenuItem;
            string eventName = item.Info.Split('|')[0];
            string menuItemId = item.Info.Split('|')[1];

            Node node = new Node();

            node["WebPageURL"].Value = menuItemId;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);

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

        protected void slid_BackClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = Selector.FindControl<SlidingMenuItem>(
                slid,
                slid.ActiveMenuItem);
            string eventName = item.Info.Split('|')[0];
            string menuItemId = item.Info.Split('|')[1];

            Node node = new Node();

            node["WebPageURL"].Value = menuItemId;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);

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

        /**
         * Level2: Will return false if this webpart can just be 'reused' to the next page
         */
        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(SliderMenu).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;
            }
        }
    }
}
