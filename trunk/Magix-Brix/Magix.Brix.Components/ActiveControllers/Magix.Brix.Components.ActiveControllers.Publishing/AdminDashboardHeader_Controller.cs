/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: We'll 'lock'
     * the Header control since it can be VERY annoying sometimes due to the DB
     * Manager, which seriously needs to be refactored here BTW ...
     */
    [ActiveController]
    public class AdminDashboardHeader_Controller : ActiveController
    {
        /**
         * Level2: Basically just ensures the Header Menu is loaded, and locked with 
         * 'Administrator Dashboard' as its value.
         */
        [ActiveEvent(Name = "Magix.Publishing.LoadHeader")]
        protected void Magix_Publishing_LoadHeader(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("Caption"))
                e.Params["Caption"].Value = "Administrator Dashboard";

            // Checking to see if Header module is loaded, and if not, loading it ...
            DynamicPanel header = Selector.FindControl<DynamicPanel>(Page, "content2");

            if (header.Controls.Count == 0 ||
                header.Controls[0].GetType().FullName.IndexOf("_header") == -1)
            {
                Node node = new Node();

                node["Width"].Value = 18;
                node["Top"].Value = 1;
                node["Last"].Value = true;
                node["CssClass"].Value = "headerModule";

                LoadModule(
                    "Magix.Brix.Components.ActiveModules.CommonModules.Header",
                    "content2",
                    node);
            }

            e.Params["Lock"].Value = true;
            RaiseEvent(
                "Magix.Core.SetFormCaption",
                e.Params);
        }
    }
}
