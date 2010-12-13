/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magic.UX.Builder;
using Magic.UX.Widgets.Core;

namespace Magic.UX.Widgets
{
    /**
     * A clickable button. The equivalent of &lt;input type="button". Use when you
     * need a clickable thing to resemble a button. See also the LinkButton for  
     * an alternative. Also remember that any Widget in Magic UX can be made
     * clickable, so you can also use a Label as your 'clickable thingie' if you wish.
     * Even though anything can be made clickable in Magic UX, it is often an
     * advantage to use buttons or link buttons since these elements will mostly
     * be recognized by screen readers and such, and it is hence more 'polite'
     * to use these specially designed types of 'clickable objects' such as the 
     * Button.
     */
    public class Button : BaseWebControlFormElementText
    {
        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("input"))
            {
                AddAttributes(el);
            }
        }

        protected override void SetValue()
        { }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("type", "button");
            el.AddAttribute("value", Text);
            base.AddAttributes(el);
        }
    }
}
