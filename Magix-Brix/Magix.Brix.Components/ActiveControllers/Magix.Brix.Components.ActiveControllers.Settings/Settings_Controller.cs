/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Settings
{
    /**
     * Level2: Helper logic to assist with Settings and updating of settings. 
     * PS!
     * Unless you have this 
     * controller in your Application Pool, Enabled, then direct changes, through the DB Admin
     * module to Settings will NOT affect the Application Pool before it's being re 
     * started ...
     */
    [ActiveController]
    public class Settings_Controller
    {
        /**
         * Level2: Need to override 'DBAdmin.Data.ChangeSimplePropertyValue' to make sure
         * settings are 'flushed' if they're being updated
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        private static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            // To interact with DBAdmin, and make updates in DBAdmin 'flush' the cache, at all
            // times kept of the Settings ...
            // BAD solution, untie ...!
            ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Data.ChangeSimplePropertyValue",
                "DBAdmin.Data.ChangeSimplePropertyValue-Override");
            ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Data.ChangeSimplePropertyValue-Passover",
                "DBAdmin.Data.ChangeSimplePropertyValue");
        }

        /**
         * Level2: Overrides 'DBAdmin.Data.ChangeSimplePropertyValue' to make sure
         * settings are 'flushed' if they're being updated. Calls original logic,
         * then resets the settings and reloads them. This because settings are 
         * cached on Application Level for speed issues
         */
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
