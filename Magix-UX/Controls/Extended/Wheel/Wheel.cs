/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;


namespace Magix.UX.Widgets
{
    /**
     */
    [ParseChildren(true, "Items")]
    public class Wheel : BaseWebControlListFormElement
    {
        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("div"))
            {
                AddAttributes(el);
                foreach (ListItem idx in Items)
                {
                    using (Element l = builder.CreateElement("option"))
                    {
                        l.AddAttribute("value", idx.Value);
                        if (!idx.Enabled)
                            l.AddAttribute("disabled", "disabled");
                        if (idx.Selected)
                            l.AddAttribute("selected", "selected");
                        l.Write(idx.Text);
                    }
                }
            }
        }
    }
}
