/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveTypes
{
    /**
     * Level2: Wrapper class for common/global settings within a Magix-Brix application.
     * Use the overloaded this operator to access Settings through code
     */
    public sealed class Settings
    {
        /**
         * Level4: Not really exposed out, so shouldn't ever be any reasons to 
         * even know about, but still left documented here for reference purposes
         */
        [ActiveType]
        public class Setting : ActiveType<Setting>
        {
            /**
             * Level4: The key part of the setting. Used as 'ID'ish
             */
            [ActiveField]
            public string Key { get; set; }

            /**
             * Level4: The actual setting value. All settings are stored internally as strings,
             * and then later converted using InvariantCulture back and forth
             */
            [ActiveField]
            public string Value { get; set; }
        }

        private static Settings _instance;
        private List<Setting> _values;

        private Settings()
        {
            _values = new List<Setting>(Setting.Select());
        }

        /**
         * Level3: Retrieves the singleton instance of your settings. Reference the Settings ActiveTypes
         * in your project, use the Instance property from below, and use the index operator
         * of the class or Get/Set to access specific settings
         */
        public static Settings Instance
        {
            get
            {
                // Checking to see if settings haven't been accessed before...
                if (_instance == null)
                {
                    // Need to do some race condition logic here ...
                    lock (typeof(Settings))
                    {
                        if (_instance == null)
                        {
                            _instance = new Settings();
                        }
                    }
                }
                return _instance;
            }
        }

        /**
         * Level3: Returns or sets the key value as a string
         */
        public string this[string key]
        {
            get
            {
                Setting retVal = _values.Find(
                    delegate(Setting idx)
                    {
                        return idx.Key == key;
                    });
                if (retVal == null)
                {
                    // Setting didn't exist in database, checking .config file...
                    return ConfigurationManager.AppSettings[key];
                }
                return retVal.Value;
            }
            set
            {
                // Verifying that value actually changed...!
                if (value == this[key])
                    return;
                lock (this)
                {
                    Setting val = _values.Find(
                        delegate(Setting idx)
                        {
                            return idx.Key == key;
                        });
                    if (val == null)
                    {
                        val = new Setting();
                        val.Key = key;
                        _values.Add(val);
                    }
                    val.Value = value;
                    val.Save();
                }
            }
        }

        /**
         * Level3: Retrieves the setting with the specific key, and converts it to typeof(T).
         * If the setting doesn't exists, a default value will be created and saved
         * with the value of defaultValue and then returned to caller.
         */
        public T Get<T>(string key, T defaultValue)
        {
            string val = this[key];
            if (val == null)
            {
                Set(key, defaultValue);
                return defaultValue;
            }
            return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }

        /**
         * Level3: Retrieves the setting with the specific key, and converts it to typeof(T).
         * If the setting doesn't exists, default(T) will be returned.
         */
        public T Get<T>(string key)
        {
            string val = this[key];
            if (val == null)
                return default(T);
            return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }

        /**
         * Level3: Sets value of key to val typeof(T)
         */
        public void Set<T>(string key, T newValue)
        {
            lock (this)
            {
                Setting val = _values.Find(
                    delegate(Setting idx)
                    {
                        return idx.Key == key;
                    });
                if (val == null)
                {
                    val = new Setting();
                    val.Key = key;
                    _values.Add(val);
                }
                val.Value = (string)Convert.ChangeType(newValue, typeof(string), CultureInfo.InvariantCulture);
                val.Save();
            }
        }

        /**
         * Level4: Returns all the keys which exists.
         */
        public IEnumerable<string> Keys
        {
            get
            {
                foreach (Setting idx in _values)
                {
                    yield return idx.Key;
                }
            }
        }

        /**
         * Level3: Will reload all settings from DB ...
         */
        public void Reload()
        {
            _instance = null;
        }

        /**
         * Level4: Will delete all settings. Notice that this operation cannot be undone!
         */
        public void Clear()
        {
            foreach (Setting idx in Setting.Select())
            {
                idx.Delete();
            }
            _values.Clear();
        }

        /**
         * Level3: Returns the number of settings in total in database
         */
        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        /**
         * Level3: Removes the specific Setting Key form the collection
         */
        public void Remove(string key)
        {
            lock(this)
            {
                foreach (Setting idx in _values)
                {
                    if (idx.Key == key)
                    {
                        idx.Delete();
                        _values.Remove(idx);
                        break;
                    }
                }
            }
        }
    }
}