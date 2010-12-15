/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Web;
using Magix.Brix.Types;

namespace Magix.Brix.Loader
{
    public class ActiveController
    {
        protected void LoadModule(string name, string container)
        {
            LoadModule(name, container, null);
        }

        protected void LoadModule(string name, string container, Node node)
        {
            if (string.IsNullOrEmpty(container))
                throw new ApplicationException("Cannot load a Module without specifying a container");
            ActiveEvents.Instance.RaiseLoadControl(name, container, node);
        }

        protected Page MainPage
        {
            get
            {
                return (Page)HttpContext.Current.Handler;
            }
        }
    }
}
