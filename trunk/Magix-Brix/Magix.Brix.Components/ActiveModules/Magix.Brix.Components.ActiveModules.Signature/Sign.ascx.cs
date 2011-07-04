/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
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
        protected Label waver;
        protected System.Web.UI.HtmlControls.HtmlInputButton sub;
        protected Button cancel;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    waver.Text = node["Waver"].Get<string>();
                    if (DataSource.Contains("ReadOnly") &&
                        DataSource["ReadOnly"].Get<bool>())
                    {
                        sub.Visible = false;
                        cancel.Text = "Close";
                    }
                };
        }

        protected string GetCoords()
        {
            if (DataSource.Contains("Coords"))
                return DataSource["Coords"].Get<string>();
            return "[]";
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





























