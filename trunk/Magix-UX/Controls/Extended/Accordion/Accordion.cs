/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * Accordions are panels, or AccordionViews grouped together where you can 
     * expand and collapse individual AccordionViews within a group. Normally 
     * only one AccordionView can be visible at the same time, but this is 
     * configurable.
     */
    public class Accordion : ViewCollectionControl<AccordionView>
    {
        /**
         * How the animation of the AccordionViews are going to happen.
         */
        public enum AnimationType
        {
            /**
             * Both of the effects will execute simultaneously
             */
            Joined,

            /**
             * The collapse will execute before the opening of a new AccordionView
             */
            Chained,

            /**
             * Clicking an AccordionView will not close any previously opened
             * AccordionViews, but clicking an open AccordionView will close it.
             */
            MultipleOpen,

            /**
             * No animations, AccordionViews will "jump"
             */
            NoAnimations
        };

        /**
         * What to trigger the expansion
         */
        public enum ExpansionTrigger
        {
            /**
             * Click will trigger the expansion of the AccordionView
             */
            Click,

            /**
             * MouseOver will trigger the expansion of the AccordionView
             */
            MouseOver,

            /**
             * Double-click will trigger the expansion of the AccordionView
             */
            DblClick,

            /**
             * Will show the Accordion as long as the Mouse is hovering over it.
             * When the mouse is brought away from the AccordionView it will collapse.
             */
            Hover
        };

        public Accordion()
        {
            CssClass = "mux-accordion";
            Tag = "ul";
        }

        /**
         * Event raised when the active AccordionView is changed. If you allow multiple
         * AccordionViews to be opened at the same time, then this event will not be raised.
         */
        public event EventHandler ActiveAccordionViewChanged;

        /**
         * Index of the current Active AccordionView. If you allow multiple AccordionViews to be
         * open at the same time, then this value doesn't make much sense.
         */
        public int ActiveAccordionViewIndex
        {
            get { return ViewState["ActiveAccordionViewIndex"] == null ? 0 : (int)ViewState["ActiveAccordionViewIndex"]; }
            set { ViewState["ActiveAccordionViewIndex"] = value; }
        }

        /**
         * How to animate AccordionViews when opened, see the AnimationType for details.
         */
        public AnimationType AnimationMode
        {
            get { return ViewState["AnimationMode"] == null ? AnimationType.Joined : (AnimationType)ViewState["AnimationMode"]; }
            set { ViewState["AnimationMode"] = value; }
        }

        /**
         * What to trigger the opening of the View, see the ExpansionTrigger for details.
         */
        public ExpansionTrigger ExpansionMode
        {
            get { return ViewState["ExpansionMode"] == null ? ExpansionTrigger.Click : (ExpansionTrigger)ViewState["ExpansionMode"]; }
            set { ViewState["ExpansionMode"] = value; }
        }

        /**
         * How many milliseconds it will take to animate when an accordion is opened. This property
         * doesn't make sense if you're using the Accordion in "NoAnimation" mode.
         */
        public int AnimationDuration
        {
            get { return ViewState["AnimationDuration"] == null ? 500 : (int)ViewState["AnimationDuration"]; }
            set { ViewState["AnimationDuration"] = value; }
        }

        /**
         * Will set the active AccordionView of the Accordion to the specified index.
         */
        public void SetActiveView(int index)
        {
            SetActiveView(this[index]);
        }

        /**
         * Sets the active AccordionView to the given one.
         */
        public void SetActiveView(AccordionView selectedAccordionView)
        {
            SetActiveView(selectedAccordionView, false);
        }

        internal void SetActiveView(AccordionView selectedAccordionView, bool raiseEvent)
        {
            AccordionView previouslySelectedAccView = this[ActiveAccordionViewIndex];

            if (AnimationMode != AnimationType.MultipleOpen && 
                previouslySelectedAccView.ID == selectedAccordionView.ID)
                return;

            ActiveAccordionViewIndex = this[selectedAccordionView];

            UpdateStylesAndAnimateActiveView(
                selectedAccordionView, 
                previouslySelectedAccView);

            // Raising our event, but only if an event handler is defined.
            if (raiseEvent && ActiveAccordionViewChanged != null)
            {
                ActiveAccordionViewChanged(this, new EventArgs());
            }
        }

        internal void AddActiveView(AccordionView accordionView)
        {
            // Updating the currently selected AccordionView
            ActiveAccordionViewIndex = this[accordionView];

            new EffectRollDown(
                accordionView.Content, 
                AnimationDuration)
                .Render();

            // Raising our event, but only if an event handler is defined.
            if (ActiveAccordionViewChanged != null)
            {
                ActiveAccordionViewChanged(this, new EventArgs());
            }
        }

        internal void RemoveActiveView(AccordionView accordionView)
        {
            // Updating the currently selected AccordionView
            ActiveAccordionViewIndex = -1;

            new EffectRollUp(
                accordionView.Content, 
                AnimationDuration)
                .Render();

            // Raising our event, but only if an event handler is defined.
            if (ActiveAccordionViewChanged != null)
            {
                ActiveAccordionViewChanged(this, new EventArgs());
            }
        }

        private void UpdateStylesAndAnimateActiveView(
            AccordionView selectedAccordionView, 
            AccordionView previouslySelectedAccView)
        {
            // Here we create animations to hide the previously selected on 
            // and show the currently selected
            if (AnimationMode == AnimationType.Joined)
            {
                // Joined effects, executing simultaneously
                new EffectRollUp(previouslySelectedAccView.Content, AnimationDuration)
                    .JoinThese(
                        new EffectRollDown(
                            selectedAccordionView.Content, AnimationDuration))
                    .Render();
            }
            else if (AnimationMode == AnimationType.Chained)
            {
                // Chained effects, first rolling up the previously selected
                // and then rolling down the newly selected one
                new EffectRollUp(previouslySelectedAccView.Content, AnimationDuration)
                    .ChainThese(
                        new EffectRollDown(
                            selectedAccordionView.Content, AnimationDuration))
                    .Render();
            }
            else if (AnimationMode == AnimationType.MultipleOpen)
            {
                // More than one AccordionView can be open at the same time
                // Therefor checking to see if we're about to hide the clicked one
                // or show it. If an AccordionView is clicked while opened it will
                // be hidden.
                if (selectedAccordionView.Content.Style[Styles.display] == "none")
                {
                    new EffectRollDown(
                        selectedAccordionView.Content, 
                        AnimationDuration)
                        .Render();
                }
                else
                {
                    new EffectRollUp(
                        selectedAccordionView.Content, 
                        AnimationDuration)
                        .Render();
                }
            }
            else // This is a "no amimations" Accordion...
            {
                // Just settings the styles directly with no animations...
                previouslySelectedAccView.Content.Style[Styles.display] = "none";
                selectedAccordionView.Content.Style[Styles.display] = "block";
            }
        }

        // Returns true if there is no ActiveAccordionViewChanged Event Handler
        // defined for this Accordion.
        internal bool NoChangedEventHandlerDefined
        {
            get { return ActiveAccordionViewChanged == null; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (AnimationMode == AnimationType.MultipleOpen && ActiveAccordionViewChanged != null)
                throw new ArgumentException("Cannot both allow Multiple Open Accordion views and handle ActiveAccordionViewChanged event");
            base.OnPreRender(e);
        }
    }
}
