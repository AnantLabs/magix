/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveModules.MetaView
{
    /**
     * Level1: UI parts for showing a MetaView in 'MultiView Mode'. Basically shows a grid, with items
     * dependent upon the look of the view. This is a Publisher Plugin module. Basically a 
     * completely 'empty' module whos only purpose is to raise the
     * 'Magix.MetaType.ShowMetaViewMultipleInCurrentContainer' event, whos default implementation
     * will simply in its entirety replace this Module, hence not really much to see here.
     * 
     * This is the PublisherPlugin you'd use if you'd like to see a 'list of MetaObjects' on 
     * the page
     */
    [ActiveModule]
    [PublisherPlugin]
    public class MetaView_Multiple : ActiveModule
    {
        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            // Crap ...
            // I don't know why ...!
            if (!IsPostBack)
                Page.LoadComplete +=
                    delegate
                    {
                        LoadModule(node);
                    };
            else
                Load +=
                    delegate
                    {
                        LoadModule(node);
                    };
        }

        private void LoadModule(Node node)
        {
            node["MetaViewName"].Value = ViewName;
            node["Container"].Value = this.Parent.ID;

            RaiseSafeEvent(
                "Magix.MetaType.ShowMetaViewMultipleInCurrentContainer",
                node);
        }

        /**
         * Level1: The name of the MetaView to use as the foundation for this form
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.MetaView.MetaView_Multiple.GetTemplateColumnSelectView")]
        public string ViewName
        {
            get { return ViewState["ViewName"] as string; }
            set { ViewState["ViewName"] = value; }
        }
    }
}



