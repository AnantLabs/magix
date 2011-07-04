/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using Magix.UX.Effects;

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

        protected bool CheckForTypeHit(ActiveEventArgs e)
        {
            if (DataSource.Contains("FullTypeName") && 
                !string.IsNullOrEmpty(DataSource["FullTypeName"].Get<string>()))
            {
                if (e.Params["FullTypeName"].Get<string>().Contains(DataSource["FullTypeName"].Get<string>()))
                    return true;
                if (DataSource.Contains("CoExistsWith") &&
                    DataSource["CoExistsWith"].Get<string>().Contains(e.Params["FullTypeName"].Get<string>()))
                    return true;
            }
            return false;
        }

        [ActiveEvent(Name = "Magix.Core.UpdateGrids")]
        protected void Magix_Core_UpdateGrids(object sender, ActiveEventArgs e)
        {
            if (CheckForTypeHit(e))
            {
                ReDataBind();
            }
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            if (CheckForTypeHit(e))
            {
                ReDataBind();
            }
        }

        [ActiveEvent(Name = "RefreshWindowContent")]
        protected virtual void RefreshWindowContent(object sender, ActiveEventArgs e)
        {
            if (e.Params["ClientID"].Get<string>() == "LastWindow")
            {
                ReDataBind();
            }
            else
            {
                if (e.Params["ClientID"].Get<string>() == this.Parent.Parent.Parent.ClientID)
                {
                    ReDataBind();
                }
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
                    "Magix.Core.ShowMessage",
                    n);
                return false;
            }
        }

        protected void FlashPanel(Panel pnl)
        {
            new EffectHighlight(pnl, 500)
                .Render();
        }
    }
}
