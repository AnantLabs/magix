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
     * Level4: Widget encapsulating an HTML5 drag and drop File Uploader control,
     * with progree bar and support for multiple files
     */
    public class Uploader : BaseWebControl
    {
        /**
         * Level4: Fired when File is uploaded. Access the file in raw [BASE64 encoded] through
         * the GetFileRawBASE64
         */
        public event EventHandler Uploaded;

        string _fileCache = null;
        /**
         * Level4: Returns the raw BASE64 encoded information about the file
         */
        public string GetFileRawBASE64()
        {
            if (string.IsNullOrEmpty(Page.Request["__FILE"]))
                throw new ArgumentException("No files in Uploader");

            if (_fileCache == null)
            {
                _fileCache = Page.Request["__FILE"];
                _fileCache = _fileCache.Substring(_fileCache.IndexOf(",") + 1);
            }

            return _fileCache;
        }

        /**
         * Level4: Returns the file's client side filename
         */
        public string GetFileName()
        {
            if (string.IsNullOrEmpty(Page.Request["__FILE"]))
                throw new ArgumentException("No files in Uploader");

            return Page.Request["__FILENAME"];
        }

        public Uploader()
        {
            CssClass = "mux-file-uploader";
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Timer),
                "Magix.UX.Js.Uploader.js");
            base.OnPreRender(e);
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                case "uploaded":
                    if (Uploaded != null)
                        Uploaded(this, new EventArgs());
                    break;
            }
        }

		protected override string GetClientSideScriptType()
		{
			return "new MUX.Uploader";
		}

        protected override void AddAttributes(Element el)
        {
            base.AddAttributes(el);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("div"))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
                using (Element ul = builder.CreateElement("ul"))
                {
                    ul.AddAttribute("id", this.ClientID + "_ul");
                }
            }
        }
    }
}
