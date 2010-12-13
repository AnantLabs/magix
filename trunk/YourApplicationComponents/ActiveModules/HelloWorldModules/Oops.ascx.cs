/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magic.UX.Widgets;
using Magic.Brix.Types;
using Magic.Brix.Loader;

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














