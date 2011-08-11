/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * A MultiPanel is a collection of MultiPanelViews, often used to create TabControls or
     * similar constructs. Often combined with TabStrip and TabButton. This control can
     * be configured extensively to mimick a lot of different scenarios. Hence we don't 
     * call it TabControl, although that happens to be one of the sub-scenarios you can
     * create with it.
     */
    public class MultiPanel : CompositeViewCollectionControl<MultiPanelView>
    {
        /**
         * How the animation of the MultiPanelViews are going to happen.
         */
        public enum AnimationType
        {
            /**
             * Both of the effects will execute simultaneously
             */
            Joined,

            /**
             * The collapse will execute before the opening of a new MultiPanelView
             */
            Chained,

            /**
             * The MultiPanelView will slide
             */
            Slide,

            /**
             * No animations, MultiPanelViews will "jump"
             */
            NoAnimations
        };

        public MultiPanel()
        {
            CssClass = "mux-multi-panel";
        }

        /**
         * The Speed of the Animation when switching active view.
         */
        public int AnimationDuration
        {
            get { return ViewState["AnimationDuration"] == null ? 500 : (int)ViewState["AnimationDuration"]; }
            set { ViewState["AnimationDuration"] = value; }
        }

        /**
         * Active/visible view. Will only work if MultiButtonClicked event handler is defined 
         * for MultiButton, or you run your own "change logic"...
         */
        public int ActiveMultiPanelViewIndex
        {
            get { return ViewState["ActiveMultiPanelViewIndex"] == null ? 0 : (int)ViewState["ActiveMultiPanelViewIndex"]; }
            set { ViewState["ActiveMultiPanelViewIndex"] = value; }
        }

        /**
         * How to animate AccordionViews when opened, see the AnimationType for details.
         */
        public AnimationType AnimationMode
        {
            get { return ViewState["AnimationMode"] == null ? AnimationType.Joined : (AnimationType)ViewState["AnimationMode"]; }
            set { ViewState["AnimationMode"] = value; }
        }

        // Overridden to make sure our base class works correctly ...
        protected override ControlCollection ViewControlCollection
        {
            get { return Content.Controls; }
        }

        /**
         * Sets the Active MultiPanelView of this instance according to the index passed in.
         */
        public void SetActiveView(int index)
        {
            SetActiveView(this[index]);
        }

        /**
         * Sets the Active MultiPanelView of this instance according to the 
         * MultiPanelView passed in.
         */
        public void SetActiveView(MultiPanelView selectedView)
        {
            // Getting the index of the previously selected AccordionView
            MultiPanelView previouslySelectedView = this[ActiveMultiPanelViewIndex];
            previouslySelectedView.DeActivate();
            selectedView.Activate();

            // Checking to see if the newly selected one is the same as the
            // previously selected one, and if so just returning since we then shouldn't
            // do any of the below logic.
            if (previouslySelectedView.ID == selectedView.ID)
                return;

            // Updating the currently selected AccordionView
            ActiveMultiPanelViewIndex = this[selectedView];

            switch(AnimationMode)
            {
                case AnimationType.NoAnimations:
                    previouslySelectedView.Style[Styles.display] = "none";
                    selectedView.Style[Styles.display] = "block";
                    break;
                case AnimationType.Joined:
                case AnimationType.Chained:
                    {
                        List<Control> ctrls = new List<Control>();
                        foreach (MultiPanelView idx in Views)
                        {
                            ctrls.Add(idx);
                        }
                        new EffectToggleElements(
                            selectedView,
                            AnimationDuration,
                            AnimationMode.ToString(),
                            ctrls)
                            .Render();
                    } break;
                case AnimationType.Slide:
                    {
                        new EffectSlide(
                            Content,
                            AnimationDuration,
                            ActiveMultiPanelViewIndex == 0 ? 0 : -ActiveMultiPanelViewIndex)
                            .Render();
                    } break;
            }
        }

        protected override void CreateCompositeControl()
        {
            SetCssClass();
            Content.Tag = "ul";
            Content.CssClass = "mux-multi-panel-content";
            base.CreateCompositeControl();
        }

        private void SetCssClass()
        {
            if (AnimationMode == AnimationType.Slide)
            {
                if (!CssClass.Contains(" mux-multi-panel-slide"))
                {
                    CssClass += " mux-multi-panel-slide";
                }
            }
            else
            {
                if (CssClass.Contains(" mux-multi-panel-slide"))
                {
                    CssClass = CssClass.Replace(" mux-multi-panel-slide", "");
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetCssClass();
            base.OnPreRender(e);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }
    }
}
