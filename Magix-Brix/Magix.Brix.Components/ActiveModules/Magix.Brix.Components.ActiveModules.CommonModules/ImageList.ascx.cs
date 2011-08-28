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
     */
    [ActiveModule]
    public class ImageList : ActiveModule
    {
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    rep.DataSource = DataSource["Items"];
                    rep.DataBind();
                };
        }

        protected void ImageClicked(object sender, EventArgs e)
        {
            Image img = sender as Image;

            // TODO: Parameters ...?
            RaiseSafeEvent(img.Info);
        }
    }
}
