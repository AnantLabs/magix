/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Signature
{
    /**
     * Signature Plugin Control for Publishing system to allow for loading of a different
     * plugin module from e.g. the click of a row button or a single-view button etc.
     */
    [ActiveController]
    public class Signature_Controller : ActiveController
    {
        /**
         * Will load the Signature module, asking for Signature from the user and saving
         * associated with the record. Will replace the module that raised the Action
         * with the Signature module
         */
        [ActiveEvent(Name = "Magix.Signature.LoadSignature")]
        protected void Magix_Signature_LoadSignature(object sender, ActiveEventArgs e)
        {
            string container = null;

            if (!e.Params.Contains("Container") ||
                string.IsNullOrEmpty(e.Params["Container"].Get<string>()))
            {
                RaiseEvent(
                    "Magix.Core.GetContainerForControl",
                    e.Params);
                if (e.Params.Contains("Container"))
                    container = e.Params["Container"].Get<string>();
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Signature.Sign",
                container,
                e.Params);
        }
    }
}
