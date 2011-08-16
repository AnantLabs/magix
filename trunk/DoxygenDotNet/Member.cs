/*
 * Doxygen.NET - .NET object wrappers for Doxygen
 * Copyright 2009 - Ra-Software AS
 * This code is licensed under the LGPL version 3.
 * 
 * Authors: 
 * Thomas Hansen (thomas@ra-ajax.org)
 * Kariem Ali (kariem@ra-ajax.org)
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Doxygen.NET
{
    /**
     * Level3: A Member documentation item. Basically a property, field, method etc within
     * a class
     */
    public class Member: IDocItem
    {
        #region IDocItem Members

        public virtual string ID { get; protected internal set; }
        public virtual string Kind { get; protected internal set; }
        public virtual string FullName { get; protected internal set; }
        public virtual string Description { get; protected internal set; }

        #endregion

        /**
         * Level3: The name of the member
         */
        public string Name { get; protected internal set; }

        /**
         * Level3: Protected, private, public or internal
         */
        public virtual string AccessModifier { get; protected internal set; }

        /**
         * Level3: The member's return type, if any
         */
        public virtual string ReturnType { get; protected internal set; }

        /**
         * Level3: Its parent. Usually a class
         */
        public Type Parent { get; protected internal set; }
    }
}
