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
         * then raise 'ActionName', unless it's either null or empty, or it's "NO-ACTION"
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
                RaiseEvent(
                    "Magix.MetaAction.RaiseAction",
                    e.Params);
            }
        }
    }
}





















