/*
 * Magix-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using Magix.Brix.Data.Internal;

namespace Magix.Brix.Data
{
    // TODO: Refactor ...
    /**
     * Baseclass for 'internal usage' behind ActiveTypes. Definitely candidate for refactoring.
     * In general do NOT reference any members from here directly, except the ID of course
     */
    public class TransactionalObject
    {
        public TransactionalObject()
        {
            ParentDocument = -1;
        }

        public virtual int ID { get; internal set; }

        public int ParentDocument { get; set; }

        public string ParentPropertyName { get; set; }

        public virtual void Save()
        { }
    }

    /**
     * Level3: Inherit your well known types or entity types - the types you want to serialize
     * to your database from this class giving the generic argument type as the type
     * you're creating. Notice that you also need to mark your types with the 
     * ActiveRecordAttribute attribute in addition to marking all your serializable 
     * properties with the ActiveFieldAttribute.
     */
    public class ActiveType<T> : TransactionalObject
    {
        protected static Type GetType(Type type)
        {
            string mapped =
                ConfigurationManager.AppSettings["typeMapping-" + type.FullName];
            if (!string.IsNullOrEmpty(mapped))
                return Adapter.ActiveTypes.Find(
                    delegate(Type idx)
                    {
                        return idx.FullName == mapped;
                    });

            // Returning original type ...
            return type;
        }

        /**
         * Level3: The data storage associated ID of the object. Often the primary
         * key if you're using a database as your data storage.
         */
        override public int ID { get; internal set; }

        /**
         * Level3: Returns the number of objects in your data storage which is of type T.
         */
        public static int Count
        {
            get { return Adapter.Instance.CountWhere(GetType(typeof(T)), null); }
        }

        /**
         * Level3: Returns the number of objects in your data storage which is of type T
         * where the given criterias are true
         */
        public static int CountWhere(params Criteria[] args)
        {
            int no = 0;
            if (args != null && args.Length > 0)
            {
                foreach (Criteria idx in args)
                {
                    if (idx is CritID)
                        no += 1;
                }
            }
            if (no > 0)
                return no;
            return Adapter.Instance.CountWhere(GetType(typeof(T)), args);
        }

        /**
         * Level3: Returns the object with the given ID from your data storage.
         * Will return null if either it's a 'type-clash' or object doesn't exist, which
         * are for all practical concerns for you, and should be, irrelevant
         */
        public static T SelectByID(int id)
        {
            return (T)Adapter.Instance.SelectByID(GetType(typeof(T)), id);
        }

        /**
         * Level3: Returns a list of objects with the given ID from your data storage
         */
        public static IEnumerable<T> SelectByIDs(params int[] ids)
        {
            foreach (int id in ids)
            {
                yield return (T)Adapter.Instance.SelectByID(GetType(typeof(T)), id);
            }
        }

        /**
         * Level3: Returns the first object from your data storage which are true
         * for the given criterias. Pass nothing () if no criterias are needed.
         */
        public static T SelectFirst(params Criteria[] args)
        {
            if (args != null && args.Length > 0 && args[0] is CritID)
            {
                return SelectByID((int)args[0].Value);
            }
            else
            {
                return (T)Adapter.Instance.SelectFirst(GetType(typeof(T)), null, args);
            }
        }

        /**
         * Level3: Returns all objects from your data storage that matches the
         * given criterias. Pass nothing () if you want all objects regardless
         * of any values or criterias.
         */
        public static IEnumerable<T> Select(params Criteria[] args)
        {
            bool hasId = false;
            if (args != null && args.Length > 0)
            {
                foreach (Criteria idx in args)
                {
                    if (idx is CritID)
                    {
                        hasId = true;
                        break;
                    }
                }
                if (hasId)
                {
                    foreach (Criteria idx in args)
                    {
                        if (idx is CritID)
                        {
                            T tmp = (T)Adapter.Instance.SelectByID(
                                GetType(typeof(T)), 
                                (int)idx.Value);
                            if (tmp != null)
                                yield return tmp;
                        }
                    }
                }
            }
            if (!hasId)
            {
                foreach (object idx in 
                    Adapter.Instance.Select(GetType(typeof(T)), null, args))
                {
                    yield return (T)idx;
                }
            }
        }

        /**
         * Level3: Deletes the object from your data storage.
         */
        virtual public void Delete()
        {
            Adapter.Instance.Delete(ID);
        }

        private bool _isSaving;

        /**
         * Level3: Save the object to your data storage. This should normally be overridden
         * to make sure the end user isn't messing up your domain model somehow
         */
        override public void Save()
        {
            if (_isSaving)
                return; // To avoid recursive relationships where this is a child of a child of this or something ....
            _isSaving = true;
            try
            {
                Adapter.Instance.Save(this);
            }
            finally
            {
                _isSaving = false;
            }
        }

        /**
         * Level3: Returns true if the given object have the same ID as the this object.
         */
        public override bool Equals(object obj)
        {
            // Since theoretically user might have created an object and assigned an ID to
            // it, we still need to check for type before de-referencing here ... :|
            return obj != null && (obj is ActiveType<T>) && (obj as ActiveType<T>).ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /**
         * Level3: Will return the ID of the ActiveType. Little bit of 'debugging helping'
         */
        public override string ToString()
        {
            return ID.ToString();
        }

        /**
         * Level3: Returns true if the two given objects does not have the same ID.
         */
        public static bool operator != (ActiveType<T> left, ActiveType<T> right)
        {
            return !(left == right);
        }

        /**
         * Level3: Returns true if the two given objects do have the same ID.
         */
        public static bool operator ==(ActiveType<T> left, ActiveType<T> right)
        {
            if ((object)left == null && (object)right != null)
                return false;
            return (object)left == null || left.Equals(right);
        }
    }
}
