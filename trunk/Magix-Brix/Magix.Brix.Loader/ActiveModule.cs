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
     * want to use from your Modules, such as RaisingEvents etc.
     */
    public abstract class ActiveModule : UserControl, IModule
    {
        public virtual void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        /**
         * The Node passed into InitialLoading will automatically be stored here ...
         */
        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
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
         * Shorthand for raising events. Will return a node, initially created empty, 
         * but passed onto the Event Handler(s)
         */
        protected Node RaiseSaveEvent(string eventName)
        {
            Node node = new Node();
            RaiseSafeEvent(eventName, node);
            return node;
        }

        /**
         * Shorthand for raising events.
         */
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
            catch (Exception err)
            {
                Exception tmp = err;
                while (tmp.InnerException != null)
                    tmp = tmp.InnerException;

                Node m = new Node();
                
                m["Message"].Value = tmp.Message;
                
                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    m);
                return false;
            }
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
                    HttpContext.Current.Request.ApplicationPath + "/")
                        .Replace("Default.aspx", "").Replace("default.aspx", "");
        }
    }
}
