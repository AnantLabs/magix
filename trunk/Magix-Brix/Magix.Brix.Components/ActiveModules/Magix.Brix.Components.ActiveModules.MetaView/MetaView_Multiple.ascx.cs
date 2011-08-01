/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
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
            Load +=
                delegate
                {
                    DataSource["MetaViewName"].Value = ViewName;
                    DataSource["Container"].Value = this.Parent.ID;

                    RaiseSafeEvent(
                        "Magix.MetaType.RaiseViewMetaTypeFromMultipleView",
                        DataSource);
                };
        }

        [ModuleSetting(ModuleEditorEventName = "Magix.MetaView.GetTemplateColumnSelectViewMultiple")]
        public string ViewName
        {
            get { return ViewState["ViewName"] as string; }
            set { ViewState["ViewName"] = value; }
        }
    }
}



