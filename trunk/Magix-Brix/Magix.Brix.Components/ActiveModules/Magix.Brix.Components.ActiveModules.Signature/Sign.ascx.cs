/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.UX.Core;

namespace Magix.Brix.Components.ActiveModules.Signature
{
    [ActiveModule]
    public class Sign : UserControl, IModule
    {
        protected HtmlInputButton sub;
        protected Button cancel;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    if (DataSource.Contains("Coords") &&
                        !string.IsNullOrEmpty(DataSource["Coords"].Get<string>()))
                    {
                        sub.Visible = false;
                        cancel.Text = "Close";
                    }
                };
        }

        protected string GetCoords()
        {
            if (DataSource.Contains("Coords") &&
                !string.IsNullOrEmpty(DataSource["Coords"].Get<string>()))
                return DataSource["Coords"].Get<string>();
            return "[]";
        }

        protected int GetWidth()
        {
            if (DataSource.Contains("Width"))
                return DataSource["Width"].Get<int>();
            return 710;
        }

        protected int GetHeight()
        {
            if (DataSource.Contains("Height"))
                return DataSource["Height"].Get<int>();
            return 360;
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }

        [WebMethod]
        protected void submitSignature(string signature)
        {
            DataSource["OKEvent"]["Params"]["Signature"].Value = signature;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["OKEvent"].Get<string>(),
                DataSource["OKEvent"]["Params"]);
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["CancelEvent"].Get<string>(),
                DataSource["CancelEvent"]["Params"]);
        }
    }
}





























