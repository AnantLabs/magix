/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.editor-min.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.button-min.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.container-min.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.element-min.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.menu-min.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.Editor.yahoo-dom-event.js", "text/javascript")]

namespace Magix.Brix.Components.ActiveModules.Editor
{
    [ActiveModule]
    public class RichEdit : UserControl, IModule
    {
        protected TextArea txt;

        void IModule.InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    foreach (string idx in new string[] { 
                        "Magix.Brix.Components.ActiveModules.Editor.yahoo-dom-event.js",
                        "Magix.Brix.Components.ActiveModules.Editor.element-min.js",
                        "Magix.Brix.Components.ActiveModules.Editor.container-min.js",
                        "Magix.Brix.Components.ActiveModules.Editor.menu-min.js",
                        "Magix.Brix.Components.ActiveModules.Editor.button-min.js",
                        "Magix.Brix.Components.ActiveModules.Editor.editor-min.js"
                    })
                    {
                    AjaxManager.Instance.IncludeScriptFromResource(
                        typeof(RichEdit),
                        idx,
                        false);
                    }
                    txt.Text = node["Text"].Get<string>();
                };
        }

        protected void save_Click(object sender, EventArgs e)
        {
        }
    }
}

