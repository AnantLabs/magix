/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewClassDetails : System.Web.UI.UserControl, IModule
    {
        protected Label count;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    count.Text = string.Format("Class has {0} instances", 
                        node["Count"].Value);
                    FullTypeName = node["FullTypeName"].Get<string>();
                };
        }

        private string FullTypeName
        {
            get { return ViewState["FullTypeName"] as string; }
            set { ViewState["FullTypeName"] = value; }
        }

        protected void select_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = FullTypeName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ViewAllInstances",
                node);
        }
    }
}




















