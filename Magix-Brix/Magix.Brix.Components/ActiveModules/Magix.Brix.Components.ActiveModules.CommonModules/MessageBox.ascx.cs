/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
     * Level2: Implements logic for showing the end user a message box, asking for confirmation or
     * something similar when needed. 'Text' will be the text shown to the user, which he should
     * read and take a stand in regards to. Dropping the 'Cancel' Node sets Cancel to 
     * in-visible if you wish. 'OK/Event' will be raise with 'OK' node if OK button is clicked.
     * 'Cancel/Event' will be raised with 'Cancel' node if Cancel button is clicked
     */
    [ActiveModule]
    public class MessageBox : ActiveModule
    {
        protected Label lbl;
        protected Button ok;
        protected Button cancel;

        public override void InitialLoading(Node node)
        {
            if (!node.Contains("OK") || !node["OK"].Contains("Event"))
                throw new ArgumentException(@"You must pass in 'OK/Event' being a name of an
Event you wish to raise upon OK, otherwise the MessageBox won't make any meaning ... ");
            base.InitialLoading(node);
            Load += delegate
            {
                string message = node["Text"].Get<string>();
                if (!message.Contains("<p"))
                {
                    message = "<p>" + message + "</p>";
                }
                lbl.Text = message;
                if (!DataSource.Contains("Cancel") || 
                    !DataSource["Cancel"]["Visible"].Get(true))
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



