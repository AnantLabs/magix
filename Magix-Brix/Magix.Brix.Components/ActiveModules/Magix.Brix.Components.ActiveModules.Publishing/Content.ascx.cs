/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    [PublisherPlugin]
    public class Content : ActiveModule
    {
        protected Label lbl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    lbl.Text = Text;
                };
        }

        [ModuleSetting(ModuleEditorName = "Magix.Brix.Components.ActiveModules.Editor.RichEdit")]
        public string Text
        {
            get { return ViewState["Text"] as string; }
            set { ViewState["Text"] = value; }
        }
    }
}



