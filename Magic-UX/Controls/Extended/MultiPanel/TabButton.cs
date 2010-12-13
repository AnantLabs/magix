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
using Magix.UX.Effects;

namespace Magix.UX.Widgets
{
    /**
     * One instance of an item within a TabStrip. Mostly shown as buttons inside of
     * the TabStrip.
     */
    public class TabButton : Panel
    {
        private LinkButton _button = new LinkButton();

        public TabButton()
        {
            CssClass = "mux-multi-button-view";
            Tag = "li";
        }

        /**
         * The caption of your button. Or the visible text.
         */
        public string Text
        {
            get { return _button.Text; }
            set { _button.Text = value; }
        }

        /**
         * MultiPanelView ID associated with this instance. Meaning which MultiPanelView
         * will be visible when button is clicked. If this value is not given, the index of the
         * MultiButtonView will be used as the basis of figuring out which view to expand. Meaning
         * that if this button is the 3rd button in the collection, the 3rd view in the view
         * collection will be expanded.
         */
        public string MultiPanelViewID
        {
            get { return ViewState["MultiPanelViewID"] == null ? "" : (string)ViewState["MultiPanelViewID"]; }
            set { ViewState["MultiPanelViewID"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            _button.ID = "btn";
            _button.CssClass = "mux-multi-button-view-button";
            switch ((Parent as TabStrip).ExpansionMode)
            {
                case TabStrip.ExpansionTrigger.Click:
                    _button.Click += ButtonTriggered;
                    break;
                case TabStrip.ExpansionTrigger.DblClick:
                    _button.DblClick += ButtonTriggered;
                    break;
                case TabStrip.ExpansionTrigger.MouseOver:
                    _button.MouseOver += ButtonTriggered;
                    break;
            }
            Controls.Add(_button);
            base.CreateChildControls();
        }

        private void ButtonTriggered(object sender, EventArgs e)
        {
            (Parent as TabStrip).SetActiveView(this);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            if ((Parent as TabStrip).NoChangedEventHandlerDefined)
            {
                // Removing all server-side event handlers...
                _button.Click -= ButtonTriggered;
                _button.DblClick -= ButtonTriggered;
                _button.MouseOver -= ButtonTriggered;

                string idOfMultiPanel = (Parent as TabStrip).MultiPanelID;

                if (string.IsNullOrEmpty(idOfMultiPanel))
                    throw new ArgumentException(
                        "You have created a completely useless MultiButton, read the documentation about how to use these things...!");

                MultiPanel panel = Selector.FindControl<MultiPanel>(Page, idOfMultiPanel);
                MultiPanelView view = null;
                if (string.IsNullOrEmpty(MultiPanelViewID))
                {
                    int indexOfThis = (Parent as TabStrip)[this];
                    view = panel[indexOfThis];
                }
                else
                {
                    view = Selector.FindControl<MultiPanelView>(panel, MultiPanelViewID);
                }
                Effect effect = null;
                if (panel.AnimationMode == MultiPanel.AnimationType.Slide)
                {
                    int indexOfThis;
                    if (string.IsNullOrEmpty(MultiPanelViewID))
                    {
                        indexOfThis = (Parent as TabStrip)[this];
                    }
                    else
                    {
                        view = Selector.FindControl<MultiPanelView>(panel, MultiPanelViewID);
                        indexOfThis = panel[view];
                    }
                    effect =
                        new EffectSlide(
                            panel.Content,
                            panel.AnimationDuration,
                            -indexOfThis);
                }
                else
                {
                    List<Control> ctrls = new List<Control>();
                    foreach (MultiPanelView idx in panel.Views)
                    {
                        ctrls.Add(idx);
                    }
                    effect =
                        new EffectToggleElements(
                            view,
                            panel.AnimationDuration,
                            panel.AnimationMode.ToString(),
                            ctrls);
                }
                switch ((Parent as TabStrip).ExpansionMode)
                {
                    case TabStrip.ExpansionTrigger.Click:
                        _button.ClickEffect = effect;
                        break;
                    case TabStrip.ExpansionTrigger.DblClick:
                        _button.DblClickEffect = effect;
                        break;
                    case TabStrip.ExpansionTrigger.MouseOver:
                        _button.MouseOverEffect = effect;
                        break;
                }
            }
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
