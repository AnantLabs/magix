/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.Web;
using Magix.Brix.Types;
using System.Diagnostics;
using System.Collections.Generic;

namespace Magix.Brix.Loader
{
    /**
     * Level3: Helper class for simplifying some of the common tasks you'd normally
     * want to use from your controllers, such as Loading Modules, raising events etc.
     * Inherit your controllers from this class if you'd like to add more 'power' to
     * them
     */
    public abstract class ActiveController
    {
        /**
         * Level3: Helper for executing 'dangerous code' such that if an exception happens,
         * it'll 'swallow' the exception, and show a Message box showing the exception
         */
        protected delegate void executor();

        /**
         * Level3: Helper for retrieving cached items
         */
        protected delegate T executor<T>();

        /**
         * Level3: Loads the given module and puts it into your default container
         */
        [DebuggerStepThrough]
        protected Node LoadModule(string name)
        {
            Node node = new Node();
            LoadModule(name, null, node);
            return node;
        }

        /**
         * Level3: Helper for execute code that you suspect might throw exceptions. Will trap exception and
         * show a message box back to end user instead of allowing exception to penetrate through
         * to Yellow Screen of Death. Will return true if operation didn't throw an exception
         * and false if it did throw an exception
         */
        [DebuggerStepThrough]
        protected bool ExecuteSafely(executor functor, string msg, params object[] args)
        {
            try
            {
                functor();
                return true;
            }
            catch (Exception err)
            {
                while (err.InnerException != null)
                    err = err.InnerException;

                Node node = new Node();

                node["Message"].Value =
                    "<p>" + string.Format(msg, args) + "</p>" +
                    "<p>Message from Server; </p>" +
                    "<p>" + err.Message + "</p>" +
                    "<p>Stack Trace; </p>" +
                    "<p class='mux-err-stack-trace'>" + err.StackTrace + "</p>";

                node["Header"].Value = err.GetType().FullName;
                node["IsError"].Value = true;

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
            }
            return false;
        }

        /**
         * Level3: Loads the given module and puts it into the given container. Will return the node
         * created and passed into creation
         */
        [DebuggerStepThrough]
        protected Node LoadModule(string name, string container)
        {
            Node node = new Node();
            LoadModule(name, container, node);
            return node;
        }

        /**
         * Level3: Shorthand method for Loading a specific Module and putting it into
         * the given container, with the given Node structure.
         */
        [DebuggerStepThrough]
        protected void LoadModule(string name, string container, Node node)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("You have to specify which Module you want to load");
            ActiveEvents.Instance.RaiseLoadControl(name, container, node);
        }

        /**
         * Level3: Shorthand for raising events. Will return a node, initially created empty, 
         * but passed onto the Event Handler(s)
         */
        [DebuggerStepThrough]
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
         * Level3: Shorthand for raising events.
         */
        [DebuggerStepThrough]
        protected void RaiseEvent(string eventName, Node node)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
        }

        /**
         * Level3: Shows a Message to the user with the given body
         */
        [DebuggerStepThrough]
        protected void ShowMessage(string body)
        {
            ShowMessage(body, null);
        }

        /**
         * Level3: Shows a Message to the user with the given body and header
         */
        [DebuggerStepThrough]
        protected void ShowMessage(string body, string header)
        {
            Node n = new Node();
            n["Message"].Value = body;

            if (header != null)
                n["Header"].Value = header;

            RaiseEvent(
                "Magix.Core.ShowMessage",
                n);
        }

        /**
         * Level3: Shows an Error Message to the user with the given body and header
         */
        [DebuggerStepThrough]
        protected void ShowError(string body, string header)
        {
            Node n = new Node();
            n["Message"].Value = body;

            if (header != null)
                n["Header"].Value = header;

            n["IsError"].Value = true;

            RaiseEvent(
                "Magix.Core.ShowMessage",
                n);
        }

        /**
         * Level3: Will include the given CSS file onto the page
         */
        [DebuggerStepThrough]
        protected void IncludeCssFile(string file)
        {
            Node node = new Node();
            node["CSSFile"].Value = file;

            RaiseEvent(
                "Magix.Core.AddCustomCssFile",
                node);
        }

        /**
         * Level3: Will return the 'base' URL of your application
         */
        protected string GetApplicationBaseUrl()
        {
            return string.Format(
                "{0}://{1}{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                (Page.Request.ApplicationPath.Equals("/")) ? 
                    "/" : 
                    HttpContext.Current.Request.ApplicationPath + "/").ToLowerInvariant();
        }

        /**
         * Level3: Shorthand for getting access to our "Page" object.
         */
        protected Page Page
        {
            get
            {
                return HttpContext.Current.Handler as Page;
            }
        }

        private Dictionary<string, object> _cache = new Dictionary<string, object>();
        /**
         * Level3: Will cache the result object for the remaining of the request. Highly
         * useful if you've got some objects or something which is expensive in retrieving, 
         * or are being for some resons retrieved several times per request. Use this one 
         * to make sure the 'retrieval method' is only ran ONCE per request, per controller.
         * Sorry, currently doesn't share cache between different controllers.
         */
        protected T Cache<T>(string key, executor<T> functor)
        {
            if (_cache.ContainsKey(key))
                return (T)_cache[key];

            T tmp = functor();

            _cache[key] = tmp;
            return tmp;
        }
    }
}
