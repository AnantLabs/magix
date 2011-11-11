/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Configuration;
using System.Web.UI;
using System.Text;
using System.IO;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Data;
using Magix.UX;
using System.Web;
using System.Threading;

namespace Magix.Brix.ApplicationPool
{
    /**
     * Level4: Your 'Application Pool', meaning the 'world' where all your 'components' lives.
     * Nothing really to see here, this should just 'work'. But are here for reference reasons
     */
    public partial class MainWebPage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            HttpContext.Current.RewritePath(
                HttpContext.Current.Request.ApplicationPath.ToLowerInvariant() + "/", false);
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeViewport();

            Node node = new Node();

            RaiseSafeEvent("Brix.Core.Page_Init", node);

            if (!IsPostBack)
            {
                node = new Node();

                RaiseSafeEvent(
                    "Magix.Core.InitialLoading",
                    node);
            }
            LoadComplete += MainWebPage_LoadComplete;
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RaiseSafeEvent("Brix.Core.LoadComplete_InitialLoading", new Node());
            }
            Form.Action = Request.Url.ToString();

            if (!AjaxManager.Instance.IsCallback)
            {
                RaiseSafeEvent(
                    "Magix.Core.PostBackOrLoad",
                    new Node ());
            }
            if (!AjaxManager.Instance.IsCallback && 
                Request.HttpMethod == "POST" &&
                !string.IsNullOrEmpty(Page.Request["event"]))
            {
                RaiseSafeEvent(
                    "Magix.Core.PostHTTPRequest",
                    new Node());
            }

            if (!AjaxManager.Instance.IsCallback)
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format(@"<base href=""{0}"" />",
                    Request.Url.Scheme + "://" +
                    Request.Url.Authority +
                    Request.ApplicationPath.ToLowerInvariant() + "/");
                Header.Controls.Add(lit);
            }
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            if (string.IsNullOrEmpty(defaultControl))
            {
                Node node = new Node();

                RaiseSafeEvent(
                    "Magix.Core.GetViewPort",
                    node);

                Form.Controls.Add(node["Control"].Get<Control>());
            }
            else
            {
                Control ctrl = PluginLoader.Instance.LoadActiveModule(defaultControl);
                Form.Controls.Add(ctrl);
            }
        }

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
                Exception tmp = err;

                while (tmp.InnerException != null)
                    tmp = tmp.InnerException;

                Node m = new Node();

                m["Message"].Value =
                    "<p>" + tmp.Message + "</p>" +
                    "<p class='mux-err-stack-trace'>" + err.StackTrace + "</p>";

                m["Milliseconds"].Value = 10000;
                m["IsError"].Value = true;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    m);
            }
            return false;
        }

        private PageStatePersister _pageStatePersister;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (Magix.Brix.Data.Adapter.Instance is IPersistViewState)
                {
                    if (_pageStatePersister == null)
                        _pageStatePersister = new Magix_PageStatePersister(this);
                    return _pageStatePersister;
                }
                else
                    return base.PageStatePersister;
            }
        }
    }
}