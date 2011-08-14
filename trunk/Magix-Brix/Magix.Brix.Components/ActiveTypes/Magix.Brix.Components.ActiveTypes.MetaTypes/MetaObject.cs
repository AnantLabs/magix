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
using System.Collections.Generic;

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

            [ActiveField(BelongsTo = true)]
            public MetaObject ParentMetaObject { get; set; }

            internal Value Clone()
            {
                Value ret = new Value();
                ret.Name = Name;
                ret.Val = Val;
                return ret;
            }

            public override void Save()
            {
                // Making sure every single property is uniquely named ...
                bool found = true;
                while (found)
                {
                    found = false;
                    Value other = ParentMetaObject.Values.Find(
                        delegate(Value idx)
                        {
                            return idx.Name == Name &&
                                idx != this;
                        });
                    if(other != null)
                    {
                        // Last added must 'yield' ...
                        other.Name += "_";
                        found = true;
                    }
                }
                base.Save();
            }
        }

        public MetaObject()
        {
            Values = new LazyList<Value>();
            Children = new LazyList<MetaObject>();
        }

        [ActiveField]
        public string TypeName { get; set; }

        [ActiveField]
        public string Reference { get; set; }

        [ActiveField]
        public DateTime Created { get; set; }

        [ActiveField]
        public LazyList<Value> Values { get; set; }

        [ActiveField(IsOwner = false)]
        public LazyList<MetaObject> Children { get; set; }

        [ActiveField(BelongsTo = true, RelationName = "Children")]
        public MetaObject ParentMetaObject { get; set; }

        public override void Save()
        {
            MetaObject idxParent = ParentMetaObject;

            while (idxParent != null)
            {
                if (idxParent == this)
                    throw new ArgumentException("You can't have cyclic relationships with your objects ... Sorry ... :(");
                idxParent = idxParent.ParentMetaObject;
            }

            if (ID == 0)
                Created = DateTime.Now;

            foreach (Value idx in Values)
            {
                idx.ParentMetaObject = this;
            }

            if (Children.ListRetrieved)
            {
                // Making sure no children are not twice. And also
                // that this is not added into Children ...
                Dictionary<int, bool> others = new Dictionary<int, bool>();
                others[ID] = true;
                foreach (MetaObject idx in Children)
                {
                    if (others.ContainsKey(idx.ID) && 
                        idx.ID != 0)
                    {
                        throw new ArgumentException("You can't have a child be added twice. Nor can you add an object to itself as its own child ...");
                    }
                    others[idx.ID] = true;
                }
            }

            // Making sure the reference at the very least says; Anonymous Coward ...
            if (string.IsNullOrEmpty(Reference))
                Reference = "[Anonymous-Coward-Reference]";
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

            foreach (MetaObject idx in Children)
            {
                MetaObject n = idx.Clone();
                ret.Children.Add(n);
            }

            ret.Save();
            return ret;
        }
    }
}
