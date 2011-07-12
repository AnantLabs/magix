/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class DashboardController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.LoadDashboard")]
        protected void Magix_Publishing_LoadDashboard(object sender, ActiveEventArgs e)
        {
            if (User.Current.InRole("Administrator"))
            {
                RaiseEvent("Magix.Publishing.LoadAdministratorDashboard", e.Params);
            }
            else
            {
                RaiseEvent("Magix.Publishing.LoadUserDashboard", e.Params);
            }
        }
    }
}
