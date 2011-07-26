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
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    [PublisherPlugin]
    public class LogInOutUser : ActiveModule
    {
        protected Panel pnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindControls();
        }

        private void DataBindControls()
        {
            Node n = new Node();
            RaiseSafeEvent(
                "Magix.Publishing.GetStateForLoginControl",
                n);
            if (n.Contains("ShouldLoadLogin") &&
                n["ShouldLoadLogin"].Get<bool>())
            {
                CreateLogInControls();
            }
            else if (n.Contains("ShouldLoadLogout") &&
                n["ShouldLoadLogout"].Get<bool>())
            {
                CreateLogOutControls();
            }
        }

        private void CreateLogOutControls()
        {
            Button b = new Button();
            b.ID = "loginBtn";
            b.CssClass = "logout-btn";
            b.Text = "Logout";
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    RaiseSafeEvent(
                        "Magix.Core.UserLoggedOut",
                        node);
                };
            pnl.Controls.Add(b);
        }

        private void CreateLogInControls()
        {
            TextBox t = new TextBox();
            t.PlaceHolder = "Username/OpenID ...";
            t.ID = "unm";
            t.CssClass = "username";
            pnl.Controls.Add(t);

            TextBox p = new TextBox();
            p.PlaceHolder = "Password ...";
            p.ID = "pwd";
            p.TextMode = TextBox.TextBoxMode.Password;
            p.CssClass = "password";
            pnl.Controls.Add(p);

            Button b = new Button();
            b.ID = "loginBtn";
            b.CssClass = "login-btn";
            b.Text = "Login";
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    node["Username"].Value = t.Text;
                    node["Password"].Value = p.Text;

                    RaiseSafeEvent(
                        "Magix.Core.LogInUser",
                        node);

                    if (!node["Success"].Get<bool>())
                    {
                        Node n = new Node();
                        n["Message"].Value = "Sorry, no access ...";
                        n["IsError"].Value = true;

                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "Magix.Core.ShowMessage",
                            n);

                        t.Select();
                        t.Focus();
                    }
                };
            pnl.Controls.Add(b);
            pnl.DefaultWidget = b.ID;
        }

        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(Header).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;
            }
        }
    }
}



