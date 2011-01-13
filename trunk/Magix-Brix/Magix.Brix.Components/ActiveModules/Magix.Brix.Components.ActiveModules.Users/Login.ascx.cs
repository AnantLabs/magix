/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveModules.Users
{
    [ActiveModule]
    public class Login : UserControl, IModule
    {
        protected TextBox username;
        protected TextBox password;
        protected Label err;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    username.Focus();
                    username.Select();
                    Page.Form.Action = Request.Url.ToString().Replace("default.aspx", "");
                };
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Username"].Value = username.Text;
            node["Password"].Value = password.Text;
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



