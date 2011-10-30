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
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.PublisherUploader
{
    /**
     * Level2: File Uploader plugin module for the Publishing system. By having this module
     * on your WebPage, you will allow the end user to drag and drop files onto anywhere on
     * the browser, which will trigger uploading of those files to the server
     */
    [ActiveModule]
    [PublisherPlugin]
    public class Uploader : ActiveModule
    {
        protected Magix.UX.Widgets.Uploader uploader;

        protected void uploader_Uploaded(object sender, EventArgs e)
        {
            string filBase64 = uploader.GetFileRawBASE64();
            string fileName = uploader.GetFileName();

            if (!string.IsNullOrEmpty(Filter))
            {
                string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
                bool allow = false;
                foreach (string idx in Filter.Split(
                    new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (fileType == idx.Substring(2))
                        allow = true;
                }
                if (!allow)
                {
                    Node n = new Node();

                    n["Message"].Value = @"Sorry, but only filetypes of '" + Filter + "' are allowed";
                    n["IsError"].Value = true;

                    RaiseEvent(
                        "Magix.Core.ShowMessage",
                        n);
                    return;
                }
            }

            DataSource["FileName"].Value = fileName;
            DataSource["File"].Value = filBase64;
            DataSource["ActionName"].Value = ActionName;
            DataSource["Folder"].Value = Folder;
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
                "Magix.Core.FileUploaded",
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
                    "Magix.Core.FileBatchUploadFinished",
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

        /**
         * Level2: If given, will only show files of given types. Should contain a string of 
         * file filters separated by semi-colons. E.g. "*.png;*.jpg;" will allow for all 
         * files having either the .png or the .jpg extensions. By default the control 
         * tolerates image files, CSV files, PDF files, CSS files, text files [.txt], 
         */
        [ModuleSetting(DefaultValue = "*.png;*.gif;*.jpg;*.jpeg;*.csv;*.pdf;*.css;*.txt;*.ttf;")]
        public string Filter
        {
            get { return ViewState["Filter"] as string; }
            set { ViewState["Filter"] = value; }
        }

        /**
         * Level2: If given, will Raise the given Action with the 'FileName', 
         * full relative path, after saving file on server. The default value of 'NO-ACTION'
         * will make sure no Actions are being raised. If you wish you can separate multiple 
         * actions with the pipe symbol (|) to have multiple Actions raised
         */
        [ModuleSetting(DefaultValue = "", ModuleEditorEventName = "Magix.Publishing.GetSelectActionForModuleControl")]
        public string ActionName
        {
            get { return ViewState["ActionName"] as string; }
            set { ViewState["ActionName"] = value; }
        }

        /**
         * Level2: What folder on your server you wish the plugin to use for saving
         * file within
         */
        [ModuleSetting(DefaultValue = "Tmp/")]
        public string Folder
        {
            get { return ViewState["Folder"] as string; }
            set { ViewState["Folder"] = value; }
        }
    }
}
