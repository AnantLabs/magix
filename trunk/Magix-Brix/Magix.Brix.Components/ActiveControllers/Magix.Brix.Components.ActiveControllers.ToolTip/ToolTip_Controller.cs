/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Loader;
using T = Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.ToolTip
{
    /**
     * Level2: Contains helper logic for displaying and maintaining previous/next Tip of today and such
     */
    [ActiveController]
    public class ToolTip_Controller : ActiveController
    {
        /**
         * Level2: Returns the Previous Tooltip on a 'Per User' level. Meaning, it'll keep track of which
         * ToolTip has been shown to which User
         */
        [ActiveEvent(Name = "Magix.Core.GetPreviousToolTip")]
        private void Magix_Core_GetPreviousToolTip(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value = 
                T.TipOfToday.Instance.Previous(UserBase.Current.Username);
        }

        /**
         * Level2: Returns the Next Tooltip on a 'Per User' level. Meaning, it'll keep track of which
         * ToolTip has been shown to which User
         */
        [ActiveEvent(Name = "Magix.Core.GetNextToolTip")]
        private void Magix_Core_GetNextToolTip(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value =
                T.TipOfToday.Instance.Next(UserBase.Current.Username);
        }

        /**
         * Level2: Returns all ToolTip instances to caller
         */
        [ActiveEvent(Name = "Magix.Core.GetAllToolTips")]
        private void Magix_Core_GetAllToolTips(object sender, ActiveEventArgs e)
        {
            foreach (T.TipOfToday.Tip idx in T.TipOfToday.Tip.Select())
            {
                e.Params["t-" + idx.ID].Value = idx.Value;
            }
        }
    }
}
