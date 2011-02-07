/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveModules.TalkBack
{
    [ActiveModule]
    public class Forum : System.Web.UI.UserControl, IModule
    {
        protected Panel wrp;
        protected System.Web.UI.WebControls.Repeater rep;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node["Posts"];
                    DataBindRepeater();
                };
        }

        private void DataBindRepeater()
        {
            rep.DataSource = DataSource;
            rep.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}




