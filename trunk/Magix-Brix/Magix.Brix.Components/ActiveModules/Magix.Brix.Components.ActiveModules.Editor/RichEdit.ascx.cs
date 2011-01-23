/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Core;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;

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
                    SaveEvent = node["SaveEvent"].UnTie();
                };
        }

        protected Node SaveEvent
        {
            get { return ViewState["SaveEvent"] as Node; }
            set { ViewState["SaveEvent"] = value; }
        }

        [ActiveEvent(Name = "Magix.Brix.Core.GetRichEditorValue")]
        protected void Magix_Brix_Core_GetRichEditorValue(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value = txt.Text;
        }

        [WebMethod]
        protected void Save(string text)
        {
            Node node = new Node();
            node["Text"].Value = txt.Text;
            RaiseSafeEvent(
                SaveEvent.Get<string>(),
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
    }
}

