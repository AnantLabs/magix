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
     * Level2: PublisherPlugin containing a conventional 'File Menu' 
     * type of menu which normally is expected to find at the
     * top of a page, which will 'drop down' child selection boxes for being able to select
     * children. Useful for conventional applications, which should look like legacy code,
     * or something. Takes the exact same input parameters as the SliderMenu PublisherPlugin
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class TopMenu : ActiveModule
    {
        protected Menu slid;

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
                CreateSingleItem(slid, idx);
            }
        }

        private void CreateSingleItem(Control parent, Node node)
        {
            string caption = node["Caption"].Get<string>();
            string eventName = 
                node["Event"].Get<string>() + 
                "|" +
                node["Event"]["WebPageURL"].Get<string>();

            MenuItem item = new MenuItem();
            item.ID = node.Name;
            if (node.Contains("Selected") &&
                node["Selected"].Get<bool>())
                item.CssClass += " selected";

            item.Text = caption;
            item.Info = eventName;
            if (node.Contains("Items") && node["Items"].Count > 0)
            {
                SubMenu level = new SubMenu();
                foreach (Node idx in node["Items"])
                {
                    CreateSingleItem(level, idx);
                }
                item.Controls.Add(level);
            }
            parent.Controls.Add(item);
        }

        protected void slid_OnMenuItemClicked(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string eventName = item.Info.Split('|')[0];
            string menuItemId = item.Info.Split('|')[1];

            Node node = new Node();

            node["WebPageURL"].Value = menuItemId;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
        }

        /**
         * Level2: Will return false if this webpart can just be 'reused' to the next page
         */
        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(TopMenu).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;
            }
        }
    }
}



