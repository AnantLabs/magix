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
    public class Header : ActiveModule
    {
        protected Label lbl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    lbl.Text = Caption;
                };
        }

        [ModuleSetting(DefaultValue = "Caption for your Caption WebPart ...")]
        public string Caption
        {
            get { return ViewState["Caption"] as string; }
            set { ViewState["Caption"] = value; }
        }

        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(Header).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;

                // We can get our next item without having to do a 'full reload' ... ;)
                Node node = new Node();
                node["WebPartID"].Value = e.Params["WebPartID"].Value;
                node["Name"].Value = "Caption";

                RaiseSafeEvent(
                    "Magix.Publishing.GetWebPartValue",
                    node);

                Caption = node["Value"].Get<string>();
                lbl.Text = Caption;
            }
        }
    }
}



