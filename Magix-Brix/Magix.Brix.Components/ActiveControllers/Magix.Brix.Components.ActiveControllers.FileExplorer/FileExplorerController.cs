/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.IO;
using System.Web;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.FileExplorer
{
    [ActiveController]
    public class FileExplorerController
    {
        [ActiveEvent(Name = "FileExplorer.Form.LaunchFileExplorer")]
        protected void FileExplorer_Form_LaunchFileExplorer(object sender, ActiveEventArgs e)
        {
            string folder = "media/";
            string filter = "*.png;*.jpeg;*.jpg;*.gif;"; // TODO: Implement support for more file types ...

            Node node = new Node();
            node["Top"].Value = 4;
            if (e.Params.Contains("Top"))
                node["Top"].Value = e.Params["Top"].Get<int>();
            node["Padding"].Value = 1;
            if (e.Params.Contains("Padding"))
                node["Padding"].Value = e.Params["Padding"].Get<int>();
            node["Width"].Value = 22;
            if (e.Params.Contains("Width"))
                node["Width"].Value = e.Params["Width"].Get<int>();
            if (e.Params.Contains("Container"))
                node["Container"].Value = e.Params["Container"].Get<string>();
            if (e.Params.Contains("Last"))
                node["Last"].Value = e.Params["Last"].Get<bool>();
            node["Folder"].Value = folder;
            node["Filter"].Value = filter;
            LaunchFileExplorer(
                node, 
                folder, 
                filter);
        }

        [ActiveEvent(Name = "FileExplorer.Form.LaunchFileExplorerWithParams")]
        protected void FileExplorer_Form_LaunchFileExplorerWithParams(object sender, ActiveEventArgs e)
        {
            string folder = e.Params["RootAccessFolder"].Get<string>();
            string filter = e.Params["Filter"].Get<string>();
            LaunchFileExplorer(e.Params, folder, filter);
        }

        private void LaunchFileExplorer(Node node, string folder, string filter)
        {
            node["RootAccessFolder"].Value = folder;

            Helper.GetFilesAndFolders(folder, filter, node);
            node["Caption"].Value = string.Format("Exploring: '" + folder + "', {0} files, {1} folders",
                node["Files"].Count,
                node["Directories"].Count);

            string container = "child";
            if (node.Contains("Container"))
                container = node["Container"].Get<string>();

            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.FileExplorer.Explorer",
                container,
                node);
        }

        [ActiveEvent(Name = "FileExplorer.GetFilesFromFolder")]
        protected void FileExplorer_GetFilesFromFolder(object sender, ActiveEventArgs e)
        {
            string folderToOpen = e.Params["FolderToOpen"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();

            if (folderToOpen == "../")
            {
                // Going up ...!
                folder = folder.Trim('/');
                folder = folder.Substring(0, folder.LastIndexOf('/') + 1);
            }
            else
                folder += string.IsNullOrEmpty(folderToOpen) ? "" : folderToOpen.Trim('/') + "/";

            Helper.GetFilesAndFolders(
                folder,
                e.Params["Filter"].Get<string>(),
                e.Params);
            e.Params["Caption"].Value = string.Format("Exploring: '" + folder + "', {0} files, {1} folders",
                e.Params["Files"].Count,
                e.Params["Directories"].Count);
        }

        [ActiveEvent(Name = "FileExplorer.FileSelectedInExplorer")]
        protected void FileExplorer_FileSelectedInExplorer(object sender, ActiveEventArgs e)
        {
            string file = e.Params["File"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();
            Helper.GetFileProperties(folder, file, e.Params);
        }

        [ActiveEvent(Name = "FileExplorer.ChangeFileName")]
        protected void FileExplorer_ChangeFileName(object sender, ActiveEventArgs e)
        {
            string newName = e.Params["NewName"].Get<string>();
            string oldName = e.Params["OldName"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();
            Helper.ChangeFileName(folder, oldName, newName);

            Helper.GetFilesAndFolders(
                folder,
                e.Params["Filter"].Get<string>(),
                e.Params);
            Helper.GetFileProperties(
                folder, 
                newName + oldName.Substring(oldName.LastIndexOf(".")), 
                e.Params);
        }

        [ActiveEvent(Name = "FileExplorer.DeleteFile")]
        protected void FileExplorer_DeleteFile(object sender, ActiveEventArgs e)
        {
            string fileName = 
                e.Params["Folder"].Get<string>() +
                e.Params["File"].Get<string>();
            Helper.DeleteFile(fileName);
        }
    }
}
