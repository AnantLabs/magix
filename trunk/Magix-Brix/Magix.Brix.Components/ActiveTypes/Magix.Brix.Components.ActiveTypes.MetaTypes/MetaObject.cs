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
    public class MetaObject : ActiveType<MetaObject>
    {
        [ActiveType]
        public class Value : ActiveType<Value>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Val { get; set; }

            internal Value Clone()
            {
                Value ret = new Value();
                ret.Name = Name;
                ret.Val = Val;
                return ret;
            }
        }

        public MetaObject()
        {
            Values = new LazyList<Value>();
        }

        [ActiveField]
        public string TypeName { get; set; }

        [ActiveField]
        public string Reference { get; set; }

        [ActiveField]
        public DateTime Created { get; set; }

        [ActiveField]
        public LazyList<Value> Values { get; set; }

        public override void Save()
        {
            if (ID == 0)
                Created = DateTime.Now;
            base.Save();
        }

        public MetaObject Clone()
        {
            return DeepClone(this);
        }

        private MetaObject DeepClone(MetaObject metaObject)
        {
            MetaObject ret = new MetaObject();
            ret.TypeName = TypeName;
            ret.Reference = "cloned: " + ID;
            foreach (Value idx in Values)
            {
                Value v = idx.Clone();
                ret.Values.Add(v);
            }
            return ret;
        }
    }
}
