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
    public class ViewClassContents : DataBindableListControl, IModule
    {
        protected Panel pnl;
        protected Button previous;
        protected Button next;
        protected Button create;

        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    SetButtonText();
                    UpdateCaption();
                };
        }

        private void SetButtonText()
        {
            create.Enabled = DataSource["IsCreate"].Get<bool>();
            bool filter = 
                !string.IsNullOrEmpty(Settings.Instance.Get(FullTypeName + ":ID", ""));

            if (filter)
            {
                // Filter on ID, and hence always showing EVERYTHING ...!
                previous.Enabled = false;
                next.Enabled = false;
                next.Text = "Next";
                previous.Text = "Previous";
            }
            else
            {
                previous.Enabled = Start > 0;
                previous.Text =
                    previous.Enabled ?
                    string.Format(
                        "Previous {0} items",
                        Math.Min(
                            MaxItems,
                            Start)) :
                    "Previous";
                next.Enabled = Start + Count < TotalCount;
                next.Text =
                    next.Enabled ?
                        string.Format(
                            "Next {0} items",
                            Math.Min(
                                MaxItems,
                                TotalCount - (Start + Count))) :
                            "Next";
            }
        }

        protected void PreviousItems(object sender, EventArgs e)
        {
            if (Start > 0)
            {
                Node node = DataSource;
                node["Start"].Value = Start - Count;
                node["End"].Value = node["Start"].Get<int>() +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 20);
                node["Objects"].UnTie();
                node["Type"].UnTie();
                RaiseForwardRewindEvent(node);
            }
        }

        protected void CreateItem(object sender, EventArgs e)
        {
            Node node = DataSource;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.CreateNewInstance",
                node);
            DataSource["Start"].UnTie();
            DataSource["End"].UnTie();
            ReDataBind();
        }

        protected void NextItems(object sender, EventArgs e)
        {
            if (Start + Count < TotalCount)
            {
                Node node = DataSource;
                node["Start"].Value = Start + Count;
                node["End"].Value = node["Start"].Get<int>() +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 20);
                node["Objects"].UnTie();
                node["Type"].UnTie();
                RaiseForwardRewindEvent(node);
            }
        }

        private void RaiseForwardRewindEvent(Node node)
        {
            try
            {
                if (!node.Contains("Start"))
                {
                    node["Start"].Value = 0;
                    node["End"].Value = Settings.Instance.Get("DBAdmin.MaxItemsToShow", 20);
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "DBAdmin.UpdateContents",
                    node);
                pnl.Controls.Clear();
                DataSource = node;
                DataBindObjects();
                pnl.ReRender();
                SetButtonText();
                UpdateCaption();

            }
            catch (Exception err)
            {
                Node node2 = new Node();
                while (err.InnerException != null)
                    err = err.InnerException;
                node2["Message"].Value = err.Message;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowMessage",
                    node2);
            }
        }

        protected override void ReDataBind()
        {
            Node tmp = DataSource;
            List<string> keysToRemove = new List<string>();
            foreach (string idxKey in ViewState.Keys)
            {
                if (idxKey.IndexOf("DBAdmin.VisibleColumns.") != -1)
                    keysToRemove.Add(idxKey);
            }
            foreach (string idxKey in keysToRemove)
            {
                ViewState.Remove(idxKey);
            }
            DataSource = tmp;
            DataSource["Objects"].UnTie();
            DataSource["Type"].UnTie();
            RaiseForwardRewindEvent(DataSource);
        }

        protected override void DataBindObjects()
        {
            System.Web.UI.Control table = CreateTable();
            if (table != null)
            {
                System.Web.UI.Control row = CreateHeader(table);
                table.Controls.Add(row);
                CreateCells(table);
                pnl.Controls.Add(table);
            }
        }
    }
}




















