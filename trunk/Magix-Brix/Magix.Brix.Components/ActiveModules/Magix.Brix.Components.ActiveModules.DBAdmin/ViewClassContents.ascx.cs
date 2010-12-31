/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
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
        protected Window wnd;
        protected DynamicPanel child;

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

        protected void PreviousItems(object sender, EventArgs e)
        {
            if (Start > 0)
            {
                Node node = new Node();
                node["Start"].Value = Start - Count;
                RaiseForwardRewindEvent(node);
            }
        }

        private void RaiseForwardRewindEvent(Node node)
        {
            node["FullTypeName"].Value = FullTypeName;
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

        protected void NextItems(object sender, EventArgs e)
        {
            if (Start + Count < TotalCount)
            {
                Node node = new Node();
                node["Start"].Value = Start + Count;
                RaiseForwardRewindEvent(node);
            }
        }

        protected override void ReDataBind()
        {
            Node node = new Node();
            node["Start"].Value = Start;
            RaiseForwardRewindEvent(node);
        }

        protected override void DataBindObjects()
        {
            HtmlTable table = CreateTable();
            if (table != null)
            {
                HtmlTableRow row = CreateHeader(table);
                table.Rows.Add(row);
                CreateCells(table);
                pnl.Controls.Add(table);
            }
        }
    }
}




















