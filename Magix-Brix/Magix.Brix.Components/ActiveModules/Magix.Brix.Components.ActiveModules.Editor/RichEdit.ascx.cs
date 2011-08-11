/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
        protected Panel wrp;

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
                    string text = node["Text"].Get<string>();

                    // Normalizing text, making sure it's made on paragraph form ...!
                    if (string.IsNullOrEmpty(text.Trim()))
                        text = "Default text...";
                    if (text.IndexOf("<p") == -1)
                        text = "<p>" + text + "</p>";
                    txt.Text = text;
                    SaveEvent = node["SaveEvent"].UnTie();

                    if (node.Contains("NoChrome") && node["NoChrome"].Get<bool>())
                    {
                        wrp.CssClass = wrp.CssClass.Replace(" mux-rich-editor", " mux-rich-editor-no-chrome");
                    }
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
            node["Params"] = SaveEvent["Params"];
            RaiseSafeEvent(
                SaveEvent.Get<string>(),
                node);
            if (node["Text"].Get<string>() != txt.Text)
                txt.Text = node["Text"].Get<string>(); // Changed in save ...!
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

