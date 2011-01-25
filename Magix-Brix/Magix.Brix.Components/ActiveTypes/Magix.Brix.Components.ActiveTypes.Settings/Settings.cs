/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveTypes
{
    /**
     * Wrapper class for common/global settings within a Magix-Brix application.
     */
    public sealed class Settings
    {
        [ActiveType]
        public class Setting : ActiveType<Setting>
        {
            [ActiveField]
            public string Key { get; set; }

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
         * Retrieves the singleton instance of your settings.
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
         * Returns or sets the key value as a string
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
         * Retrieves the setting with the specific key, and converts it to typeof(T).
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
         * Retrieves the setting with the specific key, and converts it to typeof(T).
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
         * Sets value of key to val typeof(T)
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
         * Returns all the keys which exists.
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
         * Will delete all settings. Notice that this operation cannot be undone!
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
         * Returns the number of settings in total in database
         */
        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

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