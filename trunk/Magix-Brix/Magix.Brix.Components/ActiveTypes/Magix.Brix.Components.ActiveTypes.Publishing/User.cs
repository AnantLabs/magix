/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using U = Magix.Brix.Components.ActiveTypes.Users;
using System.Collections.Generic;
using System.Web;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    [ActiveType(TableName = "docMagix.Brix.Components.ActiveTypes.Publishing.User")]
    public class User : U.UserBase
    {
        public User()
        {
            OpenIDTokens = new LazyList<OpenIDToken>();
        }

        [ActiveField]
        public string FullName { get; set; }

        [ActiveField]
        public string AvatarURL { get; set; }

        [ActiveField]
        public string Phone { get; set; }

        [ActiveField]
        public string Address { get; set; }

        [ActiveField]
        public string City { get; set; }

        [ActiveField]
        public string Zip { get; set; }

        [ActiveField]
        public string State { get; set; }

        [ActiveField]
        public string Twitter { get; set; }

        [ActiveField]
        public string Facebook { get; set; }

        [ActiveField]
        public LazyList<OpenIDToken> OpenIDTokens { get; set; }

        #region [ -- Business Logic -- ]

        // TODO: Check up if these are redundant ...!
        /**
         * Returns the object with the given ID from your data storage.
         */
        public static new User SelectByID(int id)
        {
            return (User)Adapter.Instance.SelectByID(typeof(User), id);
        }

        /**
         * Returns a list of objects with the given ID from your data storage.
         */
        public static new IEnumerable<User> SelectByIDs(params int[] ids)
        {
            foreach (int id in ids)
            {
                yield return (User)Adapter.Instance.SelectByID(
                    typeof(User), id);
            }
        }

        /**
         * Returns the first object from your data storage which are true
         * for the given criterias. Pass null if no criterias are needed.
         */
        public static new User SelectFirst(params Criteria[] args)
        {
            if (args != null && args.Length > 0 && args[0] is CritID)
            {
                return SelectByID((int)args[0].Value);
            }
            else
            {
                return (User)Adapter.Instance.SelectFirst(
                    typeof(User), null, args);
            }
        }

        /**
         * Returns all objects from your data storage that matches the
         * given criterias. Pass null if you want all objects regardless
         * of any values or criterias.
         */
        public static new IEnumerable<User> Select(params Criteria[] args)
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
                            User tmp = (User)Adapter.Instance.SelectByID(
                                typeof(User),
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
                    Adapter.Instance.Select(
                    typeof(User), null, args))
                {
                    yield return (User)idx;
                }
            }
        }

        /**
         * Returns the number of objects in your data storage which is of type T.
         */
        public static new int Count
        {
            get { return Adapter.Instance.CountWhere(GetType(typeof(User)), null); }
        }

        /**
         * Returns the number of objects in your data storage which is of type User
         * where the given criterias are true.
         */
        public static new int CountWhere(params Criteria[] args)
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
            return Adapter.Instance.CountWhere(GetType(typeof(User)), args);
        }

        public new static User Current
        {
            get
            {
                return U.UserBase.Current as User;
            }
            set
            {
                U.UserBase.Current = value;
            }
        }

        public override void Save()
        {
            if (string.IsNullOrEmpty(AvatarURL))
            {
                Node node = new Node();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Publishing.GetDefaultGravatarURL",
                    node);
                AvatarURL = node["URL"].Get<string>();
            }
            base.Save();
        }

        #endregion
    }
}
