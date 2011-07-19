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

            public ActionParams Clone()
            {
                return DeepCopy(this);
            }

            private ActionParams DeepCopy(ActionParams actionParams)
            {
                ActionParams ret = new ActionParams();

                ret.Name = Name;
                ret.TypeName = TypeName;
                ret.Value = Value;

                foreach (ActionParams idx in Children)
                {
                    ret.Children.Add(idx.Clone());
                }

                return ret;
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
            string name = Name;

            int idxNo = 2;
            while (CountWhere(
                Criteria.Eq("Name", name),
                Criteria.NotId(ID)) > 0)
            {
                name = Name + " - " + idxNo.ToString();
                idxNo += 1;
            }
            Name = name;
            base.Save();
        }

        public Action Copy()
        {
            return DeepCopy(this);
        }

        private Action DeepCopy(Action action)
        {
            Action ret = new Action();
            ret.Description = Description;
            ret.EventName = EventName;
            ret.Name = "Copy - " + Name;
            foreach (ActionParams idx in Params)
            {
                ret.Params.Add(idx.Clone());
            }
            ret.StripInput = ret.StripInput;
            return ret;
        }
    }
}
