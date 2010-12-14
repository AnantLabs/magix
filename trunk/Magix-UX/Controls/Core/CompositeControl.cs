/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.UX.Builder;

namespace Magix.UX.Widgets.Core
{
    /**
     * Abstract helper widget for widget developers. Used internally in e.g. Window and
     * Accordion. Useful for those cases where you have a widget which is 'composed' out
     * of both a norml 'surface', where the user can add up inner child widgets and such, 
     * and also 'chrome widgets', or widgets that are a part of the actual widget itself.
     * The Window is one such example since it has a 'surface' where the user might
     * add up inner controls, text or such. But it also have a Caption and a Close button.
     * That's why the Window is a 'CompositeControl'.
     */
    [ParseChildren(true, "Content")]
    public abstract class CompositeControl : Panel
    {
        private readonly Panel _content = new Panel();

        /**
         * The Content area of your widget. This is where you would want to add 
         * up child controls to make them a part of the child controls collection
         * of your widget.
         */
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public Panel Content
        {
            get { return _content; }
        }

        /**
         * This is your actual 'control area'. This is, among other things, where
         * you want to add up Aspects and such if you want to add aspects to widgets
         * which are CompositeControls. In code you can use 'myControl', but in
         * markup you have to use &lt;Control&gt; to gain access to the control itself.
         * This is because of that the Content property is the default for your
         * control.
         */
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Panel Control
        {
            get { return this; }
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

        protected virtual void CreateCompositeControl()
        {
            _content.ID = "cnt";
            Controls.Add(_content);
        }
    }
}
