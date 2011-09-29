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
     * Level2: Encapsulates a header [h1] control for you to create headers for your
     * pages and apps. Load it and raise 'Magix.Core.SetFormCaption' to set the
     * header
     */
    [ActiveModule]
    public class Header : UserControl
    {
        protected Label lbl;

        /**
         * Level2: Sets the caption of the header [h1] control
         */
        [ActiveEvent(Name = "Magix.Core.SetFormCaption")]
        protected void Magix_Core_SetFormCaption(object sender, ActiveEventArgs e)
        {
            string caption = e.Params["Caption"].Get<string>();
            lbl.Text = caption;

            if (e.Params.Contains("FontSize"))
            {
                lbl.Style[Styles.fontSize] = e.Params["FontSize"].Get<int>().ToString() + "px";
                lbl.Style[Styles.marginTop] = (36 - e.Params["FontSize"].Get<int>()).ToString() + "px";
            }
            else if (!string.IsNullOrEmpty(lbl.Style[Styles.fontSize]))
            {
                lbl.Style[Styles.fontSize] = "";
                lbl.Style[Styles.marginTop] = "";
            }
        }
    }
}
