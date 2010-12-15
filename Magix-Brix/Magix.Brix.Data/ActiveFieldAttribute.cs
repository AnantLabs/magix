/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;

namespace Magix.Brix.Data
{
    /**
     * Used to mark entity objects as serializable. If a property is
     * marked with this attribute then it will be possible to serialise
     * that property. Notice that you still need to mark you classes with the
     * ActiveRecordAttribute. Also only properties, and not fields and such
     * can be marked as serializable with this attribute.
     */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class ActiveFieldAttribute : Attribute
    {
        /**
         * If true then this is a one-to-x relationship which
         * means that the type owns this instance and will also delete
         * the instance if the object itself is deleted. If it is false
         * then this indicate a many-to-x relationship 
         * and means that the object does NOT own this property and the 
         * property will NOT be deleted when the object is deleted.
         * If it is false then the property will also NOT be saved whenever
         * the not owning object is being saved.
         * Default value is true - which means that the object will
         * be saved when parent object is saved, and also deleted when
         * the parent object is being deleted.
         */
        public bool IsOwner = true;

        /**
         * Sometimes if you have dual relationships, this property might be
         * useful for marking the name of the property controlling the relationship.
         * If you have one type containing for instance a relationship to another type,
         * and the other type also contains a relationship for the first type, and both
         * of these relationship are really the same relationship, then you can set the 
         * RelationName to the name of the property in the controlling class of the 
         * relationship.
         */
        public string RelationName;

        /**
         * If true, then the other side of the relationship controls the relationship.
         */
        public bool BelongsTo;
    }
}
