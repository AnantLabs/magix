/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Web.UI;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using HelloWorldTypes;

namespace HelloWorldController
{
    [ActiveController]
    public class HelloWorldController : ActiveController
    {
        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        private void Page_Init_InitialLoading(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["CSSFile"].Value = "media/magic-ux-skins/default.css";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.AddCustomCssFile",
                node);

            node = new Node();
            node["CSSFile"].Value = "media/modules/SingleContainer.css";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.AddCustomCssFile",
                node);

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClasses");
            return;
            Page.Title = "Magix-Brix Hello World";
            Node n = new Node();
            n["Message"].Value = "I am Marvin, I am your guide through the Universe...";
            LoadModule(
                "HelloWorldModules.Hello",
                "dyn",
                n);
        }

        [ActiveEvent(Name = "Hitchhike")]
        private void GetNextCounter(object sender, ActiveEventArgs e)
        {
            Counter c = Counter.SelectFirst();
            if (c == null)
                c = new Counter();
            c.Value += 1;
            c.Save();
            e.Params["Value"].Value = c.Value;
            if ((new Random()).Next(5) == 0)
            {
                LoadModule(
                    "HelloWorldModules.Oops",
                    "dyn2");
            }
        }
    }
}






















