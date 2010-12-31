/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Effects;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using System.Web.UI.HtmlControls;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    public abstract class DataBindableSingleInstanceControl : NestedDynamic, IModule
    {
        protected virtual void UpdateCaption()
        {
            (Parent.Parent.Parent as Window).Caption = string.Format(
@"Showing {0}({1}), belongs to {3}({4}) on {5}, total {2}",
                TypeName,
                DataSource["Object"]["ID"].Get<int>(),
                TotalCount,
                ParentType,
                ParentID,
                ParentPropertyName);
        }

        protected int ActiveID
        {
            get { return DataSource["Object"]["ID"].Get<int>(); }
        }
    }
}
