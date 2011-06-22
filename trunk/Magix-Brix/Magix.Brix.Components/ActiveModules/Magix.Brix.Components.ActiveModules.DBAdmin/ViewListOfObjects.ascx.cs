/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
    public class ViewListOfObjects : ListModule, IModule
    {
        protected Panel pnl;
        protected Button append;
        protected Button previous;
        protected Button next;
        protected Panel appendPnl;
        protected Panel previousPnl;
        protected Panel nextPnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    appendPnl.Visible = DataSource["IsAppend"].Get<bool>();
                };
        }

        protected void append_Click(object sender, EventArgs e)
        {
            if (DataSource["IsDelete"].Get<bool>())
            {
                Node node = new Node();
                node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                node["ParentID"].Value = DataSource["ParentID"].Value;
                node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
                node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;
                RaiseSafeEvent(
                    "DBAdmin.Common.CreateObjectAsChild",
                    node);
                ReDataBind();
            }
            else
            {
                // Defaulting to Append Logic ...
                Node node = new Node();
                node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                node["ParentID"].Value = DataSource["ParentID"].Value;
                node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
                node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;
                RaiseSafeEvent(
                    "DBAdmin.Form.AppendObject",
                    node);
            }
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

        protected override System.Web.UI.Control TableParent
        {
            get { return pnl; }
        }

        protected override void DataBindDone()
        {
            string parentTypeName = DataSource["ParentFullTypeName"].Get<string>();
            parentTypeName = parentTypeName.Substring(parentTypeName.LastIndexOf(".") + 1);
            (Parent.Parent.Parent as Window).Caption = string.Format(
                "{0} {1}-{2}/{3} of {4}[{5}]/{6}",
                DataSource["TypeName"].Get<string>(),
                ((int)DataSource["Start"].Value) + 1,
                DataSource["End"].Get<int>(),
                DataSource["SetCount"].Get<int>(),
                parentTypeName,
                DataSource["ParentID"].Value,
                DataSource["ParentPropertyName"].Value);
            if (DataSource["Start"].Get<int>() > 0)
            {
                previous.Enabled = true;
                previousPnl.Visible = true;
            }
            else
            {
                previous.Enabled = false;
                if (DataSource["SetCount"].Get<int>() <=
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10))
                    previousPnl.Visible = false;
                else
                    previousPnl.Visible = true;
            }
            if (DataSource["End"].Get<int>() < DataSource["SetCount"].Get<int>())
            {
                next.Enabled = true;
                nextPnl.Visible = true;
            }
            else
            {
                next.Enabled = false;
                if (DataSource["SetCount"].Get<int>() <=
                    Settings.Instance.Get("DBAdmin.MaxItemsToShow", 10))
                    nextPnl.Visible = false;
                else
                    nextPnl.Visible = true;
            }
        }

        protected override void ReDataBind()
        {
            ResetColumnsVisibility();
            DataSource["Objects"].UnTie();
            //DataSource["Type"].UnTie();
            if (RaiseSafeEvent(
                "DBAdmin.Data.GetListFromObject",
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




















