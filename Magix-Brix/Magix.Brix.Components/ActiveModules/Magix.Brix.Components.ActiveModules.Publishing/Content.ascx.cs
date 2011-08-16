/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    /**
     * Level1: A 'content' PublisherPlugin. Basically just a text fragment, that'll be edited through
     * the Magix' RichText or WYSIWYG Editor
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class Content : ActiveModule
    {
        protected Label lbl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    lbl.Text = Text;

                    // TODO: WTF ...? Isn't this common for all ...?
                    // I seriously need to stop smoking pot ... ;)
                    if (node.Contains("OverflowWebPart"))
                        (this.Parent as DynamicPanel).CssClass += " web-part-overflow";
                };
        }

        /**
         * Level1: The actual content of the Content control ... ;)
         */
        [ModuleSetting(ModuleEditorName = "Magix.Brix.Components.ActiveModules.Editor.RichEdit", DefaultValue = "<p>The Content for your <strong style=\"color:Red;\">Rich</strong> Text <em>Content</em> WebPart ...</p>")]
        public string Text
        {
            get { return ViewState["Text"] as string; }
            set { ViewState["Text"] = value; }
        }

        /**
         * Level2: Overridden to just 'fetch the text', since such things are easier and faster for 
         * the server and such than reloading the entire page. Will basically determine if
         * the container and module name are the same, if they are, it'll return STOP which
         * will stop the reloading of the new module. Then this module will raise 
         * 'Magix.Publishing.GetWebPartSettingValue' for the new 'WebPartID', which will return
         * the new Text as 'Value'
         */
        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(Content).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;

                // We can get our next item without having to do a 'full reload' ... ;)
                Node node = new Node();
                node["WebPartID"].Value = e.Params["WebPartID"].Value;
                node["Name"].Value = "Text";

                RaiseSafeEvent(
                    "Magix.Publishing.GetWebPartSettingValue",
                    node);

                Text = node["Value"].Get<string>();
                lbl.Text = Text;
            }
        }
    }
}



