/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Configuration;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.ApplicationPool
{
    public partial class MainWebPage : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeViewport();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Page_Init");
            if (!IsPostBack)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "Page_Init_InitialLoading");
            }
            LoadComplete += MainWebPage_LoadComplete;
        }

        void MainWebPage_LoadComplete(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Page_LoadComplete_InitialLoading");
        }

        private void InitializeViewport()
        {
            string defaultControl =
                ConfigurationManager.AppSettings["PortalViewDriver"];
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
                Control ctrl =
                    PluginLoader.Instance.LoadActiveModule(defaultControl);
                Form.Controls.Add(ctrl);
            }
        }

        private PageStatePersister _pageStatePersister;
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (Magix.Brix.Data.Internal.Adapter.Instance is IPersistViewState)
                {
                    if (_pageStatePersister == null)
                        _pageStatePersister = new RaBrixPageStatePersister(this);
                    return _pageStatePersister;
                }
                else
                    return base.PageStatePersister;
            }
        }
    }
}