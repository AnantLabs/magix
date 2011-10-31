/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    /**
     * Level2: Basically identical to ViewClassContents, with one addition. This module will show a 'filter field'
     * to the end user which the user can use to 'search for items' from within.
     */
    [ActiveModule]
    public class FindObject : ListModule, IModule
    {
        protected Panel pnlWrp;
        protected Panel pnl;
        protected Button previous;
        protected Button create;
        protected Button next;
        protected Button beginning;
        protected Button end;
        protected Panel root;
        protected TextBox query;
        protected Label lblCount;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    create.Visible = node.Contains("IsCreate") &&
                        node["IsCreate"].Get<bool>();
                    if (node.Contains("ChildCssClass"))
                        pnlWrp.CssClass = node["ChildCssClass"].Get<string>();
                    if (node.Contains("SetFocus") && 
                        node["SetFocus"].Get<bool>())
                    {
                        // Defaulting ....
                        new EffectTimeout(250)
                            .ChainThese(
                                new EffectFocusAndSelect(query))
                            .Render();
                    }
                    if (node.Contains("QueryText") && 
                        !string.IsNullOrEmpty(node["QueryText"].Get<string>()))
                    {
                        query.Text = node["QueryText"].Get<string>();
                    }
                };
        }

        protected override System.Web.UI.Control Root
        {
            get { return root; }
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            DataSource["Filter"].Value = query.Text;

            DataSource["Start"].Value = 0;

            DataSource["End"].Value =
                DataSource["Start"].Get<int>(0) +
                Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10);

            ReDataBind(false);

            query.Select();
            query.Focus();
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
                        DataSource["SetCount"].Get<int>() -
                            Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10),
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

            if (DataSource.Contains("SetCount"))
                lblCount.Text = "#" + DataSource["SetCount"].Value.ToString();
            else
                lblCount.Text = "?";

            UpdateCounter();
        }

        private void UpdateCounter()
        {
            if (DataSource.Contains("SetCount"))
                lblCount.Text = (DataSource["Start"].Get<int>() + 1).ToString() +
                    " / " +
                    DataSource["End"].Get<int>().ToString() +
                    " - " +
                    DataSource["SetCount"].Value.ToString();
            else
                lblCount.Text = "?";
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
            if (setSelectedRow)
            {
                // Now the logical thing to do, is to page to the end, since that's the likely
                // place we'll find this object within our grid ...
                if (DataSource["SetCount"].Get<int>() >=
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10))
                {
                    // We have more items in our SetCount, than we have space for at
                    // the time being within our Grid component ...
                    DataSource["End"].Value =
                        DataSource["SetCount"].Get<int>() + 1;

                    DataSource["Start"].Value =
                        DataSource["SetCount"].Get<int>() -
                        (Settings.Instance.Get("DBAdmin.MaxItemsToShow-" + DataSource["FullTypeName"].Get<string>(), 10) - 1);
                }
            }
            if (RaiseSafeEvent(
                (DataSource["GetContentsEventName"].Get<string>() 
                ?? "DBAdmin.Data.GetContentsOfClass"),
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
