/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.IO;
using System.Web;
using System.Drawing;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveControllers.FileExplorer
{
    public class Helper
    {
        public static void GetFilesAndFolders(
            string folder, 
            string filter, 
            Node node)
        {
            if (folder == null)
                throw new ApplicationException(
                    @"Cannot open a 'null' folder");

            folder = CleanUpFolder(folder);

            node["Folder"].Value = folder;
            node["Filter"].Value = filter;

            string webServerApp = HttpContext.Current.Server.MapPath("~/");
            string webServerFolder = folder.Replace("/", "\\");
            int idxNo = 0;
            foreach (string idxFolder in Directory.GetDirectories(webServerApp + webServerFolder))
            {
                node["Directories"]["D" + idxNo]["Name"].Value =
                    idxFolder.Substring(idxFolder.LastIndexOf("\\") + 1);
                idxNo += 1;
            }
            idxNo = 0;
            foreach (string idxFilter in filter.Split(';'))
            {
                if (string.IsNullOrEmpty(idxFilter) || 
                    idxFilter.Trim().Length == 0)
                    continue;
                foreach (string idxFile in Directory.GetFiles(
                    webServerApp + webServerFolder, idxFilter))
                {
                    switch (idxFilter.Substring(idxFilter.LastIndexOf(".") + 1))
                    {
                        case "png":
                        case "gif":
                        case "jpeg":
                        case "jpg":
                            node["Files"]["F" + idxNo]["IsImage"].Value = true;
                            using (Bitmap b = Bitmap.FromFile(idxFile) as Bitmap)
                            {
                                if (b.Width > b.Height)
                                    node["Files"]["F" + idxNo]["Wide"].Value = true;
                            }
                            break;
                    }
                    node["Files"]["F" + idxNo]["FullPath"].Value = idxFile;
                    node["Files"]["F" + idxNo]["Name"].Value =
                        idxFile.Substring(idxFile.LastIndexOf("\\") + 1);
                    idxNo += 1;
                }
            }
        }

        private static string CleanUpFolder(string folder)
        {
            if (folder != string.Empty)
            {
                // Not opening 'root' folder ...
                // (which shouldn't be allowed anyway in your app ...!!!!!!
                folder = folder.Replace("\\", "/");
                if (folder[0] == '/')
                    folder = folder.Substring(1);
                if (folder[folder.Length - 1] != '/')
                    folder += "/";
            }
            return folder;
        }

        public static void GetFileProperties(string folder, string file, Node node)
        {
            folder = CleanUpFolder(folder);

            string webServerApp = HttpContext.Current.Server.MapPath("~/");
            string webServerFolder = folder.Replace("/", "\\");
            string fullFilePath = webServerApp + webServerFolder + file;

            FileInfo info = new FileInfo(fullFilePath);

            node["File"]["FullPathName"].Value = fullFilePath;
            node["File"]["FullName"].Value = info.Name;
            node["File"]["Extension"].Value = info.Extension;
            node["File"]["Name"].Value = info.Name.Replace(info.Extension, "");
            node["File"]["Created"].Value = info.CreationTime;
            node["File"]["LastUpdated"].Value = info.LastWriteTime;
            node["File"]["IsReadOnly"].Value = info.IsReadOnly;
            node["File"]["Size"].Value = info.Length;

            switch (info.Extension)
            {
                case ".gif":
                case ".png":
                case ".jpg":
                case ".jpeg":
                    using (Bitmap b = Bitmap.FromFile(fullFilePath) as Bitmap)
                    {
                        node["File"]["IsImage"].Value = true;
                        node["File"]["ImageHeight"].Value = b.Height;
                        node["File"]["ImageWidth"].Value = b.Width;
                    }
                    break;
                default:
                    node["File"]["IsImage"].UnTie();
                    node["File"]["ImageHeight"].UnTie();
                    node["File"]["ImageWidth"].UnTie();
                    break;
            }
        }
    }
}
