/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;

namespace Magix.UX.Effects
{
    public class EffectTimeout : Effect
    {
        public EffectTimeout(int milliseconds)
            : base(null, milliseconds)
        { }

        protected override void ValidateEffect()
        { }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.Timeout"; }
        }

        protected override string GetOptions()
        {
            return "";
        }
    }
}
