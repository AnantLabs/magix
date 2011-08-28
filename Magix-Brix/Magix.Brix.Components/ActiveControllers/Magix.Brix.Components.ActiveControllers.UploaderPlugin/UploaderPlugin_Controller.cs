/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Magix.UX;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    /**
     * Level2: Contains logic for being able to upload files to the server
     */
    [ActiveController]
    public class UploaderPlugin_Controller : ActiveController
    {
        /**
         * Level2: Will save the given 'File' to the given 'Folder' with the given 'FileName' and
         * then raise 'ActionName', unless it's either null or empty, or it's "NO-ACTION". If the
         * Action starts with 'finished:', the action will only be raised when an entire batch
         * is done. Otherwise the action will be raise consecutively for every file within the
         * same batch
         */
        [ActiveEvent(Name = "Magix.Core.FileUploaded")]
        protected void Magix_Core_FileUploaded(object sender, ActiveEventArgs e)
        {
            string fileContent = e.Params["File"].Get<string>();
            string fileName = e.Params["FileName"].Get<string>();
            string folder = e.Params["Folder"].Get<string>();
            string actionNameToRaise = e.Params["ActionName"].Get<string>();

            if (!string.IsNullOrEmpty(folder))
            {
                folder = folder.Trim('/');
                if (!string.IsNullOrEmpty(folder))
                    folder = folder + "/";
            }

            string fullPath = Page.MapPath("~/" + folder + fileName);

            byte[] rawContent = Convert.FromBase64String(fileContent);

            using (FileStream stream = File.Create(fullPath))
            {
                stream.Write(rawContent, 0, rawContent.Length);
            }

            // Raising a action, if we should
            if (!string.IsNullOrEmpty(actionNameToRaise) && 
                actionNameToRaise != "NO-ACTION")
            {
                foreach (string idx in actionNameToRaise.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (idx.StartsWith("finished:"))
                        continue;

                    e.Params["ActionName"].Value = idx;

                    RaiseEvent(
                        "Magix.MetaAction.RaiseAction",
                        e.Params);
                }
            }
        }

        /**
         * Raised when an entire batch of files are done. If the 'ActionName' starts with
         * 'finished:', the Action given in the | separated string will be consecutively
         * raised
         */
        [ActiveEvent(Name = "Magix.Core.FileBatchUploadFinished")]
        protected void Magix_Core_FileBatchUploadFinished(object sender, ActiveEventArgs e)
        {
            string actionNameToRaise = e.Params["ActionName"].Get<string>();

            // Raising a action, if we should
            if (!string.IsNullOrEmpty(actionNameToRaise) &&
                actionNameToRaise != "NO-ACTION")
            {
                foreach (string idx in actionNameToRaise.Split(
                    new char[] { '|' }, 
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!idx.StartsWith("finished:"))
                        continue;

                    string actName = idx.Substring(9);

                    e.Params["ActionName"].Value = actName;

                    RaiseEvent(
                        "Magix.MetaAction.RaiseAction",
                        e.Params);
                }
            }
        }
    }
}
