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

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    /**
     * Level3: ActiveModule to help create upload files logic. Will raise 
     * 'Magix.Uploader.FileUploaded' consecutively for every file uploaded,
     * and 'Magix.Uploader.FileBatchUploadFinished' when all files in one
     * drag and drop operation are done. Alternatively it may raise the 
     * event defined in 'FileUploadedEvent' or 'AllFilesUploadedEvent'
     */
    [ActiveModule]
    public class Uploader : ActiveModule
    {
        protected Magix.UX.Widgets.Uploader uploader;

        protected void uploader_Uploaded(object sender, EventArgs e)
        {
            string filBase64 = uploader.GetFileRawBASE64();
            string fileName = uploader.GetFileName();

            if (!string.IsNullOrEmpty(DataSource["Filter"].Get<string>()))
            {
                string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
                bool allow = false;
                foreach (string idx in DataSource["Filter"].Get<string>().Split(
                    new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (fileType == idx.Substring(2))
                        allow = true;
                }
                if (!allow)
                {
                    Node n = new Node();

                    n["Message"].Value =
@"Sorry, but only filetypes of '" + DataSource["Filter"].Get<string>() + "' are allowed";
                    n["IsError"].Value = true;

                    RaiseEvent(
                        "Magix.Core.ShowMessage",
                        n);

                    return;
                }
            }

            DataSource["FileName"].Value = fileName;
            DataSource["File"].Value = filBase64;
            DataSource["SizeOfBatch"].Value = uploader.SizeOfBatch;
            DataSource["CurrentNo"].Value = uploader.CurrentNo;

            if (uploader.CurrentNo == 0)
            {
                // This is a new batch ...
                // Emptying our previous 'batch' ...
                FilesInBatch.Clear();
            }

            FilesInBatch.Add(fileName);

            RaiseSafeEvent(
                DataSource.Contains("FileUploadedEvent") ? 
                    DataSource["FileUploadedEvent"].Get<string>() : 
                    "Magix.Uploader.FileUploaded",
                DataSource);

            if (uploader.CurrentNo == uploader.SizeOfBatch - 1)
            {
                // Last file in current 'batch' was just finished processed ...
                DataSource["Files"].UnTie();
                int idxNo = 0;

                foreach (string idx in FilesInBatch)
                {
                    DataSource["Files"]["f-" + idxNo].Value = idx;
                    idxNo += 1;
                }

                RaiseSafeEvent(
                    DataSource.Contains("AllFilesUploadedEvent") ? 
                        DataSource["AllFilesUploadedEvent"].Get<string>() : 
                        "Magix.Uploader.FileBatchUploadFinished",
                    DataSource);
            }
        }

        /*
         * Contains all files in a current dragging operation ...
         */
        private List<string> FilesInBatch
        {
            get
            {
                if (ViewState["FilesInBatch"] == null)
                    ViewState["FilesInBatch"] = new List<string>();
                return (List<string>)ViewState["FilesInBatch"];
            }
        }
    }
}
