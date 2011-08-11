/*
 * Magix - A Web Application Framework for ASP.NET
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
    [ActiveModule]
    public class Login : UserControl, IModule
    {
        protected TextBox username;
        protected TextBox password;
        protected TextBox openID;
        protected Label err;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(username))
                        .Render();
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

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.LogInUser",
                node);

            if (!node["Success"].Get<bool>())
            {
                Node n = new Node();
                n["Message"].Value = "Sorry, no access ...";

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    n);

                username.Select();
                username.Focus();
            }
        }
    }
}



