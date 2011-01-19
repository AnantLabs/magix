/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
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
        protected DynamicPanel content1;
        protected DynamicPanel content2;
        protected DynamicPanel content3;
        protected DynamicPanel content4;
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
                w.CssClass = "mux-shaded mux-rounded mux-window child";
                w.Style[Styles.left] = ((idxNo % 12) * 20).ToString() + "px";
                w.Style[Styles.top] = (idxNo * 18).ToString() + "px";
                w.Style[Styles.minWidth] = "470px";
                w.Style[Styles.zIndex] = (1000 + (idxNo * 2)).ToString();
                w.Visible = false;
                w.Style[Styles.overflow] = "hidden";
                w.Style[Styles.position] = "absolute";
                w.EscKey +=
                    delegate
                    {
                        w.CloseWindow();
                    };
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
                m.Opacity = 0.3M;
                w.Controls.Add(m);

                pnlAll.Controls.Add(w);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            msgLbl.Text = "";
        }

        [ActiveEvent(Name = "Magix.Core.ShowMessage")]
        protected void Magix_Core_ShowMessage(object sender, ActiveEventArgs e)
        {
            msgLbl.Text += e.Params["Message"].Get<string>();
            new EffectFadeIn(message, 500)
                .JoinThese(new EffectRollDown())
                .ChainThese(
                    new EffectTimeout(5000),
                    new EffectFadeOut(message, 500)
                        .JoinThese(new EffectRollUp()))
                .Render();
        }

        [ActiveEvent(Name = "ClearControls")]
        protected void ClearControls(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>().StartsWith("content"))
            {
                DynamicPanel pnl = 
                    Selector.FindControl<DynamicPanel>(
                    this, 
                    e.Params["Position"].Get<string>());
                ClearControls(pnl);
                if (pnl.ID == "content3")
                {
                    // These two are normally grouped together ...
                    ClearControls(content4);
                }
            }
            else if (e.Params["Position"].Get<string>() == "child")
            {
                DynamicPanel toEmpty = child[0];
                foreach (DynamicPanel idxChild in child)
                {
                    if (idxChild.Controls.Count > 0)
                        toEmpty = idxChild;
                }
                (toEmpty.Parent.Parent as Window).CloseWindow();
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
            bool insertAtBeginning = e.Params["Parameters"].Contains("InsertAtBeginning") &&
                e.Params["Parameters"]["InsertAtBeginning"].Get<bool>();

            // Since this is our default container, 
            // we accept "null" and string.Empty values here ...!
            if (string.IsNullOrEmpty(e.Params["Position"].Get<string>()) ||
                e.Params["Position"].Get<string>().IndexOf("content") == 0)
            {
                if (string.IsNullOrEmpty(e.Params["Position"].Get<string>()))
                    e.Params["Position"].Value = "content1"; // Defaulting to content1 ...

                DynamicPanel dyn = Selector.FindControl<DynamicPanel>(
                    this, 
                    e.Params["Position"].Get<string>());

                if (dyn.ID == "content2")
                {
                    // These two are normally grouped together ...
                    ClearControls(content4);
                }

                bool hasCssClass = false;
                if (dyn.Controls.Count == 0)
                {
                    string cssClass = null;
                    if (e.Params["Parameters"].Contains("Padding"))
                    {
                        cssClass += " push-" + e.Params["Parameters"]["Padding"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Width"))
                    {
                        cssClass += " span-" + e.Params["Parameters"]["Width"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Top"))
                    {
                        cssClass += " down-" + e.Params["Parameters"]["Top"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Height"))
                    {
                        cssClass += " height-" + e.Params["Parameters"]["Height"].Get<int>();
                    }
                    if (!string.IsNullOrEmpty(cssClass))
                    {
                        hasCssClass = true;
                    }
                    else
                    {
                        // Defaulting to down-1 ...
                        cssClass = "down-1";
                    }
                    cssClass += " last";
                    dyn.CssClass = cssClass.Trim();
                }

                if (e.Params["Parameters"].Contains("Append") &&
                    e.Params["Parameters"]["Append"].Get<bool>())
                {
                    dyn.AppendControl(
                        e.Params["Name"].Value.ToString(), 
                        e.Params["Parameters"], 
                        insertAtBeginning);

                    // We highlight our newly injected module here ...!
                    new EffectHighlight(dyn, 500)
                        .Render();
                }
                else
                {
                    ClearControls(dyn);
                    dyn.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);

                    // We do NOT do any "fade in" effects if we're in append mode here ...
                    dyn.Style[Styles.display] = "none";
                    new EffectFadeIn(dyn, 500)
                        .Render();
                }
            }
            else if (e.Params["Position"].Get<string>() == "child")
            {
                DynamicPanel toAddInto = null;
                DynamicPanel last = child[0];
                foreach (DynamicPanel idx in child)
                {
                    if (idx.Controls.Count == 0)
                    {
                        toAddInto = idx;
                        break;
                    }
                    else
                        last = idx;
                }

                bool isAppending = e.Params["Parameters"].Contains("Append") &&
                    e.Params["Parameters"]["Append"].Get<bool>();

                // Now idx toAddInto is the first Dynamic *without* children,
                // while the last is the last one *with* children, or the *first* one
                // if none have children ...
                if (toAddInto == null && !isAppending)
                    throw new ApplicationException("You cannot open more Windows before you have closed some");
                if (isAppending)
                    toAddInto = last;
                Window w = toAddInto.Parent.Parent as Window;
                w.Visible = true;
                if (!isAppending)
                {
                    // We don't want to have race conditions for effects and caption
                    // of our window .....!
                    w.Style[Styles.display] = "none";
                    if (e.Params["Parameters"].Contains("WindowCssClass"))
                    {
                        w.CssClass = e.Params["Parameters"]["WindowCssClass"].Get<string>();
                    }
                    if (e.Params["Parameters"].Contains("ForcedSize"))
                    {
                        if (e.Params["Parameters"]["ForcedSize"].Contains("height"))
                        {
                            int width = e.Params["Parameters"]["ForcedSize"]["width"].Get<int>();
                            int height = e.Params["Parameters"]["ForcedSize"]["height"].Get<int>();
                            w.Style[Styles.width] = "800px";
                            w.Style[Styles.height] = "500px";
                            new EffectFadeIn(w, 750)
                                .JoinThese(
                                    new EffectSize(width, height))
                                .Render();
                        }
                        else
                        {
                            int width = e.Params["Parameters"]["ForcedSize"]["width"].Get<int>();
                            w.Style[Styles.width] = width + "px";
                            new EffectFadeIn(w, 750)
                                .JoinThese(
                                    new EffectRollDown())
                                .Render();
                        }
                    }
                    else
                    {
                        string cssClass = null;
                        if (e.Params["Parameters"].Contains("Padding"))
                        {
                            cssClass += " push-" + e.Params["Parameters"]["Padding"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("Width"))
                        {
                            cssClass += " span-" + e.Params["Parameters"]["Width"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("Top"))
                        {
                            cssClass += " down-" + e.Params["Parameters"]["Top"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("Height"))
                        {
                            cssClass += " height-" + e.Params["Parameters"]["Height"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("last"))
                        {
                            cssClass += " last";
                        }
                        if (!string.IsNullOrEmpty(cssClass))
                        {
                            w.Style[Styles.left] = "";
                            w.Style[Styles.top] = "";
                        }
                        if (!string.IsNullOrEmpty(cssClass))
                        {
                            w.CssClass += " " + cssClass.Trim();
                        }
                        else
                        {
                            w.Style[Styles.width] = "auto";
                            w.Style[Styles.height] = "auto";
                        }
                        new EffectFadeIn(w, 750)
                            .JoinThese(
                                new EffectRollDown())
                            .Render();
                    }
                    if (e.Params["Parameters"].Contains("Caption"))
                    {
                        w.Caption = e.Params["Parameters"]["Caption"].Get<string>();
                    }
                }
                if (e.Params["Parameters"].Contains("Append") &&
                    e.Params["Parameters"]["Append"].Get<bool>())
                {
                    toAddInto.AppendControl(
                        e.Params["Name"].Value.ToString(), 
                        e.Params["Parameters"],
                        insertAtBeginning);
                }
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
            if (closingWindowID == 0)
            {
                Node node = new Node();
                node["ClientID"].Value = "LastWindow";
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "RefreshWindowContent",
                    node);
            }
            else
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
            if (e.InsertAtBeginning)
                dynamic.Controls.AddAt(0, ctrl);
            else
                dynamic.Controls.Add(ctrl);
        }
    }
}



