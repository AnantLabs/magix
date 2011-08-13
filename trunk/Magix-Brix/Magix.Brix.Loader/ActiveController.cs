/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.Web;
using Magix.Brix.Types;
using System.Diagnostics;

namespace Magix.Brix.Loader
{
    /**
     * Helper class for simplifying some of the common tasks you'd normally
     * want to use from your controllers, such as Loading Modules etc.
     */
    public abstract class ActiveController
    {
        /**
         * Helper 
         */
        protected delegate void executor();

        /**
         * Loads the given module and puts it into your default container.
         */
        protected Node LoadModule(string name)
        {
            Node node = new Node();
            LoadModule(name, null, node);
            return node;
        }

        protected void ExecuteSafely(executor functor, string msg, params object[] args)
        {
            try
            {
                functor();
            }
            catch (Exception err)
            {
                while (err.InnerException != null)
                    err = err.InnerException;

                Node node = new Node();
                node["Message"].Value = 
                    "<p>" + string.Format(msg, args) + "</p>" + 
                    "<p>Message from Server; </p>" + 
                    "<p>" + err.Message + "</p>";
                node["Header"].Value = err.GetType().FullName;
                node["IsError"].Value = true;

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
            }
        }

        /**
         * Loads the given module and puts it into the given container.
         */
        protected Node LoadModule(string name, string container)
        {
            Node node = new Node();
            LoadModule(name, container, node);
            return node;
        }

        /**
         * Shorthand method for Loading a specific Module and putting it into
         * the given container, with the given Node structure.
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
        [DebuggerStepThrough]
        protected void RaiseEvent(string eventName, Node node)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                eventName,
                node);
        }

        /**
         * Shows a Message to the user with the given body
         */
        protected void ShowMessage(string body)
        {
            ShowMessage(body, null);
        }

        /**
         * Shows a Message to the user with the given body and header
         */
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
         * Shows an Error Message to the user with the given body and header
         */
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
         * Will include the given CSS file onto the page.
         */
        protected void IncludeCssFile(string file)
        {
            Node node = new Node();
            node["CSSFile"].Value = file;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.AddCustomCssFile",
                node);
        }

        /**
         * Will return the 'base' URL of your application.
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
