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
    public class MessageBox : ActiveModule, IModule
    {
        protected Label lbl;
        protected Button ok;
        protected Button cancel;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load += delegate
            {
                string message = node["Text"].Get<string>();
                if (!message.Contains("<p"))
                {
                    message = "<p>" + message + "</p>";
                }
                lbl.Text = message;
                if (!DataSource["Cancel"]["Visible"].Get(true))
                {
                    cancel.Visible = false;
                    ok.CssClass = "cancel";
                }
                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(ok))
                    .Render();
            };
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            if (DataSource.Contains("OK") &&
                DataSource["OK"].Contains("Event"))
            {
                RaiseSafeEvent(
                    DataSource["OK"]["Event"].Get<string>(),
                    DataSource["OK"]);
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            if (DataSource.Contains("Cancel") &&
                DataSource["Cancel"].Contains("Event"))
            {
                RaiseSafeEvent(
                    DataSource["Cancel"]["Event"].Get<string>(),
                    DataSource["Cancel"]);
            }
        }
    }
}



