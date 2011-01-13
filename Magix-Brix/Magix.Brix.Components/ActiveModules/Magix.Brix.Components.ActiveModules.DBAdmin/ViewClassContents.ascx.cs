/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewClassContents : ListModule, IModule
    {
        protected Panel pnl;
        protected Button previous;
        protected Button next;
        protected Button create;
        protected Button end;
        protected Button beginning;
        protected Button focs;
        protected Panel previousPnl;
        protected Panel nextPnl;
        protected Panel createPnl;
        protected Panel endPnl;
        protected Panel beginningPnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(focs))
                        .Render();
                };
        }

        protected void FirstItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = 0;
            DataSource["End"].Value = 
                Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
            ReDataBind();
        }

        protected void PreviousItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = 
                Math.Max(
                    0, 
                    DataSource["Start"].Get<int>() - 
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
            DataSource["End"].Value =
                Math.Min(
                    DataSource["SetCount"].Get<int>(), 
                    DataSource["Start"].Get<int>() + 
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
            ReDataBind();
        }

        protected void NextItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value =
                    Math.Min(
                        DataSource["SetCount"].Get<int>() - 1,
                        DataSource["Start"].Get<int>() +
                            DataSource["Objects"].Count);
            DataSource["End"].Value =
                Math.Min(
                    DataSource["SetCount"].Get<int>(),
                    DataSource["Start"].Get<int>() +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
            ReDataBind();
        }

        protected void EndItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = Math.Max(0, DataSource["SetCount"].Get<int>() - 10);
            DataSource["End"].Value = DataSource["SetCount"].Get<int>();
            ReDataBind();
        }

        protected void CreateItem(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            RaiseSafeEvent(
                "DBAdmin.Common.CreateObject",
                node);
        }

        protected override System.Web.UI.Control TableParent
        {
            get { return pnl; }
        }

        protected override void DataBindDone()
        {
            (Parent.Parent.Parent as Window).Caption = string.Format(
                "{0} {1}-{2}/{3}",
                DataSource["TypeName"].Get<string>(),
                ((int)DataSource["Start"].Value) + 1,
                DataSource["End"].Get<int>(),
                DataSource["SetCount"].Get<int>());

            previousPnl.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
            nextPnl.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);

            previous.Enabled = DataSource["Start"].Get<int>() > 0;
            next.Enabled = DataSource["End"].Get<int>() < DataSource["SetCount"].Get<int>();

            beginningPnl.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10) * 2;
            endPnl.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10) * 2;

            beginning.Enabled = DataSource["Start"].Get<int>() > 0;
            end.Enabled = DataSource["End"].Get<int>() < DataSource["SetCount"].Get<int>();
        }

        protected override void RefreshWindowContent(object sender, ActiveEventArgs e)
        {
            if (e.Params["ClientID"].Get<string>() == this.Parent.Parent.Parent.ClientID)
            {
                DataSource["Start"].Value = 0;
                DataSource["End"].Value =
                    DataSource["Start"].Get<int>(0) +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
                ReDataBind();
            }
        }

        protected override void ReDataBind()
        {
            focs.Focus();
            ResetColumnsVisibility();
            DataSource["Objects"].UnTie();
            DataSource["Type"].UnTie();
            if (RaiseSafeEvent(
                "DBAdmin.Data.GetContentsOfClass",
                DataSource))
            {
                pnl.Controls.Clear();
                DataBindGrid();
                pnl.ReRender();
            }
            FlashPanel(pnl);
        }
    }
}




















