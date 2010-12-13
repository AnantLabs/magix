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

namespace HelloWorldModules
{
    [ActiveModule]
    public class Oops : UserControl, IModule
    {
        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                };
        }
    }
}














