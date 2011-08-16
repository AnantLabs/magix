/*
 * Magix - A Web Application Framework for Humans
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
    /**
     * Level2: Contains the UI for the RichEditor or WYSIWYG editor of Magix. That little guy 
     * resembling 'word'. Specify a 'SaveEvent' to trap when 'Text' is being edited.
     */
    [ActiveModule]
    public class RichEdit : ActiveModule
    {
        protected TextArea txt;
        protected Panel wrp;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

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

        // TODO: Implement support for multiple editors ...
        /**
         * Level2: Will flat out return the Value of the RichEditor. Does NOT support multiple
         * editors on the same screen
         */
        [ActiveEvent(Name = "Magix.Brix.Core.GetRichEditorValue")]
        protected void Magix_Brix_Core_GetRichEditorValue(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value = txt.Text;
        }

        [WebMethod]
        protected void Save(string text)
        {
            SaveEvent["Value"].Value = txt.Text;

            RaiseSafeEvent(
                SaveEvent.Get<string>(),
                SaveEvent);

            if (SaveEvent["Value"].Get<string>() != txt.Text)
                txt.Text = SaveEvent["Value"].Get<string>(); // Changed in save ...!
        }
    }
}

