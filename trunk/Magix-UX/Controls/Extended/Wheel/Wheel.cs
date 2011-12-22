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
     * Fancy SelectList for showing some additional bling
     */
    [ParseChildren(true, "Items")]
    public class Wheel : BaseWebControlListFormElement
    {
        /**
         * Raised whenever the selected value of your select list is changed.
         * You can also set the selected value in code through using for instance
         * the SelectedIndex property. Whenever a user changes the actively selected
         * item of your select list, this event will be raised.
         */
        public event EventHandler SelectedIndexChanged;

        public Wheel()
        {
            CssClass = "mux-wheel";
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("div"))
            {
                AddAttributes(el);
                bool first = true;
                foreach (ListItem idx in Items)
                {
                    using (Element l = builder.CreateElement("div"))
                    {
                        string xtra = "";
                        if (first)
                        {
                            xtra = " mux-first";
                            first = false;
                            int selectedIndex = SelectedIndex - 2; // Assuming 5 rows per column
                            l.AddAttribute("style", "margin-top:-" + selectedIndex * 18 + "px");
                        }
                        l.AddAttribute("class", "mux-item" + xtra);
                        l.Write(idx.Text);
                    }
                }
            }
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                case "selectedChanged":
                    RaiseChange(int.Parse(Page.Request.Params["__sel"]));
                    break;
                default:
                    base.RaiseEvent(name);
                    break;
            }
        }

        protected override void SetValue222222()
        {
        }

        protected void RaiseChange(int set)
        {
            SelectedIndex = set;
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, new EventArgs());
        }

        protected override string GetClientSideScriptType()
        {
            return "new MUX.Wheel";
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Timer),
                "Magix.UX.Js.Wheel.js");
            base.OnPreRender(e);
        }

        protected override string GetEventsRegisterScript()
        {
            string evts = base.GetEventsRegisterScript();
            if (SelectedIndexChanged != null)
            {
                if (evts.Length != 0)
                    evts += ",";
                evts += "['change']";
            }
            return evts;
        }
    }
}
