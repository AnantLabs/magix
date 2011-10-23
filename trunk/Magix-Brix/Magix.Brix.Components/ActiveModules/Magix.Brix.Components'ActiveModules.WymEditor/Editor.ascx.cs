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

[assembly: WebResource("Magix.Brix.Components.ActiveModules.WymEditor.jquery.js", "text/javascript")]
[assembly: WebResource("Magix.Brix.Components.ActiveModules.WymEditor.jquery.wymeditor.min.js", "text/javascript")]

namespace Magix.Brix.Components.ActiveModules.WymEditor
{
    /**
     */
    [ActiveModule]
    public class Editor : ActiveModule
    {
        protected TextArea txt;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    foreach (string idx in new string[] { 
                        "Magix.Brix.Components.ActiveModules.WymEditor.jquery.js",
                        "Magix.Brix.Components.ActiveModules.WymEditor.jquery.wymeditor.min.js"
                    })
                    {
                        AjaxManager.Instance.IncludeScriptFromResource(
                            typeof(Editor),
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
                };
        }

        protected Node SaveEvent
        {
            get { return ViewState["SaveEvent"] as Node; }
            set { ViewState["SaveEvent"] = value; }
        }

        protected string GetBasePate()
        {
            return GetApplicationBaseUrl();
        }

        // TODO: Implement support for multiple editors ...
        /**
         * Level2: Will flat out return the Value of the Editor. Does NOT support multiple
         * editors on the same screen
         */
        [ActiveEvent(Name = "Magix.Brix.Core.GetRichEditorValue")]
        protected void Magix_Brix_Core_GetRichEditorValue(object sender, ActiveEventArgs e)
        {
            e.Params["Text"].Value = txt.Text;
        }

        /**
         * Level2: Sink Callback for being able to know when a new URL was created or
         * selected and tell our WYMIWYG editor about it
         */
        [ActiveEvent(Name = "Magix.Core.UrlWasCreated")]
        protected void Magix_Core_UrlWasCreated(object sender, ActiveEventArgs e)
        {
            AjaxManager.Instance.WriterAtBack.Write(
                "MUX.CustomerWYM.afterLink('{0}');", e.Params["URL"].Get<string>());
        }

        /**
         * Level2: Sink Callback for being able to know when a new Image was 
         * selected and tell our WYMIWYG editor about it
         */
        [ActiveEvent(Name = "Magix.Core.ImageWasSelected")]
        protected void Magix_Core_ImageWasSelected(object sender, ActiveEventArgs e)
        {
            AjaxManager.Instance.WriterAtBack.Write(
                "MUX.CustomerWYM.afterImage('{0}');", e.Params["URL"].Get<string>());
        }

        [WebMethod]
        protected void Save()
        {
            SaveEvent["Value"].Value = txt.Text;

            RaiseSafeEvent(
                SaveEvent.Get<string>(),
                SaveEvent);

            if (SaveEvent["Value"].Get<string>() != txt.Text)
                txt.Text = SaveEvent["Value"].Get<string>(); // Changed in save ...!
        }

        [WebMethod]
        protected void CreateLink()
        {
            RaiseSafeEvent("Magix.Core.GetHyperLinkURL");
        }

        [WebMethod]
        protected void CreateImage()
        {
            RaiseSafeEvent("Magix.Core.GetImageURL");
        }
    }
}
