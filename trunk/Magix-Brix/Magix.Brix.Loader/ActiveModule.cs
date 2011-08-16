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

namespace Magix.Brix.Loader
{
    /**
     * Level3: Helper class for simplifying some of the common tasks you'd normally
     * want to use from your Modules, such as RaisingEvents etc. Inherit your ActiveModules
     * from this class to simplify their usage
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
            catch (Exception err)
            {
                Exception tmp = err;

                while (tmp.InnerException != null)
                    tmp = tmp.InnerException;

                Node m = new Node();
                
                m["Message"].Value = tmp.Message;
                m["Milliseconds"].Value = 10000;
                m["IsError"].Value = true;
                
                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    m);
            }
            return false;
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
    }
}
