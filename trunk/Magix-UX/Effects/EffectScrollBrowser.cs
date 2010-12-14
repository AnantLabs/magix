/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using Magix.UX.Widgets;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Effects
{
    public class EffectScrollBrowser : Effect
    {
        public EffectScrollBrowser()
            : base(null, 0)
        { }

        public EffectScrollBrowser(int milliseconds)
            : base(null, milliseconds)
        { }

        protected override void ValidateEffect()
        {
            return;
        }

        protected override string GetOptions()
        {
            return "";
        }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.ScrollBrowser"; }
        }
    }
}
