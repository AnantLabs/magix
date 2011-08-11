/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Effects
{
    public class EffectFocusAndSelect : Effect
    {
        private bool isRaControl;
        private bool isOnlyFocus;

        public EffectFocusAndSelect(Control control)
            : base(control, 1)
        {
            isRaControl = control is BaseWebControl;
        }

        public EffectFocusAndSelect(Control control, bool onlyFocus)
            : base(control, 1)
        {
            isRaControl = control is BaseWebControl;
            isOnlyFocus = onlyFocus;
        }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.FocusSelect"; }
        }

        protected override string GetOptions()
        {
            if (isOnlyFocus)
                return "isFocus:true,";
            return "";
        }
    }
}
