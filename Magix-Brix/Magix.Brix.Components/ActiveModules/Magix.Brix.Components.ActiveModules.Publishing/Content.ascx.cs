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
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    [PublisherPlugin]
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
                    if (node.Contains("OverflowWebPart"))
                        (this.Parent as DynamicPanel).CssClass += " web-part-overflow";
                };
        }

        [ModuleSetting(ModuleEditorName = "Magix.Brix.Components.ActiveModules.Editor.RichEdit", DefaultValue = "<p>The Content for your Rich Content WebPart ...</p>")]
        public string Text
        {
            get { return ViewState["Text"] as string; }
            set { ViewState["Text"] = value; }
        }

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
                    "Magix.Publishing.GetWebPartValue",
                    node);

                Text = node["Value"].Get<string>();
                lbl.Text = Text;
            }
        }
    }
}



