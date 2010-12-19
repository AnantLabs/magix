/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Viewports
{
    [ActiveModule]
    public class SingleContainer : UserControl
    {
        protected DynamicPanel dyn;
        protected DynamicPanel dyn2;
        protected Window message;
        protected Label msgLbl;
        protected Panel pnlAll;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.LoadComplete += Page_LoadComplete;
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            if (!HaveCheckedChrome)
            {
                Node node = new Node();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "GetChromeAttributes",
                    node);
                if (node["WasSet"].Get<bool>())
                    HaveCheckedChrome = true;
                pnlAll.Style[Styles.fontFamily] = node["FontFamily"].Get<string>();
                pnlAll.Style[Styles.backgroundColor] = node["BackgroundColor"].Get<string>();
                pnlAll.Style[Styles.color] = node["Color"].Get<string>();
            }
        }

        private bool HaveCheckedChrome
        {
            get { return ViewState["HaveCheckedChrome"] == null ? false : (bool)ViewState["HaveCheckedChrome"]; }
            set { ViewState["HaveCheckedChrome"] = value; }
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
            if (e.Params["Position"].Get<string>() == "dyn")
            {
                ClearControls(dyn);
            }
            else if (e.Params["Position"].Get<string>() == "dyn2")
            {
                ClearControls(dyn2);
            }
        }

        private void ClearControls(DynamicPanel dynamic)
        {
            foreach (Control idx in dynamic.Controls)
            {
                ActiveEvents.Instance.RemoveListener(idx);
            }
            dynamic.ClearControls();
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "dyn")
            {
                if (true.Equals(e.Params["Parameters"]["Append"].Value))
                    dyn.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(dyn);
                    dyn.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
            else if (e.Params["Position"].Get<string>() == "dyn2")
            {
                if (true.Equals(e.Params["Parameters"]["Append"].Value))
                    dyn2.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(dyn2);
                    dyn2.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        protected void dynamic_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
        {
            DynamicPanel dynamic = sender as DynamicPanel;
            Control ctrl = PluginLoader.Instance.LoadActiveModule(e.Key);
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
    }
}



