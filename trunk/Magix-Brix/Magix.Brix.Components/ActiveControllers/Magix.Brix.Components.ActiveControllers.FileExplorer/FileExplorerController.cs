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
        protected void LaunchFileExplorer(object sender, ActiveEventArgs e)
        {
            string folder = "media/";
            string filter = "*.png;*.jpeg;*.jpg;*.gif;";

            Node node = new Node();
            node["Top"].Value = 4;
            node["Padding"].Value = 1;
            node["Width"].Value = 22;

            node["RootAccessFolder"].Value = folder;
            node["IsSelect"].Value = false;

            Helper.GetFilesAndFolders(folder, filter, node);
            node["Caption"].Value = string.Format("Exploring: '" + folder + "', {0} files, {1} folders",
                node["Files"].Count,
                node["Directories"].Count);

            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.FileExplorer.Explorer",
                "child",
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
