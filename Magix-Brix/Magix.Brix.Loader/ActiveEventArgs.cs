/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;

namespace Magix.Brix.Loader
{
    /**
     * EventArgs class that will be passed into your Magix-Brix events - the methods you mark with
     * the ActiveEvent Attribute. The Extra property will contain the "initializationObject" passed 
     * into the RaiseEvent.
     */
    public class ActiveEventArgs : EventArgs
    {
        private readonly string _name;
        private readonly Node _params;

        internal ActiveEventArgs(string name, Node pars)
        {
            _name = name;
            _params = pars;
        }

        /**
         * The name of the Active Event. Most Active Event Handlers will be mapped only to 
         * one Active Event, but ocassionally you'll have one Event Handler handling more
         * than one Event. For cases like this the Name property might be useful to understand
         * which event you're actually handling.
         */
        public string Name { get { return _name; } }

        /**
         * This is the "initializationObject" passed into your RaiseEvent call. Use this 
         * parameter to pass around data between components.
         */
        public Node Params { get { return _params; } }
    }
}
