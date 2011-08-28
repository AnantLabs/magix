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
     * Level3: A method type of member. Belongs to a class yes
     */
    public class Method : Member
    {
        public override string Kind
        {
            get { return "function"; }
        }

        /**
         * Level3: Its parameters
         */
        public List<Parameter> Parameters { get; protected internal set; }

        public Method()
        {
            Parameters = new List<Parameter>();
        }
    }
}
