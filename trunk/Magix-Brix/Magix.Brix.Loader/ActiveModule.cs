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
using System.Threading;

namespace Magix.Brix.Loader
{
    /**
     * Level3: Helper class for simplifying some of the common tasks you'd normally
     * want to use from your Modules, such as RaisingEvents etc. Inherit your ActiveModules
     * from this class to simplify their usage
     */
    public abstract class ActiveModule : UserControl, IModule
    {
        /**
         * Level3: Helper for executing 'dangerous code' such that if an exception happens,
         * it'll 'swallow' the exception, and show a Message box showing the exception
         */
        protected delegate void executor();

        private bool _firstLoad;

        /**
         * Level3: You _Must_ call this one, if you override it, to set the Module's DataSource property. 
         * And in order to get some other types of wiring running correctly for the module
         */
        public virtual void InitialLoading(Node node)
        {
            _firstLoad = true;

            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        /**
         * Level3: Is true if this was the request when the Module was loaded initially, somehow
         */
        protected bool FirstLoad
        {
            get { return _firstLoad; }
        }

        /**
         * Level3: The Node passed into InitialLoading will automatically be stored here ...
         */
        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }

        /**
         * Level3: Shorthand for raising events. Will return a node, initially created empty, 
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
         * Level3: Shorthand for raising events
         */
        protected void RaiseEvent(string eventName, Node node)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
        }

        /**
         * Level3: Shorthand for raising events. Will return a node, initially created empty, 
         * but passed onto the Event Handler(s). This method will trap any exceptions occuring,
         * and show a message box back to the end user with its exception content, if any
         */
        [DebuggerStepThrough]
        public Node RaiseSafeEvent(string eventName)
        {
            Node node = new Node();
            RaiseSafeEvent(eventName, node);
            return node;
        }

        /**
         * Level3: Shorthand for raising events. Will return a node, initially created empty, 
         * but passed onto the Event Handler(s). This method will trap any exceptions occuring,
         * and show a message box back to the end user with its exception content, if any
         */
        [DebuggerStepThrough]
        protected bool RaiseSafeEvent(string eventName, Node node)
        {
            try
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    eventName,
                    node);
                return true;
            }
            catch (ThreadAbortException)
            {
                ; // ASP.NET throws this exception upon 'Response.Redirect' calls
                // Hence we just sliently let it pass, since it's highly likely a redirect
                // and not something we'd need to display in a message box to the user ...
                throw;
            }
            catch (Exception err)
            {
                Exception tmp = err.GetBaseException();

                Node m = new Node();

                m["Message"].Value = 
                    "<p>" + tmp.Message + "</p>" + 
                    "<p class='mux-err-stack-trace'>" + tmp.StackTrace + "</p>";

                m["Milliseconds"].Value = 10000;
                m["IsError"].Value = true;

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    m);
            }
            return false;
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
         * Level3: Shows a Message to the user with the given body and header and Milliseconds
         */
        [DebuggerStepThrough]
        protected void ShowMessage(string body, string header, int milliseconds)
        {
            Node n = new Node();

            n["Message"].Value = body;
            n["Milliseconds"].Value = milliseconds;

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
         * Level3: Will include the given CSS file onto the page. Useful for injecting your
         * own CSS files onto the page
         */
        protected void IncludeCssFile(string file)
        {
            Node node = new Node();
            node["CSSFile"].Value = file;

            RaiseEvent(
                "Magix.Core.AddCustomCssFile",
                node);
        }

        /**
         * Level3: Will return the 'base' URL of your application. Meaning if your application
         * is installed on x.com/f then x.com/f will always be returned from this method.
         * Useful for using as foundation for finding specific files and so on
         */
        protected string GetApplicationBaseUrl()
        {
            return string.Format(
                "{0}://{1}{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                (Page.Request.ApplicationPath.Equals("/")) ? 
                    "/" : 
                    HttpContext.Current.Request.ApplicationPath + "/")
                        .Replace("Default.aspx", "").Replace("default.aspx", "");
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
                Exception tmp = err.GetBaseException();

                Node node = new Node();

                node["Message"].Value =
                    "<p>" + string.Format(msg, args) + "</p>" +
                    "<p>Message from Server; </p>" +
                    "<p>" + tmp.Message + "</p>" +
                    "<p>Stack Trace; </p>" +
                    "<p class='mux-err-stack-trace'>" + tmp.StackTrace + "</p>";

                node["Header"].Value = tmp.GetType().FullName;
                node["IsError"].Value = true;

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
            }
            return false;
        }
    }
}
