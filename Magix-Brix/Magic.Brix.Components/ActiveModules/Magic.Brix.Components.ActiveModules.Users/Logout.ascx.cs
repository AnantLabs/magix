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

namespace Magic.Brix.Components.ActiveModules.Users
{
    [ActiveModule]
    public class Logout : UserControl, IModule
    {
        void IModule.InitialLoading(Node node)
        {
        }

        protected void logout_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "LogoutUser");
        }
    }
}



