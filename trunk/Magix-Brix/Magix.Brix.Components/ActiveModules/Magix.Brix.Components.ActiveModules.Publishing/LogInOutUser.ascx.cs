/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: PublisherPlugin containing most of the UI for allowing a user to login/out
     * of your website. Can be set into both OpenID mode and 'only native mode' or both.
     * Raises 'Magix.Publishing.GetStateForLoginControl' to determine the state of the module,
     * meaning if it should show both OpenID logic and native logic or only one of them.
     * Will raise 'Magix.Core.UserLoggedOut' if user logs out and 'Magix.Core.LogInUser'
     * if the user tries to log in. The 'Magix.Core.LogInUser' default implementation again
     * will raise 'Magix.Core.UserLoggedIn' if it succeeds. It'll pass in 'OpenID' if
     * user has chosen to log in with OpenID and 'Username'/'Password' if user choses
     * to login natively
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class LogInOutUser : ActiveModule
    {
        protected Panel pnl;

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
            b.CssClass = "mux-logout-btn";
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
            switch (LoginMode)
            {
                case "Both":
                    BothOpenIDAndNative();
                    break;
                case "Native":
                    OnlyNative();
                    break;
                case "OpenID":
                    OnlyOpenID();
                    break;
            }
        }

        private void OnlyNative()
        {
            TextBox t = new TextBox();
            t.PlaceHolder = "Username ...";
            t.ID = "unm";
            t.CssClass = "mux-username";
            pnl.Controls.Add(t);

            TextBox p = new TextBox();
            p.PlaceHolder = "Password ...";
            p.ID = "pwd";
            p.TextMode = TextBox.TextBoxMode.Password;
            p.CssClass = "mux-password";
            pnl.Controls.Add(p);

            Button b = new Button();
            b.ID = "loginBtn";
            b.CssClass = "mux-login-btn";
            b.Text = "Login";
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    if (string.IsNullOrEmpty(p.Text))
                    {
                        Node xm = new Node();
                        xm["Message"].Value = "Password please ...?";
                        RaiseSafeEvent(
                            "Magix.Core.ShowMessage",
                            xm);
                        return;
                    }

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
                    // If we have success, this is normally follwed instantly by a re-direct
                    // or tons of other events, so we don't really do anything particular here ...
                };
            pnl.Controls.Add(b);
            pnl.DefaultWidget = b.ID;
        }

        private void OnlyOpenID()
        {
            TextBox t = new TextBox();
            t.PlaceHolder = "OpenID ...";
            t.ID = "unm";
            t.CssClass = "username open-id";
            pnl.Controls.Add(t);

            System.Web.UI.WebControls.Button b = new System.Web.UI.WebControls.Button();
            b.ID = "loginBtn";
            b.CssClass = "mux-login-btn";
            b.Text = "Login";
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    node["OpenID"].Value = t.Text;

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
                    // If we have success, this is normally follwed instantly by a re-direct
                    // or tons of other events, so we don't really do anything particular here ...
                };
            pnl.Controls.Add(b);
            pnl.DefaultWidget = b.ID;
        }

        private void BothOpenIDAndNative()
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

            System.Web.UI.WebControls.Button b = new System.Web.UI.WebControls.Button();
            b.ID = "loginBtn";
            b.CssClass = "mux-login-btn";
            b.Text = "Login";
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    if (!string.IsNullOrEmpty(p.Text))
                    {
                        // Password given, assuming username/password combo
                        node["Username"].Value = t.Text;
                        node["Password"].Value = p.Text;
                    }
                    else
                    {
                        // No password given, Assuming OpenID ...
                        node["OpenID"].Value = t.Text;
                    }

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
                    // If we have success, this is normally follwed instantly by a re-direct
                    // or tons of other events, so we don't really do anything particular here ...
                };
            pnl.Controls.Add(b);
            pnl.DefaultWidget = b.ID;
        }

        /**
         * Level2: Will return false if this webpart can just be 'reused' to the next page
         */
        [ActiveEvent(Name = "Magix.Publishing.ShouldReloadWebPart")]
        protected void Magix_Publishing_ShouldReloadWebPart(object sender, ActiveEventArgs e)
        {
            if (e.Params["ModuleName"].Get<string>() == typeof(Header).FullName &&
                e.Params["Container"].Get<string>() == Parent.ID)
            {
                e.Params["Stop"].Value = true;
            }
        }

        /**
         * Level2: Which mode you wish to use for your login control. Legal values are
         * 'OpenID', 'Native' and 'Both'. Signifying the obvious, both being the default
         * which will allow you to log in either with username/password combination or
         * an OpenID claim
         */
        [ModuleSetting(DefaultValue = "Both", ModuleEditorEventName = "Magix.Publishing.GetTemplateColumnSelectTypeOfLoginControl")]
        public string LoginMode
        {
            get { return ViewState["LoginMode"] as string; }
            set { ViewState["LoginMode"] = value; }
        }

        /**
         * Level3: Implementation of 'Get Select Type Of Login Control' for Magix. Will 
         * return a Select Lst back to caller
         */
        [ActiveEvent(Name = "Magix.Publishing.GetTemplateColumnSelectTypeOfLoginControl")]
        protected static void Magix_Publishing_GetTemplateColumnSelectTypeOfLoginControl(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Value;

                    ActiveEvents.Instance.RaiseActiveEvent(
                        typeof(LogInOutUser),
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ls.Items.Add(new ListItem("Both", "Both"));
            ls.Items.Add(new ListItem("OpenID Only", "OpenID"));
            ls.Items.Add(new ListItem("Native Only", "Native"));
            switch (e.Params["Value"].Value.ToString())
            {
                case "Both":
                    ls.SelectedIndex = 0;
                    break;
                case "OpenID":
                    ls.SelectedIndex = 1;
                    break;
                case "Native":
                    ls.SelectedIndex = 2;
                    break;
                default:
                    ls.Enabled = false;
                    break;
            }
        }
    }
}
