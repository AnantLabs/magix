/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using Magix.Brix.Types;
using System.Diagnostics;

namespace Magix.Brix.Loader
{
    /**
     * Class contains methods for raising events and other helpers, like for instance helpers
     * to load controls and such.
     */
    public sealed class ActiveEvents
    {
        private readonly Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> _methods =
            new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
        private static ActiveEvents _instance;
        private Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> _nonWeb = 
            new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
        private static Dictionary<string, string> _eventMappers = new Dictionary<string, string>();

        private delegate void AsyncDelegate(object sender, ActiveEventArgs e);

        private ActiveEvents()
        { }

        /**
         * This is our Singleton to access our only ActiveEvents object. This is
         * the property you'd use to gain access to the only existing ActiveEvents
         * object in your application pool.
         */
        public static ActiveEvents Instance
        {
            [DebuggerStepThrough]
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ActiveEvents))
                    {
                        if (_instance == null)
                            _instance = new ActiveEvents();
                    }
                }
                return _instance;
            }
        }

        /**
         * Loads a control with the given name (class name) into the given position (name of Magix.UX.Dynamic in
         * the Viewport currently used). Use this method to load Modules. Notice
         * that there exists an overload of this method which takes an object parameter that will be 
         * passed into the InitialLoading method when control is loaded.
         */
        [DebuggerStepThrough]
        public void RaiseLoadControl(string name, string position)
        {
            RaiseLoadControl(name, position, null);
        }

        /**
         * Loads a control with the given name (class name) into the given position (name of Magix.UX.Dynamic in
         * the Viewport currently used). Use this method to load Modules. This overload of the method
         * will pass the "initializingArgument" parameter into the InitialLoading method when control 
         * is loaded.
         */
        [DebuggerStepThrough]
        public void RaiseLoadControl(string name, string position, Node parameters)
        {
            Node tmpNode = new Node("LoadControl");
            tmpNode["Name"].Value = name;
            tmpNode["Position"].Value = position;
            if (parameters == null)
                tmpNode["Parameters"].Value = null;
            else
                tmpNode["Parameters"].AddRange(parameters);
            RaiseActiveEvent(this, "LoadControl", tmpNode);
        }

        /**
         * Clear all controls out of the position (Ra-Dynamic) of your Viewport.
         */
        [DebuggerStepThrough]
        public void RaiseClearControls(string position)
        {
            Node tmp = new Node("ClearControls");
            tmp["Position"].Value = position;
            RaiseActiveEvent(this, "ClearControls", tmp);
        }

        /**
         * Raises an event with null as the initialization parameter.
         * This will dispatch control to all the ActiveEvent that are marked with
         * the Name attribute matching the name parameter of this method call.
         */
        [DebuggerStepThrough]
        public void RaiseActiveEvent(object sender, string name)
        {
            RaiseActiveEvent(sender, name, null);
        }

        [DebuggerStepThrough]
        private List<Tuple<MethodInfo, Tuple<object, bool>>> SlurpAllEventHandlers(string eventName)
        {
            List<Tuple<MethodInfo, Tuple<object, bool>>> retVal = 
                new List<Tuple<MethodInfo, Tuple<object, bool>>>();

            // Adding static methods (if any)
            if (_methods.ContainsKey(eventName))
            {
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in _methods[eventName])
                {
                    retVal.Add(idx);
                }
            }

            // Adding instance methods (if any)
            if (InstanceMethod.ContainsKey(eventName))
            {
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in InstanceMethod[eventName])
                {
                    retVal.Add(idx);
                }
            }
            return retVal;
        }

        [DebuggerStepThrough]
        private void ExecuteMethod(
            MethodInfo method, 
            object context, 
            bool async, 
            object sender, 
            ActiveEventArgs e)
        {
            if (async)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        try
                        {
                            method.Invoke(context, new[] { context, e });
                        }
                        catch (Exception)
                        {
                            // TODO: I have no idea what to do here...
                        }
                    });
            }
            else
            {
                method.Invoke(context, new[] { sender, e });
            }
        }

        /**
         * Raises an event. This will dispatch control to all the ActiveEvent that are marked with
         * the Name attribute matching the name parameter of this method call.
         */
        [DebuggerStepThrough]
        public void RaiseActiveEvent(
            object sender, 
            string name, 
            Node pars)
        {
            name = RaiseEventImplementation(sender, name, pars, name);
        }

        [DebuggerStepThrough]
        private string RaiseEventImplementation(object sender, string name, Node pars, string actualName)
        {
            // Dummy dereferencing of PluginLoader to make sure we've 
            // loaded all our assemblies and types first ...!
            PluginLoader typesMumboJumbo = PluginLoader.Instance;

            name = GetEventName(name);
            ActiveEventArgs e = new ActiveEventArgs(actualName, pars);
            if (name != "")
            {
                RaiseEventImplementation(sender, "", pars, actualName);
            }
            if (_methods.ContainsKey(name) || InstanceMethod.ContainsKey(name))
            {
                // We must run this in two operations since events clear controls out
                // and hence make "dead references" to Event Handlers and such...
                // Therefor we first iterate and find all event handlers interested in
                // this event before we start calling them one by one. But every time in
                // between calling the next one, we must verify that it still exists within
                // the collection...
                List<Tuple<MethodInfo, Tuple<object, bool>>> tmp = SlurpAllEventHandlers(name);

                // Looping through all methods...
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idx in tmp)
                {
                    // Since events might load and clear controls we need to check if the event 
                    // handler still exists after *every* event handler we dispatch control to...
                    List<Tuple<MethodInfo, Tuple<object, bool>>> recheck = SlurpAllEventHandlers(name);

                    foreach (Tuple<MethodInfo, Tuple<object, bool>> idx2 in recheck)
                    {
                        if (idx.Equals(idx2))
                        {
                            ExecuteMethod(idx.Left, idx.Right.Left, idx.Right.Right, sender, e);
                            break;
                        }
                    }
                }
            }
            return name;
        }

        public void CreateEventMapping(string from, string to)
        {
            _eventMappers[from] = to;
        }

        [DebuggerStepThrough]
        private string GetEventName(string name)
        {
            if (_eventMappers.ContainsKey(name))
                return _eventMappers[name];
            else
            {
                string mapped = ConfigurationManager.AppSettings["mapped-" + name];
                if (!string.IsNullOrEmpty(mapped))
                {
                    string evtName = mapped.Replace("mapped-", "");
                    _eventMappers[name] = evtName;
                }
                else
                {
                    // No mapping, defaulting to the default event name ...
                    _eventMappers[name] = name;
                }
                return _eventMappers[name];
            }
        }

        // TODO: Try to remove or make internal somehow...?
        public void RemoveListener(object context)
        {
            // Removing all event handler with the given context (object instance)
            foreach (string idx in InstanceMethod.Keys)
            {
                List<Tuple<MethodInfo, Tuple<object, bool>>> idxCur = InstanceMethod[idx];
                List<Tuple<MethodInfo, Tuple<object, bool>>> toRemove = 
                    new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idxObj in idxCur)
                {
                    if (idxObj.Right.Left == context)
                        toRemove.Add(idxObj);
                }
                foreach (Tuple<MethodInfo, Tuple<object, bool>> idxObj in toRemove)
                {
                    idxCur.Remove(idxObj);
                }
            }
        }

        private Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>> InstanceMethod
        {
            [DebuggerStepThrough]
            get
            {
                // NON-web scenario...
                if (HttpContext.Current == null)
                    return _nonWeb;

                Page page = (Page)HttpContext.Current.Handler;
                if (!page.Items.Contains("__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"))
                {
                    page.Items["__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"] =
                        new Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>();
                }
                return (Dictionary<string, List<Tuple<MethodInfo, Tuple<object, bool>>>>)
                    page.Items["__Ra.Brix.Loader.ActiveEvents._requestEventHandlers"];
            }
        }

        internal void AddListener(object context, MethodInfo method, string name, bool async)
        {
            if (name == null)
            {
                name = "";
            }
            if (context == null)
            {
                // Static event handler, will *NEVER* be cleared until application
                // itself is restarted
                if (!_methods.ContainsKey(name))
                    _methods[name] = new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                _methods[name].Add(new Tuple<MethodInfo, Tuple<object, bool>>(method, new Tuple<object, bool>(context, async)));
            }
            else
            {
                // Request "instance" event handler, will be tossed away when
                // request is over
                if (!InstanceMethod.ContainsKey(name))
                    InstanceMethod[name] = new List<Tuple<MethodInfo, Tuple<object, bool>>>();
                InstanceMethod[name].Add(new Tuple<MethodInfo, Tuple<object, bool>>(method, new Tuple<object, bool>(context, async)));
            }
        }
    }
}