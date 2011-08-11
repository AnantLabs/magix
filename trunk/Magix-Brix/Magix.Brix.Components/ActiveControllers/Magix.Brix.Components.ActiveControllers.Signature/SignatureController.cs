/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Signature
{
    [ActiveController]
    public class SignatureController : ActiveController
    {
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
