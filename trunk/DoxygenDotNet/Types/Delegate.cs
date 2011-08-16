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
     * Level3: Encapsulates a delegate documentation item
     */
    public class Delegate : Type
    {
        public override string Kind
        {
            get { return "delegate"; }
        }

        public override List<Type> NestedTypes
        {
            get { return null; }
        }
    }
}
