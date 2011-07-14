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

namespace Magix.Brix.Components.ActiveModules.MetaTypes
{
    [ActiveModule]
    public class Dashboard : ActiveModule
    {
        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                };
        }

        protected void create_Click(object sender, EventArgs e)
        {
            RaiseSafeEvent("Magix.MetaType.CreateType");
        }
    }
}

















