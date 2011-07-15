/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.MetaViews
{
    [ActiveType]
    public class MetaView : ActiveType<MetaView>
    {
        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string TypeName { get; set; }

        [ActiveField]
        public bool IsList { get; set; }
    }
}
