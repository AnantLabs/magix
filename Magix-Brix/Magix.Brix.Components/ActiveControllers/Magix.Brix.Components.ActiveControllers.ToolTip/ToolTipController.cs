/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.ToolTip
{
    [ActiveController]
    public class ToolTipController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.GetPreviousToolTip")]
        private void Magix_Core_GetPreviousToolTip(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value = 
                Magix.Brix.Components.ActiveTypes.ToolTip.Instance.Previous(UserBase.Current.Username);
        }

        [ActiveEvent(Name = "Magix.Core.GetNextToolTip")]
        private void Magix_Core_GetNextToolTip(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value =
                Magix.Brix.Components.ActiveTypes.ToolTip.Instance.Next(UserBase.Current.Username);
        }
    }
}
