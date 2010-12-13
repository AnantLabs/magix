/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Magic.UX.Builder;
using Magic.UX.Widgets.Core;
using Magic.UX.Helpers;

namespace Magic.UX.Widgets
{
    /**
     * A CheckBox is a 'two state button' which you can turn 'on' and 'off'. Useful
     * for boolean UI situations where use must choose between two options, for
     * instance 'yes' or 'no' situations.
     */
    public class CheckBox : BaseWebControlFormElement
    {
        /**
         * Event raised when the checked state of the widget changes. Use the
         * Checked property to determine if the CheckBox is 'on' or 'off'.
         */
        public event EventHandler CheckedChanged;

        /**
         * Use this property to determine if the widget is checked or not. 
         * If this property is true, then the widget is checked. The default
         * value is 'false'. You can also set this value in your code to change 
         * the state of the checked value.
         */
        public bool Checked
        {
            get { return ViewState["Checked"] == null ? false : (bool)ViewState["Checked"]; }
            set
            {
                if (value != Checked)
                    SetJsonGeneric("checked", value ? "checked" : "");
                ViewState["Checked"] = value;
            }
        }

        protected override void SetValue()
        {
            bool valueOfChecked = Page.Request.Params[ClientID] == "on";
            if (valueOfChecked != Checked)
            {
                // Notice that to avoid the string taking up bandwidth BACK to the client
                // which it obviously does not need to do we set the ViewState value here directly instead
                // of going through the Checked property which will also modify the JSON collection
                ViewState["Checked"] = valueOfChecked;
            }
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                // Due to a bug in IE we need to trap click event in DOM instead of change if
                // browser is IE
                case "click":
                case "change":
                    if (CheckedChanged != null)
                        CheckedChanged(this, new EventArgs());
                    break;
                default:
                    base.RaiseEvent(name);
                    break;
            }
        }

        protected override string GetEventsRegisterScript()
        {
            string evts = base.GetEventsRegisterScript();
            if (CheckedChanged != null)
            {
                evts = StringHelper.ConditionalAdd(
                    evts,
                    "",
                    ",",
                    "['change']");
            }
            return evts;
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("input"))
            {
                AddAttributes(el);
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("type", "checkbox");
            el.AddAttribute("name", ClientID);
            if (Checked)
                el.AddAttribute("checked", "checked");
            base.AddAttributes(el);
        }
    }
}
