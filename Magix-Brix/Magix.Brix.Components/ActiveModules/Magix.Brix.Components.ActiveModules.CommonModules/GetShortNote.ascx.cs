/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
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
    public class GetShortNote : UserControl, IModule
    {
        protected TextArea txt;
        protected Button ok;
        protected Button cancel;

        private Node OkNode
        {
            get { return ViewState["OkNode"] as Node; }
            set { ViewState["OkNode"] = value; }
        }

        private Node CancelNode
        {
            get { return ViewState["CancelNode"] as Node; }
            set { ViewState["CancelNode"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                string message = node["Text"].Get<string>();
                txt.Text = message;
                OkNode = node["OK"].UnTie();
                CancelNode = node["Cancel"].UnTie();
                if (!CancelNode["Visible"].Get(true))
                {
                    cancel.Visible = false;
                    ok.CssClass = "cancel";
                }
                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(txt))
                    .Render();
            };
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            if (OkNode != null && OkNode["Event"].Value != null)
            {
                OkNode["Text"].Value = txt.Text;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    OkNode["Event"].Get<string>(),
                    OkNode);
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            if (CancelNode != null && CancelNode["Event"].Value != null)
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    CancelNode["Event"].Get<string>(),
                    CancelNode);
            }
            else
            {
                // Defaulting to emptying the top window in the 'child' hierarchy
                // Note, you can still override the "Cancel" node and point the event into oblivion
                // to avoid this behavior, it's just not the *default* behavior ...
                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }
    }
}



