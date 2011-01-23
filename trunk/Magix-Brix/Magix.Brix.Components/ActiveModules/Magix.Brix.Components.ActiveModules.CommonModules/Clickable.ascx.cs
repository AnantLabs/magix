/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    [ActiveModule]
    public class Clickable : UserControl, IModule
    {
        protected Button btn;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                btn.Text = node["Text"].Get<string>();
                Event = node["Event"];
            };
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            Node node = Event;
            RaiseSafeEvent(
                Event.Get<string>(),
                node);
        }

        protected bool RaiseSafeEvent(string eventName, Node node)
        {
            try
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    eventName,
                    node);
                return true;
            }
            catch (Exception err)
            {
                Node n = new Node();
                while (err.InnerException != null)
                    err = err.InnerException;
                n["Message"].Value = err.Message;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    n);
                return false;
            }
        }

        private Node Event
        {
            get { return ViewState["Event"] as Node; }
            set { ViewState["Event"] = value; }
        }
    }
}



