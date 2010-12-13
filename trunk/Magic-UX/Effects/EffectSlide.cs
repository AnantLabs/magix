/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System.Web.UI;
using System.Collections.Generic;

namespace Magic.UX.Effects
{
    public class EffectSlide : Effect
    {
        private readonly int _offset;

        public EffectSlide()
            : base(null, 0)
        { }

        public EffectSlide(Control control, int milliseconds)
            : base(control, milliseconds)
        { }

        public EffectSlide(Control control, int milliseconds,int offset)
			: base(control, milliseconds)
		{
            _offset = offset;
        }

        public EffectSlide(int offset)
			: base(null, 0)
		{
            _offset = offset;
		}
		
        protected override string NameOfEffect
        {
            get { return "MUX.Effect.Slide"; }
        }

        protected override string GetOptions()
        {
            return "offset:" + _offset + ",";
        }
    }
}
