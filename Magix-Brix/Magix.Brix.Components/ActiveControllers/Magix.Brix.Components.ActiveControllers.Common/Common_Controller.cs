/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveControllers.Common
{
    [ActiveController]
    public class Common_Controller : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.PickColorOrImage")]
        protected void Magix_Core_PickColorOrImage(object sender, ActiveEventArgs e)
        {
            e.Params["ForcedSize"]["width"].Value = 430;
            e.Params["ForcedSize"]["height"].Value = 350;
            e.Params["Caption"].Value = "Pick Color or Texture";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.PickColorOrImage",
                "child",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.Core.FilePicker.SelectFileInsteadOfColor")]
        protected void Magix_Core_FilePicker_SelectFileInsteadOfColor(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 2;
            node["IsSelect"].Value = true;
            node["ForcedSize"]["width"].Value = 910;
            node["ForcedSize"]["height"].Value = 432;
            node["Folder"].Value = "/";
            node["Filter"].Value = "*.png;*.gif;*.jpg;*.jpeg;";
            node["SelectEvent"].Value = "Magix.Core.ColorPicker.FileSelected";

            RaiseEvent(
                "Magix.FileExplorer.LaunchExplorer",
                node);
        }
    }
}
