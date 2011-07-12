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
    public class Header : ActiveModule
    {
        protected Label lbl;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                };
        }

        [ModuleSetting]
        public string Caption
        {
            get { return ViewState["Caption"] as string; }
            set { ViewState["Caption"] = value; }
        }
    }
}



