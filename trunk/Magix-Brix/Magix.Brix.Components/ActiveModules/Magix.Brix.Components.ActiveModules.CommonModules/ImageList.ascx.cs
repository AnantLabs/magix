/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    /**
     * Level2: Modules for displaying a List of Images. Pass in your Image list in the 'Items' parameter
     * to the LoadModule method
     */
    [ActiveModule]
    public class ImageList : ActiveModule
    {
        protected Panel wrp;
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    rep.DataSource = DataSource["Items"];
                    rep.DataBind();
                    if (node.Contains("ChildCssClass"))
                        wrp.CssClass = node["ChildCssClass"].Get<string>();
                };
        }

        protected void ImageClicked(object sender, EventArgs e)
        {
            ImageButton img = sender as ImageButton;

            // TODO: Parameters ...?
            RaiseSafeEvent(img.Info);
        }

        protected string GetTooltip(object objTip)
        {
            if (DataSource.Contains("DisplayTooltips") &&
                !DataSource["DisplayTooltips"].Get<bool>())
                return "";

            string toolTip = objTip as string;
            return toolTip;
        }
    }
}
