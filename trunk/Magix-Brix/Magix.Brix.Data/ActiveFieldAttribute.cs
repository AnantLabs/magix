/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.Brix.Data
{
    /**
     * Level3: Used to mark entity objects as serializable. If a property is
     * marked with this attribute then it will be possible to serialise
     * that property. Notice that you still need to mark you classes with the
     * ActiveRecordAttribute. Also only properties, and not fields and such
     * can be marked as serializable with this attribute.
     */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class ActiveFieldAttribute : Attribute
    {
        /**
         * Level3: If true then this is a one-to-x relationship which
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
         * Level3: Sometimes if you have dual relationships, this property might be
         * useful for marking the name of the property controlling the relationship.
         * If you have one type containing for instance a relationship to another type,
         * and the other type also contains a relationship for the first type, and both
         * of these relationship are really the same relationship, then you can set the 
         * RelationName to the name of the property in the controlling class of the 
         * relationship.
         */
        public string RelationName;

        /**
         * Level3: If true, then the other side of the relationship controls the relationship.
         * If you in addition supply a RelationName in combination with this property,
         * it'll assume there's a many2many composition, otherwise it'll assume ownership
         * from the other side.
         */
        public bool BelongsTo;
    }
}
