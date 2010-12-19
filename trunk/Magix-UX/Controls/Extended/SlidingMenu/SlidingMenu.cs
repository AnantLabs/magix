/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * This widget is a 'menu looka-like kinda control'. Its purpose is to be an alternative
     * way of displaying hierarchical choices for your users. But instead of 'popping up' like
     * menu items does in a conventional menu, they will 'slide in' from the side, making
     * a much more efficient way of handling your space on your web pages. For most navigational
     * purposes, this widget is a far better bet than the conventional Menu widget.
     */
    public class SlidingMenu : Panel
    {
        private readonly LinkButton _back = new LinkButton();

        /**
         * Raised when any SlidingMenuItem is clicked within the SlidingMenu hierarchy.
         * Notice that if you actually choose to create an event handler for this
         * event, then anything you do will trigger a server-request, and hence your
         * application will not be as responsive. You might, in most cases, choose to
         * only implement event handlers for the LeafMenuItemClicked instead of 
         * handling this event handler.
         */
        public event EventHandler MenuItemClicked;

        /**
         * Raised when a 'leaf menu item' is clicked. A Leaf MenuItem is a menu item
         * without any children. Since most menu items which have children are used
         * as groupings, and not really all that interesting to get notifications about
         * when clicked, this is probably the event you wish to handle, unless all
         * your menu items are URL menu items, in which case you don't even need to 
         * handle this event.
         */
        public event EventHandler LeafMenuItemClicked;

        public SlidingMenu()
        {
            CssClass = "mux-sliding-menu";
        }

        /**
         * The active SlidingMenuItem of your SlidingMenu. Use this one to see which 
         * menu item was clicked in for instance your event handlers for menu item 
         * clicked.
         */
        public string ActiveMenuItem
        {
            get { return ViewState["ActiveMenuItem"] == null ? "" : (string)ViewState["ActiveMenuItem"]; }
            private set { ViewState["ActiveMenuItem"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            CreateCompositeControl();
        }

        private void CreateCompositeControl()
        {
            _back.ID = "back";
            _back.CssClass = "mux-sliding-menu-back";
            _back.Text = "&lt;";
            _back.Click += BackButtonClicked;
            Controls.AddAt(0, _back);
        }

        private void BackButtonClicked(object sender, EventArgs e)
        {
            string idOfLevel = (sender as LinkButton).Info;
            SlidingMenuItem item = Selector.FindControl<SlidingMenuItem>(this, idOfLevel);
            int noLevels = -1;
            Control idx = item.Parent;
            SlidingMenuItem toBecomeActive = null;
            while (!(idx is SlidingMenu))
            {
                if (idx is SlidingMenuLevel)
                {
                    noLevels += 1;
                }
                else if (toBecomeActive == null && idx is SlidingMenuItem)
                {
                    toBecomeActive = idx as SlidingMenuItem;
                }
                idx = idx.Parent;
            }
            new EffectSlide(
                SlidingMenuLevel, 
                500, 
                -noLevels)
                .JoinThese(
                    new EffectFadeOut(item.SlidingMenuLevel, 500))
                .Render();
            if (toBecomeActive == null)
                ActiveMenuItem = "";
            else
                ActiveMenuItem = toBecomeActive.ID;
        }

        internal SlidingMenuLevel SlidingMenuLevel
        {
            get
            {
                foreach (Control idx in Controls)
                {
                    SlidingMenuLevel lev = idx as SlidingMenuLevel;
                    if (lev != null)
                        return lev;
                }
                return null;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (NoEventHandlerDefined)
            {
                _back.Style[Styles.visibility] = "visible";
                _back.Click -= BackButtonClicked;
                _back.ClickEffect = new EffectSlide(SlidingMenuLevel, 500, 0);
                foreach (SlidingMenuItem idx in SlidingMenuLevel.Views)
                {
                    if (idx.SlidingMenuLevel != null)
                    {
                        Effect tmp = new EffectFadeOut(idx.SlidingMenuLevel, 500);
                        _back.ClickEffect.JoinThese(tmp);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ActiveMenuItem))
                {
                    _back.Style[Styles.visibility] = "hidden";
                }
                else
                {
                    SlidingMenuItem item = Selector.FindControl<SlidingMenuItem>(this, ActiveMenuItem);
                    if (item.SlidingMenuLevel != null)
                    {
                        _back.Text = item.Text;
                        _back.Info = item.ID;
                        _back.Style[Styles.visibility] = "visible";
                    }
                    else
                    {
                        Control idx = item.Parent;
                        int noLevels = 0;
                        while (!(idx is SlidingMenu))
                        {
                            if (idx is SlidingMenuLevel)
                                noLevels += 1;
                            idx = idx.Parent;
                        }
                        if (noLevels >= 2)
                            _back.Style[Styles.visibility] = "visible";
                        else
                            _back.Style[Styles.visibility] = "hidden";
                    }
                }
            }
            base.OnPreRender(e);
        }

        internal void RaiseMenuItemClicked(SlidingMenuItem SlidingMenuItem, Panel slideIn)
        {
            if (!string.IsNullOrEmpty(ActiveMenuItem))
            {
                SlidingMenuItem old = Selector.FindControl<SlidingMenuItem>(this, ActiveMenuItem);
                old.CssClass = old.CssClass.Replace(" mux-sliding-menu-item-active", "");
            }
            SlidingMenuItem.CssClass += " mux-sliding-menu-item-active";
            ActiveMenuItem = SlidingMenuItem.ID;

            if (MenuItemClicked != null)
            {
                MenuItemClicked(SlidingMenuItem, new EventArgs());
            }
            if (LeafMenuItemClicked != null)
            {
                if (SlidingMenuItem.SlidingMenuLevel == null)
                {
                    LeafMenuItemClicked(SlidingMenuItem, new EventArgs());
                }
            }
            if (slideIn != null)
            {
                int noLevels = 0;
                Control tmp = SlidingMenuItem.Parent;
                while (tmp.ID != this.ID)
                {
                    if (tmp is SlidingMenuLevel)
                        noLevels += 1;
                    tmp = tmp.Parent;
                }
                new EffectSlide(SlidingMenuLevel, 500, -noLevels)
                    .JoinThese(new EffectFadeIn(slideIn, 500))
                    .Render();
            }
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }

        internal bool NoEventHandlerDefined
        {
            get { return MenuItemClicked == null; }
        }

        internal bool NoEmptyEventHandlerDefined
        {
            get { return LeafMenuItemClicked == null; }
        }
    }
}
