/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;

namespace Magix.Brix.Data.Internal
{
    /**
      * Static helper class for data-storage Adapter developers.
      */
    public static class Helpers
    {
        public static string TypeName(Type type)
        {
            ActiveTypeAttribute[] attr = 
                type.GetCustomAttributes(typeof(ActiveTypeAttribute), true)
                as ActiveTypeAttribute[];
            if (attr != null &&
                attr.Length > 0 &&
                !string.IsNullOrEmpty(attr[0].TableName))
                return attr[0].TableName;
            return "doc" + type.FullName;
        }

        public static string PropertyName(PropertyInfo prop)
        {
            return "prop" + prop.Name;
        }

        public static string PropertyName(string propName)
        {
            return "prop" + propName;
        }
    }

    /// Static helper class for data-storage Adapter developers.
    public static class CopyOfHelpers
    {
        public static string TypeName(Type type)
        {
            return "doc" + type.FullName;
        }

        public static string PropertyName(PropertyInfo prop)
        {
            return "prop" + prop.Name;
        }

        public static string PropertyName(string propName)
        {
            return "prop" + propName;
        }
    }
}
