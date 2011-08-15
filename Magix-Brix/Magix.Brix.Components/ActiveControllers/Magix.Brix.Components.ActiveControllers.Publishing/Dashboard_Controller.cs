/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Main 'router' in dispatching important Dashboard functionality 
     */
    [ActiveController]
    public class Dashboard_Controller : ActiveController
    {
        /**
         * Level2: Will load the Dashboard if User is of type Administrator through raising
         * 'Magix.Publishing.LoadDashboard', otherwise raise
         * 'Magix.Publishing.LoadUserDashboard' which isn't currently being catched 
         * any place
         */
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
