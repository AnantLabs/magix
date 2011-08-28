/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Tiny helper for menu item in Administrator Dashboard to view the File System Browser
     */
    [ActiveController]
    public class FileSystem_Controller : ActiveController
    {
        /**
         * Level2: Will call 'Magix.FileExplorer.LaunchExplorer' with a couple of 
         * predefined values for positioning and such. Clears all containers from 4 and out
         */
        [ActiveEvent(Name = "Magix.Publishing.ViewFileSystem")]
        protected void Magix_Publishing_ViewFileSystem(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["MarginBottom"].Value = 20;
            node["Last"].Value = true;
            node["Container"].Value = "content3";
            node["Filter"].Value = "*.*";
            node["IsCreate"].Value = true;
            node["CanCreateNewCssFile"].Value = true;
            node["RootAccessFolder"].Value = "/";
            node["Folder"].Value = "/";

            RaiseEvent(
                "Magix.FileExplorer.LaunchExplorer",
                node);

            ActiveEvents.Instance.RaiseClearControls("content4");
            ActiveEvents.Instance.RaiseClearControls("content5");
            ActiveEvents.Instance.RaiseClearControls("content6");
        }
    }
}
