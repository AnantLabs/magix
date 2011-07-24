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
using System.Diagnostics;

namespace Magix.Brix.Components
{
    public abstract class Module : ActiveModule, IModule
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

        protected void FlashPanel(Panel pnl)
        {
            new EffectHighlight(pnl, 500)
                .Render();
        }
    }
}
