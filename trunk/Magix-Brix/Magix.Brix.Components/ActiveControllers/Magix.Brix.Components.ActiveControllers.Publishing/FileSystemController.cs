/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
    [ActiveController]
    public class FileSystemController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.ViewFileSystem")]
        protected void Magix_Publishing_ViewFileSystem(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Container"].Value = "content3";
            node["Filter"].Value = "*.png;*.jpeg;*.jpg;*.gif;*.css;";
            node["IsCreate"].Value = true;
            node["CanCreateNewCssFile"].Value = true;

            RaiseEvent(
                "FileExplorer.Form.LaunchFileExplorer",
                node);

            ActiveEvents.Instance.RaiseClearControls("content4");
            ActiveEvents.Instance.RaiseClearControls("content5");
            ActiveEvents.Instance.RaiseClearControls("content6");
        }
    }
}

























