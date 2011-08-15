/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.IO;
using System.Web;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using System.Web.UI;

namespace Magix.Brix.Components.ActiveControllers.FileExplorer
{
    /**
     * Level1: Contains Logic for file explorer. The file explorer is a component where you can browse
     * the file folder system on your web server, remotely, and do many operations. Such as
     * editing CSS files, uploading images and such. PS! To use this Module it is IMPERATIVE
     * that you've given the 'NETWORK SERVICE' account 'Full Access' to at the very least
     * the 'media/' folder, or whatever folder you plan to use the explorer on on your
     * web server
     */
    [ActiveController]
    public class FileExplorerController : ActiveController
    {
        /**
         * Level1: Use this method to launch the file explorer. You can override any parameter you wish.
         * The default file filter is; "*.png;*.jpeg;*.jpg;*.gif;" but can be changed through the
         * "Filter" parameter. If you add up CSS, and set "CanCreateNewCssFile" to true, then
         * CSS files can both be edited and created on the fly. If you set "IsSelect", then
         * the end user can "select" files, which upon selection is done will raise the
         * 'SelectEvent'
         */
        [ActiveEvent(Name = "Magix.FileExplorer.LaunchExplorer")]
        protected void Magix_FileExplorer_LaunchExplorer(object sender, ActiveEventArgs e)
        {
            // Default values used unless input node has different ideas ...
            string folder = "media/";
            string filter = "*.png;*.jpeg;*.jpg;*.gif;";

            Node node = e.Params;

            if (!node.Contains("Folder"))
                node["Folder"].Value = folder;

            if (!node.Contains("Filter"))
                node["Filter"].Value = filter;

            LaunchFileExplorer(node);
        }

        /*
         * Helper for the above ...
         */
        private void LaunchFileExplorer(Node node)
        {
            string filter = node["Filter"].Get<string>();
            string folder = node["Folder"].Get<string>();

            if (filter == null)
                throw new ArgumentException(@"You have to give the FileExplorer a 'Filter' parameter 
to explain it which file types to show");

            if (folder == null)
                throw new ArgumentException(@"You have to give the FileExplorer a 'Folder' parameter 
to explain it where to start showing files to the user");

            node["RootAccessFolder"].Value = folder;

            ExecuteSafely(
                delegate
                {
                    Helper.GetFilesAndFolders(folder, filter, node);
                },
                @"Something went wrong while trying to get Files and Folder for FileExplorer. Are you sure you have given the 'NETWORK SERVICE' Windows User 
Account 'Full Control' over the specific folder?");

            node["Caption"].Value = folder;

            // Defaulting to child container ...
            string container = "child";

            if (node.Contains("Container"))
                container = node["Container"].Get<string>();

            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.FileExplorer.Explorer",
                container,
                node);
        }

        /**
         * Level2: Will retrieve the 'FolderToOpen' folder's files back to caller
         */
        [ActiveEvent(Name = "Magix.FileExplorer.GetFilesFromFolder")]
        protected void Magix_FileExplorer_GetFilesFromFolder(object sender, ActiveEventArgs e)
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

            ExecuteSafely(
                delegate
                {
                    Helper.GetFilesAndFolders(
                        folder,
                        e.Params["Filter"].Get<string>(),
                        e.Params);
                },
                "You sure the 'NETWORK SERVICE' Windows User Account has 'Full Access' to the folder {0} ...?",
                folder);

            e.Params["Caption"].Value = folder;
        }

        /**
         * Level1: Will open up EitAsciiFile 'Notepad'ish' Editor with the given 'File' for editing.
         * Intended to allow editing of CSS files and other types of ASCII files
         */
        [ActiveEvent(Name = "Magix.FileExplorer.EditAsciiFile")]
        protected void Magix_FileExplorer_EditAsciiFile(object sender, ActiveEventArgs e)
        {
            string fullPath = HttpContext.Current.Server.MapPath("~/" + e.Params["File"].Get<string>());

            Node node = new Node();

            node["File"].Value = fullPath;
            node["Width"].Value = 24;
            node["Caption"].Value = "Editing file: " + 
                fullPath.Replace(HttpContext.Current.Server.MapPath("~"), "");

            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.FileExplorer.EditAsciiFile",
                "child",
                node);
        }

        /**
         * Level1: Will retrieve the properties for the file, such as disc size, width/height if image, etc
         */
        [ActiveEvent(Name = "Magix.FileExplorer.FileSelected")]
        protected void Magix_FileExplorer_FileSelected(object sender, ActiveEventArgs e)
        {
            string file = e.Params["File"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();

            ExecuteSafely(
                delegate
                {
                    Helper.GetFileProperties(folder, file, e.Params);
                },
                "You sure you gave the 'NETWORK SERVICE' Windows User Acount Full Access to the folder {0}",
                folder);
        }

        /**
         * Level1: Changes the name of the file with 'OldName' to 'NewName' within the given 'Folder'
         */
        [ActiveEvent(Name = "Magix.FileExplorer.ChangeFileName")]
        protected void Magix_FileExplorer_ChangeFileName(object sender, ActiveEventArgs e)
        {
            string newName = e.Params["NewName"].Get<string>();
            string oldName = e.Params["OldName"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();
            Helper.ChangeFileName(folder, oldName, newName);

            ExecuteSafely(
                delegate
                {
                    Helper.GetFilesAndFolders(
                        folder,
                        e.Params["Filter"].Get<string>(),
                        e.Params);

                    Helper.GetFileProperties(
                        folder,
                        newName + oldName.Substring(oldName.LastIndexOf(".")),
                        e.Params);
                },
                "You sure you gave the 'NETWORK SERVICE' Windows User Account Full Access to the folder {0}",
                folder);
        }

        /**
         * Level1: Will delete the 'File' within the 'Folder' of the system
         */
        [ActiveEvent(Name = "Magix.FileExplorer.DeleteFile")]
        protected void Magix_FileExplorer_DeleteFile(object sender, ActiveEventArgs e)
        {
            string fileName = 
                e.Params["Folder"].Get<string>() +
                e.Params["File"].Get<string>();

            ExecuteSafely(
                delegate
                {
                    Helper.DeleteFile(fileName);
                },
                @"You sure you gave the 'NETWORK SERVICE' Windows User Account Full Access to the folder {0}.
or might the file {1} be non-writeable",
                e.Params["Folder"].Get<string>(),
                e.Params["File"].Get<string>());
        }
    }
}
