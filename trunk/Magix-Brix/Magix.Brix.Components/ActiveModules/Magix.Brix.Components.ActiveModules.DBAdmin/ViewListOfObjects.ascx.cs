/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewListOfObjects : DataBindableListControl, IModule
    {
        protected Panel pnl;
        protected Button append;
        protected Button previous;
        protected Button next;

        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    UpdateCaption();
                    SetButtonText();
                    append.Enabled = node["IsAppend"].Get<bool>();
                };
        }

        protected void append_Click(object sender, EventArgs e)
        {
            try
            {
                Node node = new Node();
                node["ParentID"].Value = ParentID;
                node["ParentPropertyName"].Value = ParentPropertyName;
                node["ParentType"].Value = ParentFullType;
                node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "DBAdmin.AppendComplexInstance",
                    node);
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
            try
            {
                DataSource["Objects"].UnTie();
                Node node = DataSource;
                List<string> keysToRemove = new List<string>();
                foreach (string idxKey in ViewState.Keys)
                {
                    if (idxKey.IndexOf("DBAdmin.VisibleColumns.") == 0)
                        keysToRemove.Add(idxKey);
                }
                foreach (string idxKey in keysToRemove)
                {
                    ViewState.Remove(idxKey);
                }
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "DBAdmin.UpdateList",
                    node);
                pnl.Controls.Clear();
                UpdateCaption();
                DataBindObjects();
                pnl.ReRender();
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

        protected void PreviousItems(object sender, EventArgs e)
        {
            if (Start > 0)
            {
                Node node = DataSource;
                node["Start"].Value = Math.Max(0, Start - Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10));
                node["End"].Value = Math.Min(
                    node["Start"].Get<int>() + Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10),
                    TotalCount - node["Start"].Get<int>());
                node["Objects"].UnTie();
                node["Type"].UnTie();
                RaiseForwardRewindEvent(node);
            }
        }

        protected void NextItems(object sender, EventArgs e)
        {
            if (Start + Count < TotalCount)
            {
                Node node = DataSource;
                node["Start"].Value = Start + Count;
                node["End"].Value = node["Start"].Get<int>() +
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
                node["Objects"].UnTie();
                node["Type"].UnTie();
                RaiseForwardRewindEvent(node);
            }
        }

        private void SetButtonText()
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

        private void RaiseForwardRewindEvent(Node node)
        {
            try
            {
                if (!node.Contains("Start"))
                {
                    node["Start"].Value = 0;
                    node["End"].Value = Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10);
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
                new EffectScrollBrowser(250)
                    .Render();
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

        protected override void UpdateCaption()
        {
            (Parent.Parent.Parent as Window).Caption = string.Format(
@"{0}-{1}/{2} of {3} belonging to {4}({5}) on {6}",
                Start,
                End,
                TotalCount,
                TypeName,
                ParentType,
                ParentID,
                ParentPropertyName);
        }
    }
}




















