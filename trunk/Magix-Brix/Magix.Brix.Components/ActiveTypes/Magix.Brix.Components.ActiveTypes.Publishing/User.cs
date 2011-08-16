/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    // TODO: WTF ..? ['TableName']
    /**
     * Encapsulates one user in Magix' Publishing system. Inherited to add more,
     * specific to the publishing system, to the class
     */
    [ActiveType(TableName = "docMagix.Brix.Components.ActiveTypes.Publishing.User")]
    public class User : U.UserBase
    {
        public User()
        {
            OpenIDTokens = new LazyList<OpenIDToken>();
        }

        /**
         * Full name of person
         */
        [ActiveField]
        public string FullName { get; set; }

        /**
         * URL, can either be relative [media/images/myImage.jpg] or absolute [http://g.com/x.jpg]
         */
        [ActiveField]
        public string AvatarURL { get; set; }

        /**
         * Phone number to person
         */
        [ActiveField]
        public string Phone { get; set; }

        /**
         * Street Address to person
         */
        [ActiveField]
        public string Address { get; set; }

        /**
         * City where person lives
         */
        [ActiveField]
        public string City { get; set; }

        /**
         * Zip, yup ...
         */
        [ActiveField]
        public string Zip { get; set; }

        /**
         * State [if US. Ignore if other countries]
         */
        [ActiveField]
        public string State { get; set; }

        /**
         * The Twitter Handle to the person, e.g. 'WinergyInc'
         */
        [ActiveField]
        public string Twitter { get; set; }

        /**
         * The Facebook username to the person, e.g. 'polterguy'
         */
        [ActiveField]
        public string Facebook { get; set; }

        /**
         * Date of birth
         */
        [ActiveField]
        public DateTime BirthDate { get; set; }

        /**
         * Country where person lives
         */
        [ActiveField]
        public string Country { get; set; }

        /**
         * Yup, normally 'Male' or 'Female'
         */
        [ActiveField]
        public string Gender { get; set; }

        /**
         * Come estas? Comprende ...?
         */
        [ActiveField]
        public string Language { get; set; }

        // TODO: Unique within app ...?
        /**
         * A small little personal 'nickname', e.g. 'polterguy'
         */
        [ActiveField]
        public string Nickname { get; set; }

        /**
         * Which timezone is person within
         */
        [ActiveField]
        public string TimeZone { get; set; }

        /**
         * List of OpenID Claims associated with person
         */
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

        /**
         * Overridden to make sure we've got a default AvatarURL before calling base
         */
        public override void Save()
        {
            if (string.IsNullOrEmpty(AvatarURL))
            {
                Node node = new Node();

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Publishing.GetDefaultAvatarURL",
                    node);

                AvatarURL = node["URL"].Get<string>();
            }
            base.Save();
        }

        #endregion
    }
}
