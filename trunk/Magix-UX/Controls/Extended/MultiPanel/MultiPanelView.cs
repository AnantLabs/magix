/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using System;

namespace Magix.UX.Widgets
{
    /**
     * Instances of items within a MultiPanel. These are the items a 
     * MultiPanel is made of. Most scenarios you would use this control within
     * would have only one of the MultiPanelViews visible at the same time.
     */
    public class MultiPanelView : Panel
    {
        // TODO: Comment ...
        public event EventHandler Activated;
        public event EventHandler DeActivated;

        public MultiPanelView()
        {
            CssClass = "mux-multi-panel-view";
            Tag = "li";
        }

        protected override void OnInit(System.EventArgs e)
        {
            // Notice that we've intentionally added up the setter to the CSS classes
            // twice since if they're not changed between OnInit and OnPreRender, 
            // they will not affect the ViewState
            SetCssClass();
            base.OnInit(e);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            SetCssClass();
            base.OnPreRender(e);
        }

        private void SetCssClass()
        {
            int idxCurrentVisibleView = (Parent.Parent as MultiPanel).ActiveMultiPanelViewIndex;
            int idxOfThisView = (Parent.Parent as MultiPanel)[this];
            if (idxCurrentVisibleView == idxOfThisView || 
                (Parent.Parent as MultiPanel).AnimationMode == MultiPanel.AnimationType.Slide)
            {
                Style[Styles.display] = "";
            }
            else
            {
                Style[Styles.display] = "none";
            }
        }

        internal void DeActivate()
        {
            if (DeActivated != null)
                DeActivated(this, new EventArgs());
        }

        internal void Activate()
        {
            if (Activated != null)
                Activated(this, new EventArgs());
        }
    }
}
