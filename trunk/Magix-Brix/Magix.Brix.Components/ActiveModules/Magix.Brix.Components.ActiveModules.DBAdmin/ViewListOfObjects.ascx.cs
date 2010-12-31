/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewListOfObjects : DataBindableListControl, IModule
    {
        protected Panel pnl;
        protected Button append;

        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    UpdateCaption();
                    append.Enabled = node["IsAppend"].Get<bool>();
                };
        }

        protected void append_Click(object sender, EventArgs e)
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

        protected override void ReDataBind()
        {
            DataSource["Objects"].UnTie();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.UpdateList",
                DataSource);
            pnl.Controls.Clear();
            UpdateCaption();
            DataBindObjects();
            pnl.ReRender();
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




















