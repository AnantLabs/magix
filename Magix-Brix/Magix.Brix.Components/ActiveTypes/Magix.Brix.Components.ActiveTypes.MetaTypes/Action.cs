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
    public class Action : ActiveType<Action>
    {
        [ActiveEvent]
        public string Name { get; set; }

        [ActiveEvent]
        public string Description { get; set; }

        [ActiveEvent]
        public string EventName { get; set; }
    }
}
