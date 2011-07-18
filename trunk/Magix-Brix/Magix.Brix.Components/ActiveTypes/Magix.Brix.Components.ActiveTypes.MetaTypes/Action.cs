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
        [ActiveType]
        public class ActionParams : ActiveType<ActionParams>
        {
            public ActionParams()
            {
                Children = new LazyList<ActionParams>();
            }

            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Value { get; set; }

            [ActiveField]
            public string TypeName { get; set; }

            [ActiveField]
            public LazyList<ActionParams> Children { get; set; }

            public override void Save()
            {
                if (string.IsNullOrEmpty(TypeName))
                    TypeName = "System.String";
                base.Save();
            }
        }

        public Action()
        {
            Params = new LazyList<ActionParams>();
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string Description { get; set; }

        [ActiveField]
        public string EventName { get; set; }

        [ActiveField]
        public LazyList<ActionParams> Params { get; set; }

        [ActiveField]
        public bool StripInput { get; set; }

        public override void Save()
        {
            foreach (Action idx in Action.Select(
                Criteria.Eq("Name", Name)))
            {
                if (idx.ID != ID)
                    throw new ArgumentException("Ooops, the Name for that Action seems to be already taken by another View in the system, either change the name of the previously named Action, or choose another name for this Action ...");
            }
            base.Save();
        }
    }
}
