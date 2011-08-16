/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
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
    /**
     * Level4: Baseclass for the 'Edit Objects' parts of the Grid system in Magix. Not meant
     * for directly consuming through this class
     */
    public abstract class Module : ActiveModule
    {
        protected abstract void ReDataBind();

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

        /**
         * Level4: Handled to make sure we update ours too, since object might be changed
         * some of its properties
         */
        [ActiveEvent(Name = "Magix.Core.UpdateGrids")]
        protected void Magix_Core_UpdateGrids(object sender, ActiveEventArgs e)
        {
            if (CheckForTypeHit(e))
            {
                ReDataBind();
            }
        }

        /**
         * Level4: Checking to see if it may be us, and if so, re-databind
         */
        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            if (CheckForTypeHit(e))
            {
                ReDataBind();
            }
        }

        /**
         * Level4: Handled to make sure we re-databind at the right spots
         */
        [ActiveEvent(Name = "RefreshWindowContent")]
        protected virtual void RefreshWindowContent(object sender, ActiveEventArgs e)
        {
            if (e.Params["ClientID"].Get<string>() == "LastWindow")
            {
                ReDataBind();
            }
            else
            {
                // TODO: OMG, I hate these parts ... :(
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
