﻿/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;

namespace Magix.Brix.Loader
{
    /**
     * Mark your controllers with this Attribute. Notice that an Active Controller must
     * have a default constructor taking zero parameters. This constructor should also
     * ideally execute FAST since all controllers in your Magix-Brix project will be 
     * instantiated once every request.
     */
    public class ActiveControllerAttribute : Attribute
    {
    }
}