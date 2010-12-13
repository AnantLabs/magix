/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
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
    }
}
