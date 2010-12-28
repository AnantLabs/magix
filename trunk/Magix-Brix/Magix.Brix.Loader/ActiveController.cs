/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Web;
using Magix.Brix.Types;

namespace Magix.Brix.Loader
{
    /**
     * Helper class for simplifying some of the common tasks you'd normally
     * want to use from your controllers, such as Loading Modules etc.
     */
    public abstract class ActiveController
    {
        /**
         * Loads the given module and puts it into the given container.
         * Will create a Node object, pass it into the InitialLoading, and return
         * it back to the caller.
         */
        protected Node LoadModule(string name, string container)
        {
            Node node = new Node();
            LoadModule(name, container, node);
            return node;
        }

        /**
         * Loads the given module and puts it into your default container.
         */
        protected Node LoadModule(string name)
        {
            Node node = new Node();
            LoadModule(name, null, node);
            return node;
        }

        /**
         * Shorthand method for Loading a specific Module and putting it into
         * the given container.
         */
        protected void LoadModule(string name, string container, Node node)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("You have to specify which Module you want to load");
            ActiveEvents.Instance.RaiseLoadControl(name, container, node);
        }

        /**
         * Shorthand for raising events. Will return a node, initially created empty, 
         * but passed onto the Event Handler(s)
         */
        protected Node RaiseEvent(string eventName)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
            return node;
        }

        /**
         * Shorthand for raising events.
         */
        protected void RaiseEvent(string eventName, Node node)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
        }

        /**
         * Shorthand for getting access to our "Page" object.
         */
        protected Page Page
        {
            get
            {
                return (Page)HttpContext.Current.Handler;
            }
        }
    }
}
