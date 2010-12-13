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
    public class EffectGeneric : Effect
    {
        public EffectGeneric()
            : base(null, 0)
        { }

        public EffectGeneric(string beat)
            : base(null, 0)
        {
            End = beat;
        }

        protected override string NameOfEffect
        {
            get { return "MUX.Effect.Generic"; }
        }

        public string Start { get; set; }

        public string Loop { get; set; }

        public string End { get; set; }

        protected override string GetOptions()
        {
            string retVal = "";
            if (!string.IsNullOrEmpty(Start))
                retVal += "start: " + Start + ",";
            if (!string.IsNullOrEmpty(Loop))
                retVal += "loop: " + Loop + ",";
            if (!string.IsNullOrEmpty(End))
                retVal += "end: " + End + ",";
            return retVal;
        }

        protected override void ValidateEffect()
        {
        }
    }
}
