/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;

namespace Magix.UX.Effects
{
    public class EffectHighlight : Effect
    {
        public EffectHighlight()
            : base(null, 0)
        { }

        public EffectHighlight(Control control, int milliseconds)
			: base(control, milliseconds)
		{ }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.Highlight"; }
        }

        protected override string GetOptions()
        {
            return "";
        }
    }
}
