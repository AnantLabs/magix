/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Types;
using System.Diagnostics;

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
        private Node _params;

        [DebuggerStepThrough]
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
        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
        }

        /**
         * This is the "initializationObject" passed into your RaiseEvent call. Use this 
         * parameter to pass around data between components.
         */
        public Node Params
        {
            [DebuggerStepThrough]
            get { return _params; }
            [DebuggerStepThrough]
            set { _params = value; }
        }
    }
}
