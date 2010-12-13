/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magic.UX;
using Magic.UX.Widgets;
using Magic.Brix.Types;
using Magic.Brix.Loader;
using Magic.UX.Effects;

namespace Magic.Brix.Viewports
{
    [ActiveModule]
    public class Admin : UserControl, IModule
    {
        protected SlidingMenu sliding;
        protected DynamicPanel dynAdmin;
        protected DynamicPanel dynAdmin2;
        protected Window message;
        protected Label msgLbl;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    if (node["ClientID"].Value == null)
                    {
                        sliding.Visible = false;
                    }
                    else
                    {
                        ClientID = node["ClientID"].Get<int>();
                    }
                };
        }

        [ActiveEvent(Name = "ShowMessage")]
        protected void ShowMessage(object sender, ActiveEventArgs e)
        {
            msgLbl.Text = e.Params["Message"].Get<string>();
            new EffectFadeIn(message, 500)
                .JoinThese(new EffectRollDown())
                .ChainThese(
                    new EffectTimeout(3000),
                    new EffectFadeOut(message, 500)
                        .JoinThese(new EffectRollUp()))
                .Render();
        }

        [ActiveEvent(Name = "ClearControls")]
        protected void ClearControls(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "dynAdmin")
            {
                dynAdmin.ClearControls();
            }
            else if (e.Params["Position"].Get<string>() == "dynAdmin2")
            {
                dynAdmin2.ClearControls();
            }
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "dynAdmin")
            {
                if (true.Equals(e.Params["Parameters"]["Append"].Value))
                    dynAdmin.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                    dynAdmin.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
            }
            else if (e.Params["Position"].Get<string>() == "dynAdmin2")
            {
                if (true.Equals(e.Params["Parameters"]["Append"].Value))
                    dynAdmin2.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                    dynAdmin2.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
            }
        }

        protected void dynamic_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
        {
            DynamicPanel dynamic = sender as DynamicPanel;
            Control ctrl = PluginLoader.Instance.LoadControl(e.Key);
            if (e.FirstReload)
            {
                ctrl.Init +=
                    delegate
                    {
                        IModule module = ctrl as IModule;
                        if (module != null)
                        {
                            module.InitialLoading(e.Extra as Node);
                        }
                    };
            }
            dynamic.Controls.Add(ctrl);
        }

        private new int ClientID
        {
            get { return (int)ViewState["ClientID"]; }
            set { ViewState["ClientID"] = value; }
        }

        protected void sliding_LeafMenuItemClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = sender as SlidingMenuItem;
            switch (item.ID)
            {
                case "LoadDashboard":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "LoadDashboard");
                    break;
                case "CreateNewMenu":
                    Node node = new Node();
                    node["ClientID"].Value = ClientID;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "CreateNewMenu",
                        node);
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ViewAllMenus");
                    break;
                case "ViewAllMenus":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ViewAllMenus");
                    break;
                case "ViewRatings":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowAllRatings");
                    break;
                case "ViewExports":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowReportsAndExports");
                    break;
                case "ViewClubMembers":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ViewWineClubMembers");
                    break;
                case "EditEmailTemplate":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "EditEmailTemplate");
                    break;
                case "Settings":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "EditClientSettings");
                    break;
                case "LogOut":
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "LogoutUser");
                    break;
            }
        }
    }
}



