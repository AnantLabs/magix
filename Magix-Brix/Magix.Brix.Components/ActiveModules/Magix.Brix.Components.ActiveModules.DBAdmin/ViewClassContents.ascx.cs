/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
        protected Panel previousPnl;
        protected Panel nextPnl;
        protected Panel createPnl;
        protected Panel endPnl;
        protected Panel beginningPnl;
        protected Panel root;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    create.Visible = node.Contains("IsCreate") && 
                        node["IsCreate"].Get<bool>();
                };
        }

        protected override System.Web.UI.Control Root
        {
            get { return root; }
        }

        protected void FirstItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = 0;
            DataSource["End"].Value = 
                Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
            ReDataBind(false);
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
            ReDataBind(false);
        }

        protected void NextItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value =
                    Math.Min(
                        DataSource["SetCount"].Get<int>() - 10,
                        DataSource["Start"].Get<int>() +
                            DataSource["Objects"].Count);
            DataSource["End"].Value =
                Math.Min(
                    DataSource["SetCount"].Get<int>(),
                    DataSource["Start"].Get<int>() +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
            ReDataBind(false);
        }

        protected void EndItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = Math.Max(0, DataSource["SetCount"].Get<int>() - 10);
            DataSource["End"].Value = DataSource["SetCount"].Get<int>();
            ReDataBind(false);
        }

        protected void CreateItem(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            RaiseSafeEvent(
                DataSource.Contains("CreateEventName") ? 
                    DataSource["CreateEventName"].Get<string>() :
                    "DBAdmin.Common.CreateObject",
                node);
            ReDataBind(true);
        }

        protected override System.Web.UI.Control TableParent
        {
            get { return pnl; }
        }

        protected override void DataBindDone()
        {
            if (Parent.Parent.Parent is Window)
            {
                (Parent.Parent.Parent as Window).Caption = string.Format(
                    "{0} {1}-{2}/{3}",
                    DataSource["TypeName"].Get<string>(),
                    ((int)DataSource["Start"].Value) + 1,
                    DataSource["End"].Get<int>(),
                    DataSource["SetCount"].Get<int>());
            }
            else
            {
                Node node = new Node();
                node["Caption"].Value = string.Format(
                    "{0} {1}-{2}/{3}",
                    DataSource["TypeName"].Get<string>(),
                    ((int)DataSource["Start"].Value) + 1,
                    DataSource["End"].Get<int>(),
                    DataSource["SetCount"].Get<int>());
                RaiseSafeEvent(
                    "Magix.Core.SetFormCaption",
                    node);
            }

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
            if (e.Params["ClientID"].Get<string>() == "LastWindow")
            {
                // Last window was closed, we're still around, what are we waiting for ...
                /// ... phreaking UPDATE ...!!! :P
                DataSource["Start"].Value = 0;
                DataSource["End"].Value =
                    DataSource["Start"].Get<int>(0) +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
                ReDataBind(false);
            }
            else
            {
                if (e.Params["ClientID"].Get<string>() == this.Parent.Parent.Parent.ClientID)
                {
                    DataSource["Start"].Value = 0;
                    DataSource["End"].Value =
                        DataSource["Start"].Get<int>(0) +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
                    ReDataBind(false);
                }
            }
        }

        protected override void ReDataBind()
        {
            ReDataBind(false);
        }

        private void ReDataBind(bool setSelectedRow)
        {
            ResetColumnsVisibility();
            DataSource["Objects"].UnTie();
            //DataSource["Type"].UnTie();
            if (DataSource["SetCount"].Get<int>() >= DataSource["End"].Get<int>() &&
                DataSource["End"].Get<int>() -
                DataSource["Start"].Get<int>() <
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10))
            {
                DataSource["End"].Value =
                    DataSource["Start"].Get<int>() +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
            }
            if (setSelectedRow)
            {
                // Now the logical thing to do, is to page to the end, since that's the likely
                // place we'll find this object within our grid ...
                if (DataSource["SetCount"].Get<int>() >=
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10))
                {
                    // We have more items in our SetCount, than we have space for at
                    // the time being within our Grid component ...
                    DataSource["End"].Value =
                        DataSource["SetCount"].Get<int>() + 1;
                    DataSource["Start"].Value =
                        DataSource["SetCount"].Get<int>() -
                        (Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10) - 1);
                }
            }
            if (RaiseSafeEvent(
                "DBAdmin.Data.GetContentsOfClass",
                DataSource))
            {
                if (setSelectedRow)
                {
                    // Findinge the object with the highest ID, since this
                    // is basically only being called from the "CreateNewObject"
                    // logic, and hence the only point where we want to update the
                    // selecetd item ...
                    int selectedID = -1;
                    foreach (Node idx in DataSource["Objects"])
                    {
                        if (idx.Contains("ID"))
                        {
                            int id = idx["ID"].Get<int>();
                            if (id > selectedID)
                            {
                                // We exploit the fact here that every single DB system in the world,
                                // using integers as ID's, will create larger and larger ID's for us,
                                // for every new object being created ...
                                selectedID = id;
                            }
                        }
                    }
                    SelectedID = selectedID;
                }

                pnl.Controls.Clear();
                DataBindGrid();
                pnl.ReRender();
            }
            FlashPanel(pnl);
        }
    }
}




















