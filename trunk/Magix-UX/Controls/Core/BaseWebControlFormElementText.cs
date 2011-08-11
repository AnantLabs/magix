/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.ComponentModel;
using Magix.UX.Builder;

namespace Magix.UX.Widgets.Core
{
    /**
     * Abstract base class for WebControls which are BaseWebControlFormElement type of
     * controls, but also have a Text property. Text property is overridable, its 
     * default implementation sets the 'Value' property on the client-side, which
     * may or may not be suitable for your needs.
     */
    public abstract class BaseWebControlFormElementText : BaseWebControlFormElement
    {
        /**
         * The text that is displayed within the control, default value is string.Empty.
         * This is very often a string next to the widget which the user can read.
         */
        public virtual string Text
        {
            get { return ViewState["Text"] == null ? "" : (string)ViewState["Text"]; }
            set
            {
                if (value != Text)
                    SetJsonValue("Value", value);
                ViewState["Text"] = value;
            }
        }
    }
}
