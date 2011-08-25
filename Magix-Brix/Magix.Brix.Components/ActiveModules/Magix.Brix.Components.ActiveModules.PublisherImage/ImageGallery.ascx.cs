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
     * Level1: Represents an Image Gallery plugin for the Publishing system. Allows for
     * browsing of Images through a Gallery lookalike UI
     */
    [ActiveModule]
    [PublisherPlugin]
    public class ImageGallery : ActiveModule
    {
        protected ASP.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    if (string.IsNullOrEmpty(GalleryName) && node.Contains("GalleryName"))
                    {
                        GalleryName = node["GalleryName"].Get<string>();
                    }
                    else
                    {
                        DataSource["GalleryName"].Value = GalleryName;
                    }

                    RaiseSafeEvent(
                        "Magix.Gallery.GetGalleryData",
                        DataSource);

                    rep.DataSource = DataSource["Images"];
                    rep.DataBind();
                };
        }

        protected int GetNoImages()
        {
            return DataSource["Images"].Count;
        }

        /**
         * Level1: The Unique name of the Image Gallery
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.Gallery.GetTemplateColumnSelectGallery")]
        public string GalleryName
        {
            get { return ViewState["GalleryName"] as string; }
            set { ViewState["GalleryName"] = value; }
        }
    }
}



