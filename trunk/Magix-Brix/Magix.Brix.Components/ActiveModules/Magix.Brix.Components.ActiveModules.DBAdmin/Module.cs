﻿/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    public abstract class Module : UserControl, IModule
    {
        protected abstract void ReDataBind();

        public virtual void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }

        [ActiveEvent(Name = "RefreshWindowContent")]
        protected virtual void RefreshWindowContent(object sender, ActiveEventArgs e)
        {
            if (e.Params["ClientID"].Get<string>() == this.Parent.Parent.Parent.ClientID)
            {
                ReDataBind();
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
            catch(Exception err)
            {
                Node n = new Node();
                while (err.InnerException != null)
                    err = err.InnerException;
                n["Message"].Value = err.Message;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowMessage",
                    n);
                return false;
            }
        }
    }
}