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
     * Level1: A 'header' PublisherPlugin. Basically just an HTML h1 element
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
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

        /**
         * Level1: BAsically the content of the h1 element. Default value is
         * 'The Caption of your Header'
         */
        [ModuleSetting(DefaultValue = "The Caption of your Header WebPart")]
        public string Caption
        {
            get { return ViewState["Caption"] as string; }
            set { ViewState["Caption"] = value; }
        }

        /**
         * Level2: Will return false if this webpart can just be updated by asking for new data
         */
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
                    "Magix.Publishing.GetWebPartSettingValue",
                    node);

                Caption = node["Value"].Get<string>();
                lbl.Text = Caption;
            }
        }
    }
}



