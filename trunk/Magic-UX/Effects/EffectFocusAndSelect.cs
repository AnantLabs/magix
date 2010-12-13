/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magic.UX.Widgets;
using Magic.UX.Widgets.Core;

namespace Magic.UX.Effects
{
    public class EffectFocusAndSelect : Effect
    {
        private bool isRaControl;

        public EffectFocusAndSelect(Control control)
			: base(control, 1)
		{
            isRaControl = control is BaseWebControl;
        }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.FocusSelect"; }
        }

        protected override string GetOptions()
        {
            return "";
        }
    }
}
