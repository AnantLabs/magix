﻿/*
 * Magix-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Collections.Generic;
using Magix.Brix.Data.Internal;
using System.Data.Common;

namespace Magix.Brix.Data
{
    public class TransactionalObject
    {
        public virtual int ID { get; internal set; }

        public DbTransaction Transaction { get; set; }

        public int ParentDocument { get; set; }

        public string ParentPropertyName { get; set; }

        public virtual void Save()
        { }

        public void Reset()
        {
            Transaction = null;
        }
    }

    /**
     * Inherit your well known types or entity types - the types you want to serialize
     * to your database from this class giving the generic argument type as the type
     * you're creating. Notice that you also need to mark your types with the 
     * ActiveRecordAttribute attribute in addition to marking all your serializable 
     * properties with the ActiveFieldAttribute.
     */
    public class ActiveType<T> : TransactionalObject
    {
        /**
         * The data storage associated ID of the object. Often the primary
         * key if you're using a database as your data storage.
         */
        override public int ID { get; internal set; }

        /**
         * Returns the number of objects in your data storage which is of type T.
         */
        public static int Count
        {
            get { return Adapter.Instance.CountWhere(typeof(T), null); }
        }

        /**
         * Returns the number of objects in your data storage which is of type T
         * where the given criterias are true.
         */
        public static int CountWhere(params Criteria[] args)
        {
            if (args != null && args.Length > 0 && args[0] is CritID)
            {
                return args.Length;
            }
            return Adapter.Instance.CountWhere(typeof(T), args);
        }

        /**
         * Returns the object with the given ID from your data storage.
         */
        public static T SelectByID(int id)
        {
            return (T)Adapter.Instance.SelectByID(typeof(T), id);
        }

        /**
         * Returns a list of objects with the given ID from your data storage.
         */
        public static IEnumerable<T> SelectByIDs(params int[] ids)
        {
            foreach (int id in ids)
            {
                yield return (T)Adapter.Instance.SelectByID(typeof(T), id);
            }
        }

        /**
         * Returns the first object from your data storage which are true
         * for the given criterias. Pass null if no criterias are needed.
         */
        public static T SelectFirst(params Criteria[] args)
        {
            if (args != null && args.Length > 0 && args[0] is CritID)
            {
                return SelectByID((int)args[0].Value);
            }
            else
            {
                return (T)Adapter.Instance.SelectFirst(typeof(T), null, args);
            }
        }

        /**
         * Returns all objects from your data storage that matches the
         * given criterias. Pass null if you want all objects regardless
         * of any values or criterias.
         */
        public static IEnumerable<T> Select(params Criteria[] args)
        {
            if (args != null && args.Length > 0 && args[0] is CritID)
            {
                foreach (CritID idx in args)
                {
                    yield return (T)Adapter.Instance.SelectByID(typeof(T), (int)idx.Value);
                }
            }
            else
            {
                foreach (object idx in Adapter.Instance.Select(typeof(T), null, args))
                {
                    yield return (T)idx;
                }
            }
        }

        /**
         * Deletes the object from your data storage.
         */
        virtual public void Delete()
        {
            Adapter.Instance.Delete(ID);
        }

        /**
         * Save the object to your data storage.
         */
        override public void Save()
        {
            Adapter.Instance.Save(this);
        }

        /**
         * Returns true if the given object have the same ID as the this object.
         */
        public override bool Equals(object obj)
        {
            return obj != null && (obj is ActiveType<T>) && (obj as ActiveType<T>).ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        /**
         * Returns true if the two given objects does not have the same ID.
         */
        public static bool operator != (ActiveType<T> left, ActiveType<T> right)
        {
            if ((object)left == null && (object)right != null)
                return true;
            return (object) left != null && !left.Equals(right);
        }

        /**
         * Returns true if the two given objects do have the same ID.
         */
        public static bool operator ==(ActiveType<T> left, ActiveType<T> right)
        {
            if ((object)left == null && (object)right != null)
                return false;
            return (object)left == null || left.Equals(right);
        }
    }
}