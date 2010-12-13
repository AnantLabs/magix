/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * A strip of TabButtons, used often to trigger changing active view in combination
     * with the MultiPanel control. This control is a collection of TabButtons.
     */
    public class TabStrip : ViewCollectionControl<TabButton>
    {
        /**
         * What to trigger the expansion
         */
        public enum ExpansionTrigger
        {
            /**
             * Click will trigger the expansion of the View
             */
            Click,

            /**
             * MouseOver will trigger the expansion of the View
             */
            MouseOver,

            /**
             * Double-click will trigger the expansion of the View
             */
            DblClick
        };

        public TabStrip()
        {
            CssClass = "mux-multi-button";
            Tag = "ul";
        }

        /**
         * Server-side Event Handler raised whan any of the buttons are clicked. If you
         * do not handle this event, then the entire logic will run completely on the 
         * client-side, which means it'll run significantly faster due to not having to
         * go to the server at all to do its logic.
         */
        public event EventHandler MultiButtonClicked;

        /**
         * What to trigger the opening of the View. Possible values are Click, 
         * DblClick and MouseOver.
         */
        public ExpansionTrigger ExpansionMode
        {
            get { return ViewState["ExpansionMode"] == null ? ExpansionTrigger.Click : (ExpansionTrigger)ViewState["ExpansionMode"]; }
            set { ViewState["ExpansionMode"] = value; }
        }

        /**
         * Currently active or visible view's index
         */
        public int ActiveMultiButtonViewIndex
        {
            get { return ViewState["ActiveMultiButtonViewIndex"] == null ? 0 : (int)ViewState["ActiveMultiButtonViewIndex"]; }
            set { ViewState["ActiveMultiButtonViewIndex"] = value; }
        }

        /**
         * ID of MultiPanel associated with this instance. This must be set to something since
         * otherwise you have effectively created a completely useless TabStrip, with no purpose.
         */
        public string MultiPanelID
        {
            get { return ViewState["MultiPanelID"] == null ? "" : (string)ViewState["MultiPanelID"]; }
            set { ViewState["MultiPanelID"] = value; }
        }

        internal void SetActiveView(TabButton selectedView)
        {
            // Getting the index of the previously selected AccordionView
            TabButton previouslySelectedView = this[ActiveMultiButtonViewIndex];

            // Checking to see if the newly selected one is the same as the
            // previously selected one, and if so just returning since we then shouldn't
            // do any of the below logic.
            if (previouslySelectedView.ID == selectedView.ID)
                return;

            // Updating the currently selected AccordionView
            ActiveMultiButtonViewIndex = this[selectedView];

            // Raising our event, but only if an event handler is defined.)
            if (MultiButtonClicked != null)
            {
                MultiButtonClicked(this, new EventArgs());
            }

            if (!string.IsNullOrEmpty(MultiPanelID))
            {
                MultiPanel panel = Selector.FindControl<MultiPanel>(Page, MultiPanelID);
                if(string.IsNullOrEmpty(selectedView.MultiPanelViewID))
                    panel.SetActiveView(ActiveMultiButtonViewIndex);
                else
                {
                    MultiPanelView view = 
                        Selector.FindControl<MultiPanelView>(panel, selectedView.MultiPanelViewID);
                    panel.SetActiveView(view);
                }
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

        internal bool NoChangedEventHandlerDefined
        {
            get { return MultiButtonClicked == null; }
        }
    }
}
