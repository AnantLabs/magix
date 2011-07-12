/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class SliderMenuController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetSliderMenuItems")]
        protected void Magix_Publishing_GetSliderMenuItems(object sender, ActiveEventArgs e)
        {
            foreach (PageObject idx in PageObject.Select())
            {
                e.Params["Items"]["i" + idx.ID]["Caption"].Value = idx.Name;
                e.Params["Items"]["i" + idx.ID]["Event"]["Name"].Value = "Magix.Publishing.SliderMenuItemClicked";
                e.Params["Items"]["i" + idx.ID]["Event"]["MenuItemID"].Value = idx.ID;
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.SliderMenuItemClicked")]
        protected void Magix_Publishing_SliderMenuItemClicked(object sender, ActiveEventArgs e)
        {
            PageObject o = PageObject.SelectByID(e.Params["MenuItemID"].Get<int>());
        }
    }
}























