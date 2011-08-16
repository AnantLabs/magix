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
     * Level3: One parameter to a Method
     */
    public class Parameter
    {
        /**
         * Level3: The parameter's type
         */
        public string Type { get; protected internal set; }

        /**
         * Level3: The variable name of the parameter
         */
        public string Name { get; protected internal set; }
    }
}
