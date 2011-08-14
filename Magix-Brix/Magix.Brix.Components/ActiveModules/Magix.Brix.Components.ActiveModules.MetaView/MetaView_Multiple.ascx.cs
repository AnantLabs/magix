/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * MagicBRIX is licensed as GPLv3.
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
    [ActiveModule]
    [PublisherPlugin]
    public class MetaView_Multiple : ActiveModule, IModule
    {
        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
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

        [ModuleSetting(ModuleEditorEventName = "Magix.MetaView.MetaView_Multiple.GetTemplateColumnSelectView")]
        public string ViewName
        {
            get { return ViewState["ViewName"] as string; }
            set { ViewState["ViewName"] = value; }
        }
    }
}



