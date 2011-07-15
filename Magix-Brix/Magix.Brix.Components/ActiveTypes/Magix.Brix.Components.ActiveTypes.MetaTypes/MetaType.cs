/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Reflection;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.MetaTypes
{
    [ActiveType]
    public class MetaType : ActiveType<MetaType>
    {
        [ActiveType]
        public class Value : ActiveType<Value>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Val { get; set; }
        }

        public MetaType()
        {
            Values = new LazyList<Value>();
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string Reference { get; set; }

        [ActiveField]
        public DateTime Created { get; set; }

        [ActiveField]
        public LazyList<Value> Values { get; set; }
    }
}
