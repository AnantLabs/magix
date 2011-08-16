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
     * Level3: Interface for all 'Doxygen Support Types of Item Types' around here. Basically
     * either a Property, Method, Delegate or something other
     */
    public interface IDocItem
    {
        /**
         * Level3: The id of the item
         */
        string ID { get; }

        /**
         * Level3: The 'Doxygen Kind' of item ['class', 'struct', 'enum' etc]
         */
        string Kind { get; }

        /**
         * Level3: Its full name
         */
        string FullName { get; }

        /**
         * Level3: And [finally] its documentation
         */
        string Description { get; }
    }
}
