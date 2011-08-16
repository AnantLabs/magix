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
    /**
     * The storage for the 'Meta Application System' in Magix. Every time you create a new object, using
     * a Meta View or something similar, a MetaObject is being created to hold this data. A Meta Object
     * is basically an object 'without structure', where you can, by editing your views, maintain the
     * structure you wish for your data. In such a way you can create your own Meta Applications,
     * using Magix, without being imposed onto an existing data-structure in any ways
     */
    [ActiveType]
    public class MetaObject : ActiveType<MetaObject>
    {
        /**
         * Encapsulates one 'property' or 'field' for a MetaObject. Every MetaObject have
         * a list of these, which must have Unique Names within the same MetaObject.
         * The Value is the 'value' of the property, and the 'Name' its name
         */
        [ActiveType]
        public class Property : ActiveType<Property>
        {
            /**
             * The property's name
             */
            [ActiveField]
            public string Name { get; set; }

            /**
             * The property's value
             */
            [ActiveField]
            public string Value { get; set; }

            /**
             * The MetaObject this property belongs to
             */
            [ActiveField(BelongsTo = true)]
            public MetaObject ParentMetaObject { get; set; }

            internal Property Clone()
            {
                Property ret = new Property();
                ret.Name = Name;
                ret.Value = Value;
                return ret;
            }

            /**
             * Overridden to make sure Name's unique within MetaObject
             */
            public override void Save()
            {
                // Making sure every single property is uniquely named ...
                bool found = true;
                while (found)
                {
                    found = false;
                    Property other = ParentMetaObject.Values.Find(
                        delegate(Property idx)
                        {
                            return idx.Name == Name &&
                                idx != this;
                        });
                    if(other != null)
                    {
                        // 'This' added must 'yield' ...
                        Name += "_";
                        found = true;
                    }
                }
                base.Save();
            }
        }

        public MetaObject()
        {
            Values = new LazyList<Property>();
            Children = new LazyList<MetaObject>();
        }

        /**
         * The 'type' of your MetaObject. This property is being extensively used through out the entire
         * Magix to separate your MetaObjects into specific 'types' for you. By filtering upon these types,
         * different types of objects can emerge, such as 'Customer' and 'Contacts' and such. Though to
         * play well with others, you should probably prefix all your TypeNames somehow, with e.g. your
         * Company Name or something, to avoid clashing with other modules and other people's/company's 
         * MetaObjects
         */
        [ActiveField]
        public string TypeName { get; set; }

        /**
         * Normally an automatically generated reference of how the object came into existance. E.g.
         * the name of the View that created it, etc
         */
        [ActiveField]
        public string Reference { get; set; }

        /**
         * An automatically maintained value signifying when the object was created
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * All 'values', 'properties' or 'fields' of the object
         */
        [ActiveField]
        public LazyList<Property> Values { get; set; }

        /**
         * The child MetaObjects of this object. Every MetaObject can have as many children
         * as it sees fit. You can this way have one Customer having several Contacts for instance.
         * Later we will create possibilities within the MetaView system to react upon these, intelligently,
         * somehow, and allow the user to 'nest forms' and such. But currently this is unresolved
         * unfortunately. Please notice that these are 'loosely coupled objects', meaning they won't
         * be deleted when parent is deleted, and they can belong to more than one object
         */
        [ActiveField(IsOwner = false)]
        public LazyList<MetaObject> Children { get; set; }

        /**
         * The parent of the this object, if any
         */
        [ActiveField(BelongsTo = true, RelationName = "Children")]
        public MetaObject ParentMetaObject { get; set; }

        /**
         * Making sure every child has this as its parent to avoid 'cache bugs'. Plus other
         * types of logic
         */
        public override void Save()
        {
            // Making sure everything is like it should be ... ;)
            if (Children.ListRetrieved)
            {
                foreach (MetaObject idx in Children)
                {
                    idx.ParentMetaObject = this;
                }
            }

            MetaObject idxParent = ParentMetaObject;

            while (idxParent != null)
            {
                if (idxParent == this)
                    throw new ArgumentException("You can't have cyclic relationships with your objects ... Sorry ... :(");
                idxParent = idxParent.ParentMetaObject;
            }

            if (ID == 0)
                Created = DateTime.Now;

            // In case of 'caching issues' ...
            // Plus to assure uniqueness of Names of properties ...
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            foreach (Property idx in Values)
            {
                string name = idx.Name ?? "";
                while (dict.ContainsKey(name))
                {
                    name += "_";
                }
                dict[name] = true;
                idx.ParentMetaObject = this;
                idx.Name = name;
            }

            // Making sure the reference at the very least says; Anonymous Coward ...
            if (string.IsNullOrEmpty(Reference))
                Reference = "[Anonymous-Coward-Reference]";
            base.Save();
        }

        /**
         * Will return a deep copy of the entire Meta Object, with all its Child Objects being cloned,
         * and all its properties being cloned. WARNING; Might take insane amounts of time, if 
         * your object graph is huge
         */
        public MetaObject Clone()
        {
            return DeepClone(this);
        }

        private MetaObject DeepClone(MetaObject metaObject)
        {
            MetaObject ret = new MetaObject();
            ret.TypeName = TypeName;
            ret.Reference = "cloned: " + ID;
            foreach (Property idx in Values)
            {
                Property v = idx.Clone();
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
