﻿/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
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
    [ActiveModule]
    public class Header : UserControl, IModule
    {
        protected Label lbl;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
            };
        }

        private bool Lock
        {
            get { return ViewState["Lock"] == null ? false : (bool)ViewState["Lock"]; }
            set { ViewState["Lock"] = value; }
        }

        private string Popped
        {
            get { return ViewState["Popped"] as string; }
            set { ViewState["Popped"] = value; }
        }

        private int PoppedNo
        {
            get { return ViewState["PoppedNo"] == null ? 0 : (int)ViewState["PoppedNo"]; }
            set { ViewState["PoppedNo"] = value; }
        }

        [ActiveEvent(Name = "Magix.Core.UnlockFormCaption")]
        protected void Magix_Core_UnlockFormCaption(object sender, ActiveEventArgs e)
        {
            Lock = false;
            lbl.Text = Popped;
            lbl.Style[Styles.fontSize] = PoppedNo.ToString() + "px";
            lbl.Style[Styles.marginTop] = (36 - PoppedNo).ToString() + "px";
        }

        [ActiveEvent(Name = "Magix.Core.SetFormCaption")]
        protected void Magix_Core_SetFormCaption(object sender, ActiveEventArgs e)
        {
            if (Lock)
                return;
            Lock = e.Params.Contains("Lock") && e.Params["Lock"].Get<bool>();
            if (Lock)
            {
                Popped = lbl.Text;
                int size = 36;
                if (!string.IsNullOrEmpty(lbl.Style[Styles.fontSize]))
                    size = int.Parse(lbl.Style[Styles.fontSize].Replace("px", ""));
                PoppedNo = size;
            }
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



