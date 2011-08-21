/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.IO;
using System.Web;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Gallery;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveControllers.GalleryPlugin
{
    [ActiveController]
    public class GalleryPlugin_Controller : ActiveController
    {
        [ActiveEvent(Name = "Magix.Gallery.GetGalleryData")]
        protected void Magix_Gallery_GetGalleryData(object sender, ActiveEventArgs e)
        {
            Gallery g = Gallery.SelectFirst(
                Criteria.Eq(
                    "Name", 
                    e.Params["GalleryName"].Get<string>()));

            int idxNo = 0;
            foreach (Gallery.Image idx in g.Images)
            {
                e.Params["Images"]["i-" + idxNo].Value = idx.FileName;
                idxNo += 1;
            }
        }
    }
}
