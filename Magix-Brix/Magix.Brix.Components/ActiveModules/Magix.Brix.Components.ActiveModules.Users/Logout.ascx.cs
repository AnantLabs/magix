/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.Users
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



