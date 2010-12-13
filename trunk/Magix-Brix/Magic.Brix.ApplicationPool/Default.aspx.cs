/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Configuration;
using System.Web.UI;
using Magix.Brix.Loader;
using Magix.Brix.Data;
using System.Text;
using System.IO;

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
            string defaultControl = ConfigurationManager.AppSettings["PortalViewDriver"];
            Control ctrl = PluginLoader.Instance.LoadControl(defaultControl);
            Form.Controls.Add(ctrl);
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