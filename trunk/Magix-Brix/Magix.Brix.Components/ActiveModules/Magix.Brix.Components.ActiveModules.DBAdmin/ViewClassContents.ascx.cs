/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: Will show all objects of type 'FullTypeName' in a Grid from which the user can filter,
     * edit, change values, delete objects, create new objects and such from. Basically the 
     * 'show single table' logic of the Magix' 'Database Enterprise Manager'. If 'IsCreate',
     * will allow for creation of objects of the 'FullTypeName' by the click of a button
     */
    [ActiveModule]
    public class ViewClassContents : ListModule, IModule
    {
        protected Panel pnl;
        protected Button create;
        protected Button previous;
        protected Button next;
        protected Button end;
        protected Button beginning;
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
                Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);

            ReDataBind(false);
        }

        protected void PreviousItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = 
                Math.Max(
                    0, 
                    DataSource["Start"].Get<int>() -
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10));

            DataSource["End"].Value =
                Math.Min(
                    DataSource["SetCount"].Get<int>(), 
                    DataSource["Start"].Get<int>() +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10));

            ReDataBind(false);
        }

        protected void NextItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value =
                    Math.Min(
                        DataSource["SetCount"].Get<int>() - Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10),
                        DataSource["Start"].Get<int>() +
                            DataSource["Objects"].Count);

            DataSource["End"].Value =
                Math.Min(
                    DataSource["SetCount"].Get<int>(),
                    DataSource["Start"].Get<int>() +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10));

            ReDataBind(false);
        }

        protected void EndItems(object sender, EventArgs e)
        {
            DataSource["Start"].Value = Math.Max(0, DataSource["SetCount"].Get<int>() - Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10));
            DataSource["End"].Value = DataSource["SetCount"].Get<int>();
            ReDataBind(false);
        }

        protected void CreateItem(object sender, EventArgs e)
        {
            RaiseSafeEvent(
                DataSource.Contains("CreateEventName") ? 
                    DataSource["CreateEventName"].Get<string>() :
                    "DBAdmin.Common.CreateObject",
                DataSource);

            ReDataBind(false);
        }

        protected override System.Web.UI.Control TableParent
        {
            get { return pnl; }
        }

        protected override void DataBindDone()
        {
            // TODO: MAJORLY REFACTOR ... !! IDIOT LOGIC ...!!
            if (Parent.Parent.Parent is Window)
            {
                (Parent.Parent.Parent as Window).Caption =
                    DataSource.Contains("Header") ?
                        DataSource["Header"].Get<string>() :
                        string.Format(
                        "{0} {1}-{2}/{3}",
                        DataSource["TypeName"].Get<string>(),
                        ((int)DataSource["Start"].Value) + 1,
                        DataSource["End"].Get<int>(),
                        DataSource["SetCount"].Get<int>());
            }
            else
            {
                //Node node = new Node();

                //node["Caption"].Value =
                //    DataSource.Contains("Header") ?
                //        DataSource["Header"].Get<string>() :
                //        string.Format(
                //        "{0} {1}-{2}/{3}",
                //        DataSource["TypeName"].Get<string>(),
                //        ((int)DataSource["Start"].Value) + 1,
                //        DataSource["End"].Get<int>(),
                //        DataSource["SetCount"].Get<int>());

                //RaiseSafeEvent(
                //    "Magix.Core.SetFormCaption",
                //    node);
            }

            previous.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);
            next.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);

            previous.Enabled = DataSource["Start"].Get<int>() > 0;
            next.Enabled = DataSource["End"].Get<int>() < DataSource["SetCount"].Get<int>();

            beginning.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10) * 2;
            end.Visible = DataSource["SetCount"].Get<int>() > Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10) * 2;

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
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);

                ReDataBind(false);
            }
            else
            {
                if (e.Params["ClientID"].Get<string>() == this.Parent.Parent.Parent.ClientID)
                {
                    DataSource["Start"].Value = 0;

                    DataSource["End"].Value =
                        DataSource["Start"].Get<int>(0) +
                        Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);

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

            if (DataSource["SetCount"].Get<int>() >= DataSource["End"].Get<int>() &&
                DataSource["End"].Get<int>() -
                DataSource["Start"].Get<int>() <
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10))
            {
                DataSource["End"].Value =
                    DataSource["Start"].Get<int>() +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);
            }
            if (RaiseSafeEvent(
                DataSource.Contains("GetObjectsEvent") ?
                    DataSource["GetObjectsEvent"].Get<string>() :
                    "DBAdmin.Data.GetContentsOfClass",
                DataSource))
            {
                if (setSelectedRow)
                {
                    // Findinge the object with the highest ID, since this
                    // is basically only being called from the "CreateNewObject"
                    // logic, and hence the only point where we want to update the
                    // selecetd item ...
                    string selectedID = "0";
                    foreach (Node idx in DataSource["Objects"])
                    {
                        if (idx.Contains("ID"))
                        {
                            string id = idx["ID"].Value.ToString();
                            if (id.CompareTo(selectedID) > 0)
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
