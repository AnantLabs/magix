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
    [PublisherPlugin]
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
                node["Event"]["Name"].Get<string>() + 
                "|" + 
                node["Event"]["MenuItemID"].Get<string>();

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

            node["MenuItemID"].Value = menuItemId;

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

        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(SliderMenu).FullName)
            {
                e.Params["Stop"].Value = true;
            }
        }
    }
}



