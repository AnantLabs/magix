/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.Users
{
    // TODO: Put into 'Commons' ...?
    // Hehe ... ;)
    /**
     * Shows a login box with username/password/openid combo. Username/password has preference, but
     * if only OpenID is given, the system will attempt at login you in using the OpenID claim. Note,
     * this is not the Publisher Plugin login box. This is the 'main system one', which you get by
     * going to ?dashboard=true
     */
    [ActiveModule]
    public class Login : ActiveModule
    {
        protected TextBox username;
        protected TextBox password;
        protected TextBox openID;
        protected Label err;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(username))
                        .Render();

                    // TODO: Clean up this, once and for all ... :(
                    Page.Form.Action = Request.Url.ToString().Replace("default.aspx", "");
                };
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            if (!string.IsNullOrEmpty(username.Text) &&
                !string.IsNullOrEmpty(password.Text))
            {
                node["Username"].Value = username.Text;
                node["Password"].Value = password.Text;
            }
            else if (!string.IsNullOrEmpty(username.Text))
            {
                node["Username"].Value = username.Text;
            }
            else
            {
                node["OpenID"].Value = openID.Text;
            }

            RaiseSafeEvent(
                "Magix.Core.LogInUser",
                node);

            if (!node["Success"].Get<bool>())
            {
                Node n = new Node();
                n["Message"].Value = "Sorry, no access ...";

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    n);

                username.Select();
                username.Focus();
            }
        }
    }
}
