/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.UX.Aspects;
using Magix.Brix.Types;
using Magix.Brix.Loader;

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
        protected Window[] wnd;
        protected DynamicPanel[] child;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateChildContainers();
        }

        private void CreateChildContainers()
        {
            wnd = new Window[20];
            child = new DynamicPanel[20];
            for (int idxNo = 0; idxNo < 20; idxNo++)
            {
                // Window
                Window w = new Window();
                w.CssClass = "mux-shaded mux-rounded window-left-buttons";
                w.Style[Styles.left] = ((idxNo * 25) + 10).ToString() + "px";
                w.Style[Styles.top] = ((idxNo * 30) + 10).ToString() + "px";
                w.Style[Styles.position] = "absolute";
                w.Style[Styles.minWidth] = "550px";
                w.Style[Styles.minHeight] = "250px";
                w.Style[Styles.zIndex] = (1000 + idxNo).ToString();
                w.Style[Styles.overflow] = "auto";
                w.Visible = false;
                w.ID = "wd" + idxNo;
                w.Closed += wnd_Closed;
                wnd[idxNo] = w;

                // Dynamic Panel
                DynamicPanel p = new DynamicPanel();
                p.CssClass = "dynamic";
                p.Reload += dynamic_LoadControls;
                p.ID = "dny" + idxNo;
                child[idxNo] = p;
                w.Content.Controls.Add(p);

                // Aspect Modal
                AspectModal m = new AspectModal();
                m.ID = "md" + idxNo;
                m.Opacity = 0.2M;
                w.Controls.Add(m);

                this.Controls.Add(w);
            }
        }

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
            else if (e.Params["Position"].Get<string>() == "child")
            {
                DynamicPanel toEmpty = child[0];
                foreach (DynamicPanel idxChild in child)
                {
                    if (idxChild.Controls.Count > 0)
                        toEmpty = idxChild;
                }
                ClearControls(toEmpty);
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
            if (e.Params["Position"].Get<string>() == "dyn" || 
                string.IsNullOrEmpty(e.Params["Position"].Get<string>()))
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
            else if (e.Params["Position"].Get<string>() == "child")
            {
                DynamicPanel toAddInto = null;
                foreach (DynamicPanel idx in child)
                {
                    if (idx.Controls.Count == 0)
                    {
                        toAddInto = idx;
                        break;
                    }
                }
                if (toAddInto == null)
                    throw new ApplicationException("You cannot open more Windows before you have closed some");
                Window w = toAddInto.Parent.Parent as Window;
                w.Visible = true;
                w.Style[Styles.display] = "none";
                new EffectFadeIn(w, 500)
                    .Render();
                if (true.Equals(e.Params["Parameters"]["Append"].Value))
                    toAddInto.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(toAddInto);
                    toAddInto.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        protected void wnd_Closed(object sender, EventArgs e)
        {
            Window w = sender as Window;
            ClearControls(w.Content.Controls[0] as DynamicPanel);
            int closingWindowID = int.Parse(w.ID.Replace("wd", ""));
            if (closingWindowID != 0)
            {
                int refreshWindowID = closingWindowID - 1;
                Node node = new Node();
                node["ClientID"].Value = w.ClientID.Replace(w.ID, "wd" + refreshWindowID);
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "RefreshWindowContent",
                    node);
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



