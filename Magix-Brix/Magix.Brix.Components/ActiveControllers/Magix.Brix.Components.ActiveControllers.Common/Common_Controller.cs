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
    /**
     * Level2: Contains some common things and common control helpers. For instance loading of 
     * a color picker dialogue
     */
    [ActiveController]
    public class Common_Controller : ActiveController
    {
        /**
         * Level2: Will load up a Color Picker dialogue such that the end user can pick a color 
         * or image for using as e.g. background color/textures
         */
        [ActiveEvent(Name = "Magix.Core.PickColorOrImage")]
        protected void Magix_Core_PickColorOrImage(object sender, ActiveEventArgs e)
        {
            e.Params["ForcedSize"]["width"].Value = 430;
            e.Params["ForcedSize"]["height"].Value = 350;
            if (!e.Params.Contains("Caption"))
                e.Params["Caption"].Value = "Pick Color or Texture";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.PickColorOrImage",
                "child",
                e.Params);
        }

        /**
         * Level2: Will launch the File Explorer to allow the end user to pick and image 
         * for rendering as e.g. background textures for widgets etc
         */
        [ActiveEvent(Name = "Magix.Core.FilePicker.SelectFileInsteadOfColor")]
        protected void Magix_Core_FilePicker_SelectFileInsteadOfColor(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (e.Params.Contains("Top"))
                node["Top"].Value = e.Params["Top"].Value;
            else
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
