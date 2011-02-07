/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Settings
{
    [ActiveController]
    public class SettingsController
    {
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        private static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Data.ChangeSimplePropertyValue",
                "DBAdmin.Data.ChangeSimplePropertyValue-Override");
            ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Data.ChangeSimplePropertyValue-Passover",
                "DBAdmin.Data.ChangeSimplePropertyValue");
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue-Override")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue_Override(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "DBAdmin.Data.ChangeSimplePropertyValue-Passover",
                e.Params);

            // This one is here to make sure our Settings are "touched" and "reloaded"
            // whe any Setting value is changed from DBAdmin
            if (e.Params["FullTypeName"].Get<string>() ==
                typeof(Magix.Brix.Components.ActiveTypes.Settings.Setting)
                    .FullName)
            {
                Magix.Brix.Components.ActiveTypes.Settings.Instance.Reload();
            }
        }
    }
}
