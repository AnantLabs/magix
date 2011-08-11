/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
            StripInput = true;
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

        [ActiveField]
        public DateTime Created { get; set; }

        public override void Save()
        {
            if (ID == 0)
                Created = DateTime.Now;
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
            ret.StripInput = StripInput;
            foreach (ActionParams idx in Params)
            {
                ret.Params.Add(idx.Clone());
            }
            return ret;
        }
    }
}
