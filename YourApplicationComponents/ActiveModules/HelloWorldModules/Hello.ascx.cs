/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magic.UX.Widgets;
using Magic.Brix.Types;
using Magic.Brix.Loader;

namespace HelloWorldModules
{
    [ActiveModule]
    public class Hello : UserControl, IModule
    {
        protected Label lbl;
        protected Button btn;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    lbl.Text = node["Message"].Get<string>();
                };
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Hitchhike",
                node);
            btn.Text = "You've tried " + node["Value"].Get<int>() + " times - and still no success...";
        }
    }
}














