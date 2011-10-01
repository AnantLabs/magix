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
using Magix.UX;

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

        protected string _guid;

        /**
         * Level4: Checking to see if it may be us, and if so, re-databind
         */
        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            if (CheckForTypeHit(e))
            {
                if (e.Params.Contains("_guid") && e.Params["_guid"].Get<string>() == _guid)
                    return; // 'This' was the source, and we don't need to update ...
                ReDataBind();
            }
        }

        // TODO: Refactor ...!
        /**
         * Level4: Handled to make sure we re-databind at the right spots. This event is raised
         * whenever a child window is closed, and other windows might need to refresh. We're
         * making sure its either a 'ClientID' match or the incoming 'ClientID' is 'LastWindow'
         * before we do our update here. 'LastWindow' is true if it's the 'last popup window'
         */
        [ActiveEvent(Name = "Magix.Core.RefreshWindowContent")]
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

                    if (e.Params.Contains("ReFocus") &&
                        e.Params["ReFocus"].Get<bool>())
                    {
                        new EffectTimeout(250)
                            .ChainThese(
                                new EffectFocusAndSelect(Selector.SelectFirst<LinkButton>(this.Parent.Parent.Parent)))
                            .Render();
                    }
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
