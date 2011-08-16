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

namespace Magix.Brix.ApplicationPool
{
    /**
     * Level1: Your 'Application Pool', meaning the 'world' where all your 'components' lives.
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

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Brix.Core.Page_Init");

            if (!IsPostBack)
            {
                Node node = new Node();

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.InitialLoading",
                    node);
            }
            LoadComplete += MainWebPage_LoadComplete;
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Brix.Core.LoadComplete_InitialLoading");
            }
            Form.Action = Request.Url.ToString();

            if (!AjaxManager.Instance.IsCallback)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.PostBackOrLoad");
            }
        }

        private void InitializeViewport()
        {
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            if (string.IsNullOrEmpty(defaultControl))
            {
                Node node = new Node();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
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