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

        void IModule.InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    UpdateCaption();
                };
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




















