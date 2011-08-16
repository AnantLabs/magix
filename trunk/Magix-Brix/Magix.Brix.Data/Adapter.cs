/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;

namespace Magix.Brix.Data
{
    // TODO: Map reduce db engine ... Hmmmmm !! :))))
    /**
     * Level4: Abstract base class for all Database Adapters in Magix-Brix. If you wish
     * to build your own data adapter then inherit from this class and implement
     * the abstract methods, add up a reference to the dll and change the 
     * data-configuration line in your configuration file and it should work.
     * Class provides some common functions needed for all database adapters
     * like caching, instantiation and such in addition.
     */
    public abstract class Adapter
    {
        private static Adapter _adapter;
        private static ConstructorInfo _ctorToAdapter;
        private readonly static List<Type> _activeTypes = new List<Type>();
        private readonly static List<Type> _activeModules = new List<Type>();

        /**
         * Level4: Retrieves the configured database adapter. Notice that you would very rarely
         * want to use this directly but instead access it indirectly through your 
         * ActiveType classes
         */
        public static Adapter Instance
        {
            get
            {
                if (HttpContext.Current == null ||
                    HttpContext.Current.CurrentHandler == null ||
                    (HttpContext.Current.CurrentHandler as Page) == null)
                {
                    // Not web...!
                    // Probably Unit Tests ...
                    if (_adapter == null)
                        _adapter = CreateAdapter();
                    return _adapter;
                }

                // Web!!
                Page page = HttpContext.Current.CurrentHandler as Page;
                page.Unload += PageUnload;
                if (page.Items["__LegoDataAdapter"] == null)
                {
                    Adapter adapter = CreateAdapter();
                    page.Items["__LegoDataAdapter"] = adapter;
                }
                return page.Items["__LegoDataAdapter"] as Adapter;
            }
        }

        private static Adapter CreateAdapter()
        {
            if (_ctorToAdapter == null)
            {
                lock (typeof(Adapter))
                {
                    if (_ctorToAdapter == null)
                        _ctorToAdapter = GetAdapterConstructor();
                }
            }

            Adapter adapter = _ctorToAdapter.Invoke(null) as Adapter;
            if (adapter == null)
            {
                string adapterType = ConfigurationManager.AppSettings["LegoDatabaseAdapter"];
                throw new ConfigurationErrorsException("Unknown database adapter type '" + adapterType + "' or couldn't cast to type Adapter. Make sure your Database adapter implements the abstract class 'Adapter'");
            }
            string connectionString = ConfigurationManager.AppSettings["LegoConnectionString"];
            adapter.Open(connectionString);
            return adapter;
        }

        /**
         * Level4: A list of all your ActiveTypes in the system
         */
        public static List<Type> ActiveTypes
        {
            get
            {
                return _activeTypes;
            }
        }

        /**
         * Level4: A list of all your ActiveModules in the system
         */
        public static List<Type> ActiveModules
        {
            get
            {
                return _activeModules;
            }
        }

        private static ConstructorInfo GetAdapterConstructor()
        {
            string adapterType = ConfigurationManager.AppSettings["LegoDatabaseAdapter"];
            foreach (Assembly idxAsm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (idxAsm.GlobalAssemblyCache)
                    continue;
                try
                {
                    foreach (Type idxType in idxAsm.GetTypes())
                    {
                        if (adapterType == idxType.FullName)
                        {
                            return idxType.GetConstructor(new Type[] { });
                        }
                    }
                }
                catch
                {
                    ; // Intentionally do nothing...
                }
            }
            return null;
        }

        /**
         * Level4: Careful here. This is often a VERY expensive operation if you're doing it frequently.
         * Though sometimes needed I guess. This one is mostly here for being able to go 
         * 'completely reset' in case of transactional rollbacks and such. If you use this
         * one directly yourself, you'd better be sure you know what you're doing. 
         * Since alternatively you'd win the 'slowest system on the planet' award ... ;)
         */
        public void InvalidateCache()
        {
            Cache.Clear();
        }

        /**
         * Level4: DO NOT TOUCH this one either. It's exclusively here [really] for internal usage
         * in regards to transaction support for rollbacks and such
         */
        public abstract void ResetTransaction();

        private Dictionary<int, object> _cache = new Dictionary<int, object>();

        /**
         * Level4: Dictionary of ID, ActiveType objects built up during the Request. Every ActiveType object
         * being fetched during on HTTP request [one postback] is being held in a cache for the duration
         * of the rest of that request on a 'per request basis'. This is because research have shown that
         * even though completely different modules, unknown to each other really, are constantly running
         * in paralel, they often tend to react upon the same actual list of objects. Meaning that by
         * caching everything on one request like this we loose basically nothing, since we're in GC 
         * land anyway, and aren't really 'locking up memory' in anyways. While we also get to have a 
         * __BLISTERING__ fast Data Adapter technology, since after it's in the cache, the criterias
         * will only execute to fetch the ID of the object you're requesting,
         * if you're using 'complex queries', and if you're querying
         * directly for the ID's, you'll get immediately here upon requesting your ActiveTypes. Hence;
         * Hoahh ...! SWOSH ...! :P
         */
        public Dictionary<int, object> Cache
        {
            get
            {
                return _cache;
            }
        }

        /*
         * Tidying up ...
         */
        static void PageUnload(object sender, EventArgs e)
        {
            Instance.Close();
        }

        /**
         * Level4: Retrieves an object with the given ID from your data storage.
         */
        public object SelectByID(Type type, int id)
        {
            if (Cache.ContainsKey(id) && Cache[id].GetType() == type)
                return Cache[id];
            object retVal = SelectObjectByID(type, id);
            return retVal;
        }

        /**
         * Level4: Deletes the object with the given ID from your data storage.
         */
        public void Delete(int id)
        {
            DeleteObject(id);
            Cache.Remove(id);
        }

        /**
         * Level4: Should return the number of items of the given type from
         * your data storage with the given criterias.
         */
        public abstract int CountWhere(Type type, params Criteria[] args);

        /**
         * Level4: Should return the object from your data storage with the given ID
         * being of the given Type.
         */
        protected abstract object SelectObjectByID(Type type, int id);

        /**
         * Level4: Should return the first object of type; "type" - with the given criterias.
         */
        public abstract object SelectFirst(Type type, string propertyName, params Criteria[] args);

        /**
         * Level4: Should return all objects of the given type with the given criterias.
         */
        public abstract IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args);

        /**
         * Level4: Should return some sort of string identification of the underlaying datasource
         */
        public abstract string GetConnectionString();

        /**
         * Level4: Begins a new transaction object, which ensures the entire opertion within
         * the scope of the transaction object will be either saved or rejected, 
         * and thrown an exception from ...
         */
        public abstract Transaction BeginTransaction();

        /**
         * Level4: Should return all objects in your data storage.
         */
        public abstract IEnumerable<object> Select();

        /**
         * Level4: Should delete the object with the given ID from your data storage.
         */
        protected abstract void DeleteObject(int id);

        /**
         * Level4: Should save the given object into your data storage.
         */
        public abstract void Save(object value);

        /**
         * Level4: Called when data storage should close. Often used to
         * close file handles or database connections etc.
         */
        public abstract void Close();

        /**
         * Level4: Called when your data storage should be opened. Often used to
         * open file handles or database connections etc.
         */
        public abstract void Open(string connectionString);
    }
}
