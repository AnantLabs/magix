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
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.PublisherImage
{
    /**
     */
    [ActiveModule]
    [PublisherPlugin]
    public class Galleries : ActiveModule
    {
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    RaiseEvent(
                        "Magix.Publishing.Gallery.GetAllGalleries",
                        DataSource);

                    rep.DataSource = DataSource["Galleries"];
                    rep.DataBind();
                };
        }

        protected void OpenGallery(object sender, EventArgs e)
        {
            DataSource["ID"].Value = int.Parse((sender as Panel).Info);

            RaiseSafeEvent(
                "Magix.Publishing.Gallery.OpenGalleryInCurrentContainer",
                DataSource);
        }
    }
}



