/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
     * Abstract helper widget for widget developers. Used internally in e.g.
     * TreeItem. Useful for those cases where you have a widget which is 'composed' out
     * of both a norml 'surface', where the user can add up inner child widgets and such, 
     * and also 'chrome widgets', or widgets that are a part of the actual widget itself, and
     * you in addition to this also have a specific type of widgets that are supposed to
     * be its child control collections, like for instance the TreeItem are only supposed
     * to have TreeItems within itself.
     * The TreeItem of the TreeView is one such example since it has a 'surface' where the 
     * user might add up inner controls, text or such. But it also have a Text property
     * and many other child controls. In addition
     * it is supposed to only have child controls of type TreeItem.
     * That's why the TreeItem is a 'CompositeViewCollectionControl'.
     */
    [ParseChildren(true, "Content")]
    public class CompositeViewCollectionControl<T> : ViewCollectionControl<T> where T: Control
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
