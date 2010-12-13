/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;

namespace Magic.Brix.Loader
{
    /**
     * Mark your Active Modules with this attribute. If you mark your Modules with this attribute
     * you can load them using the PluginLoader.LoadControl method.
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ActiveModuleAttribute : Attribute
    {
    }
}
