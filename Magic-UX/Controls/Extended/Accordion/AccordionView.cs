/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using Magic.UX.Builder;
using Magic.UX.Widgets;
using Magic.UX.Effects;
using Magic.UX.Widgets.Core;

namespace Magic.UX.Widgets
{
    /**
     * Panels contained inside the Accordion control. Basically an Accordion is only a collection
     * of these types of controls.
     */
    public class AccordionView : CompositeControl
    {
        private readonly LinkButton _caption = new LinkButton();

        public AccordionView()
        {
            CssClass = "mux-acc-view";
            Tag = "li";
        }

        /**
         * The Header text of the AccordionView. The text displayed at its top.
         */
        public string Caption
        {
            get { return _caption.Text; }
            set { _caption.Text = value; }
        }

        protected override void CreateCompositeControl()
        {
            Content.CssClass = "mux-acc-view-content";
            SetVisibility();

            _caption.ID = "capt";
            _caption.CssClass = "mux-acc-view-caption";
            switch ((Parent as Accordion).ExpansionMode)
            {
                case Accordion.ExpansionTrigger.Click:
                    _caption.Click += ChangeActiveView;
                    break;
                case Accordion.ExpansionTrigger.DblClick:
                    _caption.DblClick += ChangeActiveView;
                    break;
                case Accordion.ExpansionTrigger.MouseOver:
                    _caption.MouseOver += ChangeActiveView;
                    break;
                case Accordion.ExpansionTrigger.Hover:
                    MouseOver += AddActiveView;
                    MouseOut += RemoveActiveView;
                    break;
            }
            Controls.Add(_caption);
            base.CreateCompositeControl();
        }

        private void ChangeActiveView(object sender, EventArgs e)
        {
            (Parent as Accordion).SetActiveView(this, true);
        }

        private void AddActiveView(object sender, EventArgs e)
        {
            (Parent as Accordion).AddActiveView(this);
        }

        private void RemoveActiveView(object sender, EventArgs e)
        {
            (Parent as Accordion).RemoveActiveView(this);
        }

        private void SetVisibility()
        {
            int idxCurrentVisibleView = (Parent as Accordion).ActiveAccordionViewIndex;
            int idxOfThisView = (Parent as Accordion)[this];
            if((Parent as Accordion).ExpansionMode != Accordion.ExpansionTrigger.Hover && 
                idxCurrentVisibleView == idxOfThisView)
            {
                Content.Style[Styles.display] = "";
            }
            else
            {
                Content.Style[Styles.display] = "none";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if((Parent as Accordion).NoChangedEventHandlerDefined)
            {
                // This Accordion has not defined an Event Handler for the
                // ActiveAccordionViewChanged, and hence we must couple
                // the changing of Active Accordion with client-side effects.

                // Before we can add the client-side effects, we must remove the
                // client-side event handlers so that we don't raise XHR requests
                // when clicked.
                // Removing all server-side event handlers...
                _caption.Click -= ChangeActiveView;
                _caption.DblClick -= ChangeActiveView;
                _caption.MouseOver -= ChangeActiveView;
                _caption.MouseOut -= ChangeActiveView;
                MouseOver -= AddActiveView;
                MouseOut -= RemoveActiveView;

                if ((Parent as Accordion).ExpansionMode == Accordion.ExpansionTrigger.Hover)
                {
                    MouseOverEffect = 
                        new EffectRollDown(
                            Content, 
                            (Parent as Accordion).AnimationDuration);
                    MouseOutEffect = 
                        new EffectRollUp(
                            Content, 
                            (Parent as Accordion).AnimationDuration);
                }
                else
                {
                    // Getting a list of all the AccordionViews to send into the effect
                    // initialization.
                    List<Control> ctrls = new List<Control>();
                    foreach (AccordionView idx in (Parent as Accordion).Views)
                    {
                        ctrls.Add(idx.Content);
                    }

                    Effect effect = null;
                    if ((Parent as Accordion).AnimationMode == Accordion.AnimationType.MultipleOpen)
                    {
                        effect = 
                            new EffectToggle(
                                Content, 
                                (Parent as Accordion).AnimationDuration);
                    }
                    else
                    {
                        string animationType = (Parent as Accordion).AnimationMode.ToString();
                        effect = 
                            new EffectToggleElements(
                                Content, 
                                (Parent as Accordion).AnimationDuration, 
                                animationType, 
                                ctrls);
                    }

                    // Associating the client-side effect with the correct DOM event
                    switch ((Parent as Accordion).ExpansionMode)
                    {
                        case Accordion.ExpansionTrigger.Click:
                            _caption.ClickEffect = effect;
                            break;
                        case Accordion.ExpansionTrigger.DblClick:
                            _caption.DblClickEffect = effect;
                            break;
                        case Accordion.ExpansionTrigger.MouseOver:
                            _caption.MouseOverEffect = effect;
                            break;
                    }
                }
            }
            base.OnPreRender(e);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                _caption.RenderControl(builder.Writer);
                Content.RenderControl(builder.Writer);
            }
        }
    }
}
