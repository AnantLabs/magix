/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.UX.Aspects;
using Magix.Brix.Types;
using Magix.Brix.Loader;

[assembly: WebResource("Magix.Brix.Viewports.iscroll.js", "text/javascript")]

namespace Magix.Brix.Viewports
{
    [ActiveModule]
    public class SingleContainer : UserControl
    {
        protected DynamicPanel content1;
        protected DynamicPanel content2;
        protected DynamicPanel content3;
        protected DynamicPanel content4;
        protected DynamicPanel content5;
        protected Window message;
        protected Label msgLbl;
        protected Panel pnlAll;
        protected Panel wrp;
        protected Window[] wnd;
        protected DynamicPanel[] child;
        protected AspectSmartScroll scroll;
        protected Image ajaxWait;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();

            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(SingleContainer),
                "Magix.Brix.Viewports.iscroll.js",
                false);
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
                w.Style[Styles.zIndex] = (1000 + (idxNo * 8)).ToString();
                w.Visible = false;
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

        private bool first = true;

        [ActiveEvent(Name = "Magix.Core.AddCustomCssFile")]
        protected void Magix_Core_AddCustomCssFile(object sender, ActiveEventArgs e)
        {
            string cssFile = e.Params["CSSFile"].Get<String>();
            if (AjaxManager.Instance.IsCallback)
                throw new ApplicationException(
                    "Sorry, no support for setting custom CSS files in Ajax Callbacks (yet!)");
            if (!string.IsNullOrEmpty(cssFile))
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format(@"
<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />
",
                    cssFile);
                Page.Header.Controls.Add(lit);
            }
        }

        [ActiveEvent(Name = "Magix.Core.ShowMessage")]
        protected void Magix_Core_ShowMessage(object sender, ActiveEventArgs e)
        {
            if (first)
            {
                msgLbl.Text = "";
                new EffectFadeIn(message, 250)
                    .JoinThese(new EffectRollDown())
                    .ChainThese(
                        new EffectTimeout(2500),
                        new EffectFadeOut(message, 250)
                            .JoinThese(new EffectRollUp()))
                    .Render();
                first = false;
            }
            msgLbl.Text += string.Format("<p>{0}</p>", e.Params["Message"].Get<string>());
        }

        [ActiveEvent(Name = "ClearControls")]
        protected void ClearControls(object sender, ActiveEventArgs e)
        {
            string container = e.Params["Position"].Value as string;
            if (string.IsNullOrEmpty(container))
                container = "content1";
            if (container.StartsWith("content"))
            {
                DynamicPanel pnl = 
                    Selector.FindControl<DynamicPanel>(
                    this, 
                    container);
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

        [ActiveEvent(Name = "Magix.Core.SetBodyStyles")]
        private void SetStyles(object sender, ActiveEventArgs e)
        {
            string bgcolor = e.Params["BackgroundColor"].Get<string>();
            string color = e.Params["Color"].Get<string>();
            if (!string.IsNullOrEmpty(color))
            {
                wrp.Style[Styles.color] = color;
            }
            if (!string.IsNullOrEmpty(bgcolor))
            {
                wrp.Style[Styles.backgroundColor] = bgcolor;
            }
        }

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            string cssClass = null;

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

                if (dyn.ID == "content3")
                {
                    // These are normally grouped together with different "blocks" of
                    // applications ...
                    ClearControls(content4);
                    ClearControls(content5);
                }

                if (dyn.Controls.Count == 0 || 
                    !e.Params["Parameters"].Contains("Append") ||
                    !e.Params["Parameters"]["Append"].Get<bool>())
                {
                    if (e.Params["Parameters"].Contains("ParentIsRelative"))
                    {
                        wrp.Style[Styles.position] = "relative";
                    }
                    else
                    {
                        wrp.Style[Styles.position] = "";
                    }
                    if (e.Params["Parameters"].Contains("Padding"))
                    {
                        cssClass += " prepend-" + e.Params["Parameters"]["Padding"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("CssClass"))
                    {
                        cssClass += " " + e.Params["Parameters"]["CssClass"].Get<string>();
                    }
                    if (e.Params["Parameters"].Contains("Width"))
                    {
                        cssClass += " span-" + e.Params["Parameters"]["Width"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Height"))
                    {
                        cssClass += " height-" + e.Params["Parameters"]["Height"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Top"))
                    {
                        cssClass += " down-" + e.Params["Parameters"]["Top"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Pull"))
                    {
                        cssClass += " pull-" + e.Params["Parameters"]["Pull"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Push"))
                    {
                        cssClass += " push-" + e.Params["Parameters"]["Push"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Absolute"))
                    {
                        cssClass += " absolutized";
                    }
                    if (e.Params["Parameters"].Contains("Clear"))
                    {
                        cssClass += " cleared";
                    }
                    if (e.Params["Parameters"].Contains("Relativize"))
                    {
                        cssClass += " relativized";
                    }
                    if (e.Params["Parameters"].Contains("Last"))
                    {
                        cssClass += " last";
                    }
                    if (e.Params["Parameters"].Contains("Overflow"))
                    {
                        cssClass += " overflowized";
                    }
                    if (string.IsNullOrEmpty(cssClass))
                    {
                        // Defaulting to down-1 ...
                        cssClass = "down-1";
                    }
                    dyn.CssClass = cssClass.Trim();
                    if (AjaxManager.Instance.IsCallback)
                    {
                        dyn.Style[Styles.display] = "none";
                        new EffectFadeIn(dyn, 750)
                            .Render();
                    }
                    else
                    {
                        dyn.Style[Styles.display] = "";
                    }
                }

                if (e.Params["Parameters"].Contains("Append") &&
                    e.Params["Parameters"]["Append"].Get<bool>())
                {
                    dyn.AppendControl(
                        e.Params["Name"].Value.ToString(), 
                        e.Params["Parameters"]);

                    if (AjaxManager.Instance.IsCallback)
                    {
                        // We highlight our newly injected module's container here ...!
                        new EffectHighlight(dyn, 500)
                            .Render();
                    }
                }
                else
                {
                    ClearControls(dyn);
                    dyn.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
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

                    // Resetting any previous CSS classes ...
                    w.CssClass = "mux-shaded mux-rounded mux-window child";
                    
                    // Adding child CSS class(es)
                    if (e.Params["Parameters"].Contains("WindowCssClass"))
                    {
                        w.CssClass = e.Params["Parameters"]["WindowCssClass"].Get<string>();
                    }

                    // Background color ...?
                    if (e.Params["Parameters"].Contains("BackgroundColor"))
                    {
                        string bg = e.Params["Parameters"]["BackgroundColor"].Get<string>();
                        toAddInto.Style[Styles.backgroundColor] = bg;
                    }
                    else
                    {
                        toAddInto.Style[Styles.backgroundColor] = "";
                    }
                    if (e.Params["Parameters"].Contains("ToolTip"))
                    {
                        string tooltip = e.Params["Parameters"]["ToolTip"].Get<string>();
                        toAddInto.ToolTip = tooltip;
                    }
                    else
                    {
                        toAddInto.ToolTip = "";
                    }
                    if (e.Params["Parameters"].Contains("DynCssClass"))
                    {
                        string cssClass2 = e.Params["Parameters"]["DynCssClass"].Get<string>();
                        toAddInto.CssClass += " " + cssClass2;
                    }
                    else
                    {
                        toAddInto.CssClass = "dynamic";
                    }

                    if (e.Params["Parameters"].Contains("Padding"))
                    {
                        cssClass += " prepend-" + e.Params["Parameters"]["Padding"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Top"))
                    {
                        cssClass += " down-" + e.Params["Parameters"]["Top"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Pull"))
                    {
                        cssClass += " pull-" + e.Params["Parameters"]["Pull"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Push"))
                    {
                        cssClass += " push-" + e.Params["Parameters"]["Push"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("Width"))
                    {
                        cssClass += " span-" + e.Params["Parameters"]["Width"].Get<int>();
                    }
                    if (e.Params["Parameters"].Contains("last"))
                    {
                        cssClass += " last";
                    }
                    if (e.Params["Parameters"].Contains("NoClose"))
                        w.Closable = false;
                    else
                        w.Closable = true;
                    if (!string.IsNullOrEmpty(cssClass))
                    {
                        w.Style[Styles.left] = "";
                        w.Style[Styles.top] = "";
                        w.Style[Styles.width] = "";
                        w.Style[Styles.height] = "";
                        w.CssClass += " " + cssClass.Trim();
                    }
                    else
                    {
                        w.Style[Styles.width] = "auto";
                        w.Style[Styles.height] = "auto";
                    }

                    // Animation ...?
                    if (e.Params["Parameters"].Contains("ForcedSize"))
                    {
                        if (e.Params["Parameters"]["ForcedSize"].Contains("height"))
                        {
                            int width = e.Params["Parameters"]["ForcedSize"]["width"].Get<int>();
                            int height = e.Params["Parameters"]["ForcedSize"]["height"].Get<int>();
                            w.Style[Styles.width] = "800px";
                            w.Style[Styles.height] = "500px";
                            if (AjaxManager.Instance.IsCallback)
                            {
                                new EffectFadeIn(w, 750)
                                    .JoinThese(
                                        new EffectSize(width, height))
                                    .Render();
                            }
                            else
                            {
                                w.Style[Styles.display] = "";
                                new EffectSize(w, 750, width, height)
                                    .Render();
                            }
                        }
                        else
                        {
                            int width = e.Params["Parameters"]["ForcedSize"]["width"].Get<int>();
                            w.Style[Styles.width] = width + "px";
                            if (AjaxManager.Instance.IsCallback)
                            {
                                new EffectFadeIn(w, 750)
                                    .JoinThese(
                                        new EffectRollDown())
                                    .Render();
                            }
                            else
                            {
                                w.Style[Styles.display] = "";
                            }
                        }
                    }
                    else
                    {
                        if (e.Params["Parameters"].Contains("Width"))
                        {
                            cssClass += " span-" + e.Params["Parameters"]["Width"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("Height"))
                        {
                            cssClass += " height-" + e.Params["Parameters"]["Height"].Get<int>();
                        }
                        if (AjaxManager.Instance.IsCallback)
                        {
                            new EffectFadeIn(w, 750)
                                .JoinThese(
                                    new EffectRollDown())
                                .Render();
                        }
                        else
                        {
                            w.Style[Styles.display] = "";
                        }
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
                        e.Params["Parameters"]);
                }
                else
                {
                    ClearControls(toAddInto);
                    toAddInto.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        [ActiveEvent(Name = "Magix.Core.SetAjaxWaitImage")]
        protected void Magix_Core_SetAjaxWaitImage(object sender, ActiveEventArgs e)
        {
            ajaxWait.ImageUrl = e.Params["Image"].Get<string>();
        }

        [ActiveEvent(Name = "Magix.Core.SetViewPortSize")]
        protected void Magix_Core_SetViewPortSize(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("Width"))
            {
                if (!e.Params["NoCss"].Get<bool>())
                {
                    if (e.Params["CSSWidth"].Value != null)
                    {
                        wrp.Style[Styles.width] = e.Params["CSSWidth"].Value.ToString();
                        wrp.Style[Styles.marginRight] = "auto !important";
                        wrp.Style[Styles.marginLeft] = "auto !important";
                    }
                    else
                    {
                        wrp.Style[Styles.width] = e.Params["Width"].Value.ToString();
                        wrp.Style[Styles.marginRight] = "auto !important";
                        wrp.Style[Styles.marginLeft] = "auto !important";
                    }
                }
                string contr =
                    string.Format(@"
<meta name=""viewport"" content=""width={0}{1}{2}{3}"" />
<meta name=""apple-mobile-web-app-capable"" content=""yes"" />
<meta name=""apple-mobile-web-app-status-bar-style"" content=""black"" />
<link rel=""apple-touch-icon"" href=""./media/images/icon.png"" />",
                e.Params["Width"].Value.ToString().Replace("px", ""),
                e.Params["InitialScale"].Value != null 
                    ? string.Format(", initial-scale={0}", e.Params["InitialScale"].Get<string>()) 
                    : "",
                e.Params["InitialScale"].Value == null ? ", user-scalable=no" : "",
                e.Params["MaxScale"].Value != null ? string.Format(", maximum-scale={0}", e.Params["MaxScale"].Value) : "");
                LiteralControl lit = new LiteralControl(contr);
                Page.Header.Controls.Add(lit);
            }
            if (e.Params.Contains("Height"))
            {
                wrp.Style[Styles.height] = e.Params["Height"].Value.ToString();
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



