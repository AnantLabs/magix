/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.Brix.Data
{
    /**
     * Mark your well known types or entity types with this attribute to make them serializable.
     * In addition you must inherit from ActiveRecord with the type of the type you're creating
     * as the generic type argument. Notice that this attribute is for classes, you still need to
     * mark every property that you wish to serialize with the ActiveFieldAttribute.
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ActiveTypeAttribute : Attribute
    {
        public string TableName;
    }
}
