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
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;

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
        protected DynamicPanel content6;
        protected DynamicPanel content7;
        protected DynamicPanel fullScreen;
        protected Window message;
        protected Label msgLbl;
        protected Panel pnlAll;
        protected Panel wrp;
        protected Window[] wnd;
        protected DynamicPanel[] child;
        protected AspectSmartScroll scroll;
        protected Image ajaxWait;
        protected Panel debug;
        protected Timer timer;

        [DebuggerStepThrough]
        private bool IsDebug()
        {
            return Session["Magix.Core.IsDebug"] != null ?
                (bool)Session["Magix.Core.IsDebug"] :
                false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateChildContainers();
            ReCreateDebugControls();
            DebuggingEvents = new List<Tuple<string, Node>>();
        }

        private void ReCreateDebugControls()
        {
            if (IsDebug())
            {
                int idxNo = 0;
                foreach (Tuple<string, Node> idx in DebuggingEvents)
                {
                    LinkButton b = new LinkButton();
                    b.Text = idx.Left;
                    b.ID = "bb-" + idxNo;
                    b.CssClass = "clear-left span-6";
                    Tuple<string, Node> tmp = idx;
                    //b.ToolTip = idx.Right == null ? "" : idx.Right.ToJSONString();
                    b.Click += delegate(object sender, EventArgs e)
                    {
                        LinkButton b2 = sender as LinkButton;
                        Node node = new Node();
                        node["EventName"].Value = tmp.Left;
                        node["EventNode"].Value = tmp.Right;
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "Magix.Core.EventClickedWhileDebugging",
                            node);
                    };
                    debug.Controls.Add(b);
                    idxNo += 1;
                }
            }
        }

        private List<Tuple<string, Node>> DebuggingEvents
        {
            get
            {
                if (Session["Magix.Brix.DebuggingEvents"] == null)
                    Session["Magix.Brix.DebuggingEvents"] = new List<Tuple<string, Node>>();
                return Session["Magix.Brix.DebuggingEvents"] as List<Tuple<string, Node>>;
            }
            set
            {
                Session["Magix.Brix.DebuggingEvents"] = value;
            }
        }

        [ActiveEvent] // Null event handler for logging in debug cases ...
        [DebuggerStepThrough]
        protected void NULLEventHandler(object sender, ActiveEventArgs e)
        {
            if (IsDebug())
            {
                DebuggingEvents.Add(new Tuple<string, Node>(e.Name, e.Params));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (IsDebug())
            {
                if (!wrp.CssClass.Contains("showgrid"))
                    wrp.CssClass += " showgrid";
                debug.Visible = true;
            }
            else
            {
                if (wrp.CssClass.Contains(" showgrid"))
                    wrp.CssClass = wrp.CssClass.Replace(" showgrid", "");
                debug.Visible = false;
            }
            debug.Controls.Clear();
            ReCreateDebugControls();
            debug.ReRender();
            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Session["Magix.Core.ShowMessage-Delayed"] != null)
            {
                Magix_Core_ShowMessage(this, Session["Magix.Core.ShowMessage-Delayed"] as ActiveEventArgs);
                Session["Magix.Core.ShowMessage-Delayed"] = null;
            }

            message.ClickEffect = new EffectFadeOut(message, 250)
                .JoinThese(
                    new EffectRollUp());

            // Re-including all previously embedded CSS files
            // Need to preserve CSS files for those scenarios where 
            // you're doing a 'conventional postback' ...
            if (!AjaxManager.Instance.IsCallback && IsPostBack)
                IncludeAllCssFiles();

            HttpCookie cookie = Request.Cookies["UserID"];
            if (cookie == null)
            {
                // Creating new cookie, with Random GUID inside ...
                cookie = new HttpCookie("UserID", Guid.NewGuid().ToString());
                cookie.HttpOnly = true;
                cookie.Expires = DateTime.Now.AddYears(5);
                Page.Response.Cookies.Add(cookie);
                Node node = new Node();
                node["UserID"].Value = cookie.Value;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.NewUserIDCookieCreated",
                    node);
            }
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            // ORDER COUNTS!!!
            // In case of exceptions ...
            timer.Enabled = false;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                timer.Info);
        }

        private void IncludeAllCssFiles()
        {
            foreach (string idx in CssFiles)
            {
                IncludeCssFile(idx);
            }
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
                w.Style[Styles.position] = "absolute";
                w.Style[Styles.left] = (idxNo * 20).ToString() + "px";
                w.Style[Styles.top] = (idxNo * 36).ToString() + "px";
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

        private bool firstMessage = true;

        private List<string> CssFiles
        {
            get
            {
                if (ViewState["CssFiles"] == null)
                    ViewState["CssFiles"] = new List<string>();
                return ViewState["CssFiles"] as List<string>;
            }
        }

        private List<string> HeaderElements
        {
            get
            {
                if (ViewState["HeaderElements"] == null)
                    ViewState["HeaderElements"] = new List<string>();
                return ViewState["HeaderElements"] as List<string>;
            }
        }

        [ActiveEvent(Name = "Magix.Core.SetTitleOfPage")]
        protected void Magix_Core_SetTitleOfPage(object sender, ActiveEventArgs e)
        {
            string caption = e.Params["Caption"].Get<string>();
            if (string.IsNullOrEmpty(caption))
                throw new ArgumentException("Cannot set title to nothing. Try being more creative than such ... ;)");
            Page.Title = caption; // TODO: Implement support for changing Browser Title bar during Ajax Callbacks ...
        }

        [ActiveEvent(Name = "Magix.Core.SetViewPortContainerSettings")]
        protected void Magix_Core_SetViewPortContainerSettings(object sender, ActiveEventArgs e)
        {
            if (e.Params["Container"].Get<string>().StartsWith("content"))
            {
                // Ours ...
                DynamicPanel p = Selector.FindControl<DynamicPanel>(
                    this,
                    e.Params["Container"].Get<string>());
                if (p != null)
                {
                    p.CssClass = e.Params["CssClass"].Get<string>() + " web-part";
                    PutInSpan(e.Params, p, "Width", "span");
                    PutInSpan(e.Params, p, "Top", "down");
                    PutInSpan(e.Params, p, "MarginBottom", "spcBottom");
                    PutInSpan(e.Params, p, "PullTop", "pullTop");
                    PutInSpan(e.Params, p, "Height", "height");
                    PutInSpan(e.Params, p, "PushLeft", "pushLeft");
                    PutInSpan(e.Params, p, "PushRight", "pushRight");
                    PutInSpan(e.Params, p, "Padding", "prepend");
                    if(e.Params.Contains("Last") &&
                        e.Params["Last"].Get<bool>())
                        p.CssClass += " last";
                }
            }
        }

        private void PutInSpan(Node node, DynamicPanel p, string nodeName, string cssName)
        {
            if (node.Contains(nodeName))
                p.CssClass += " " + cssName + "-" + node[nodeName].Value.ToString();
        }

        [ActiveEvent(Name = "Magix.Core.GetViewPortSettings")]
        protected void Magix_Core_GetViewPortSettings(object sender, ActiveEventArgs e)
        {
            if (e.Params["Container"].Get<string>().StartsWith("content"))
            {
                // Ours ...
                DynamicPanel p = Selector.FindControl<DynamicPanel>(
                    this,
                    e.Params["Container"].Get<string>());
                if (p != null)
                {
                    ExtractSpan(e.Params, p, "Width", "span");
                    ExtractSpan(e.Params, p, "Top", "down");
                    ExtractSpan(e.Params, p, "MarginBottom", "spcBottom");
                    ExtractSpan(e.Params, p, "PullTop", "pullTop");
                    ExtractSpan(e.Params, p, "Height", "height");
                    ExtractSpan(e.Params, p, "PushLeft", "pushLeft");
                    ExtractSpan(e.Params, p, "PushRight", "pushRight");
                    ExtractSpan(e.Params, p, "Padding", "prepend");
                }
            }
        }

        private void ExtractSpan(Node node, DynamicPanel p, string nodeName, string cssName)
        {
            string css = p.CssClass;
            if (css.Contains(cssName))
            {
                string no = css.Split(new string[] { cssName }, StringSplitOptions.RemoveEmptyEntries)[1];
                no = no.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
                int value = int.Parse(no);
                node[nodeName].Value = value;
            }
        }

        [ActiveEvent(Name = "Magix.Core.TimeOut")]
        protected void Magix_Core_TimeOut(object sender, ActiveEventArgs e)
        {
            timer.Enabled = true;
            timer.Info = e.Params["EventName"].Get<string>();
        }

        [ActiveEvent(Name = "Magix.Core.AddLinkInHeader")]
        protected void Magix_Core_AddLinkInHeader(object sender, ActiveEventArgs e)
        {
            string headerStr = "<link";
            foreach (Node idx in e.Params)
            {
                headerStr += " " + idx.Name + "=\"" + idx.Get<string>() + "\"";
            }
            headerStr += " />";
            HeaderElements.Add(headerStr);
            IncludeHeaderFile(headerStr);
        }

        [ActiveEvent(Name = "Magix.Core.AddCustomCssFile")]
        protected void Magix_Core_AddCustomCssFile(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("CSSFile"))
            {
                string cssFile = e.Params["CSSFile"].Get<String>();
                CssFiles.Add(cssFile);
                IncludeCssFile(cssFile);
            }
            foreach (Node idx in e.Params)
            {
                if (idx.Name.IndexOf("CSSFile") == 0 &&
                    idx.Name != "CSSFile")
                {
                    // BEGINS with CSSFile, but is NOT CSSFile directly
                    // Meaning, will handle stuff such as "CSSFile1" and "CSSFile-Main" etc ...

                    // PS!
                    // Will add them in the order they were ADDED to the node, NOT the
                    // alphabetical order, numerical order or anything else ...
                    string cssFile = idx.Get<string>();
                    if (idx.Contains("Back") && idx["Back"].Get<bool>())
                    {
                        cssFile += "?back";
                    }
                    CssFiles.Add(cssFile);
                    IncludeCssFile(cssFile);
                }
            }
        }

        private void IncludeCssFile(string cssFile)
        {
            if (!string.IsNullOrEmpty(cssFile))
            {
                if (cssFile.Contains("~"))
                {
                    string appPath = HttpContext.Current.Request.Url.ToString();
                    appPath = appPath.Substring(0, appPath.LastIndexOf('/'));
                    cssFile = cssFile.Replace("~", appPath);
                }
                if (AjaxManager.Instance.IsCallback)
                {
                    AjaxManager.Instance.WriterAtBack.Write(
                        @"MUX.Element.prototype.includeCSS('<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />');", cssFile);
                }
                else
                {
                    LiteralControl lit = new LiteralControl();
                    lit.Text = string.Format(@"
<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />
",
                        cssFile);
                    Page.Header.Controls.Add(lit);
                }
            }
        }

        private void IncludeHeaderFile(string headerFile)
        {
            if (!string.IsNullOrEmpty(headerFile))
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format(@"
{0}
",
                    headerFile);
                Page.Header.Controls.Add(lit);
            }
        }

        [ActiveEvent(Name = "Magix.Core.ShowMessage")]
        protected void Magix_Core_ShowMessage(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("Delayed"))
            {
                // Postponing till 'next page cycle', probably due to refresh of page, while
                // still having something important to say ...
                e.Params["Delayed"].UnTie();
                Session["Magix.Core.ShowMessage-Delayed"] = e;
            }
            else
            {
                int timeOut = 2500;
                if (e.Params.Contains("Milliseconds"))
                    timeOut = e.Params["Milliseconds"].Get<int>();

                if (e.Params.Contains("IsError") &&
                    e.Params["IsError"].Get<bool>())
                {
                    message.CssClass += " error-message";
                }
                else
                {
                    message.CssClass = message.CssClass.Replace(" error-message", "");
                }

                if (firstMessage)
                {
                    msgLbl.Text = "";
                    new EffectFadeIn(message, 250)
                        .JoinThese(new EffectRollDown())
                        .ChainThese(
                            new EffectTimeout(timeOut),
                            new EffectFadeOut(message, 250)
                                .JoinThese(new EffectRollUp()))
                        .Render();
                    firstMessage = false;
                    if (e.Params.Contains("Header"))
                        message.Caption = e.Params["Header"].Get<string>();
                    else
                        message.Caption = "Message from Marvin ...";
                }
                msgLbl.Text += string.Format("<p>{0}</p>", e.Params["Message"].Get<string>());
            }
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
                ClearControls(pnl, true);
                if (pnl.ID == "content2")
                {
                    ClearControls(content3, true);
                    ClearControls(content4, true);
                    ClearControls(content5, true);
                    ClearControls(content6, true);
                    ClearControls(content7, true);
                }
                if (pnl.ID == "content3")
                {
                    ClearControls(content4, true);
                    ClearControls(content5, true);
                    ClearControls(content6, true);
                    ClearControls(content7, true);
                }
                else if (pnl.ID == "content4")
                {
                    ClearControls(content5, true);
                    ClearControls(content6, true);
                    ClearControls(content7, true);
                }
                else if (pnl.ID == "content5")
                {
                    ClearControls(content6, true);
                    ClearControls(content7, true);
                }
                else if (pnl.ID == "content6")
                {
                    ClearControls(content7, true);
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
                ClearControls(toEmpty, true);
            }
            else if (e.Params["Position"].Get<string>() == "fullScreen")
            {
                ClearControls(fullScreen, true);
                new EffectFadeOut(fullScreen, 500)
                    .Render();
            }
        }

        private void ClearControls(DynamicPanel dynamic, bool clearCss)
        {
            if (clearCss)
                dynamic.CssClass = "";
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

        [ActiveEvent(Name = "Magix.Core.GetNumberOfChildrenOfContainer")]
        protected void Magix_Core_GetNumberOfChildrenOfContainer(object sender, ActiveEventArgs e)
        {
            if (e.Params["Container"].Get<string>().StartsWith("content"))
            {
                e.Params["Count"].Value = 
                    Selector.FindControl<DynamicPanel>(
                        this, 
                        e.Params["Container"].Get<string>()).Controls.Count;
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

                if (!e.Params["Parameters"].Contains("Append") || !e.Params["Parameters"]["Append"].Get<bool>())
                    ClearControls(dyn, !e.Params["Parameters"].Contains("FreezeContainer") ||
                        !e.Params["Parameters"]["FreezeContainer"].Get<bool>());

                if (dyn.ID == "content3")
                {
                    // These are normally grouped together with different "blocks" of
                    // applications ...
                    if (!(e.Params["Parameters"].Contains("FREEZE4") && e.Params["Parameters"]["FREEZE4"].Get<bool>()))
                    {
                        ClearControls(content4, true);
                        e.Params["FREEZE4"].UnTie(); // To be sure ...!
                    }
                    if (!(e.Params["Parameters"].Contains("FREEZE5") && e.Params["Parameters"]["FREEZE5"].Get<bool>()))
                    {
                        ClearControls(content5, true);
                        e.Params["FREEZE5"].UnTie(); // To be sure ...!
                    }
                    if (!(e.Params["Parameters"].Contains("FREEZE6") && e.Params["Parameters"]["FREEZE6"].Get<bool>()))
                    {
                        ClearControls(content6, true);
                        e.Params["FREEZE6"].UnTie(); // To be sure ...!
                    }
                    if (!(e.Params["Parameters"].Contains("FREEZE7") && e.Params["Parameters"]["FREEZE7"].Get<bool>()))
                    {
                        ClearControls(content7, true);
                        e.Params["FREEZE7"].UnTie(); // To be sure ...!
                    }
                }

                if (dyn.Controls.Count == 0)
                {
                    if (e.Params["Parameters"].Contains("ClearBoth") && e.Params["Parameters"]["ClearBoth"].Get<bool>())
                    {
                        dyn.Style[Styles.clear] = "both";
                    }
                    else
                    {
                        dyn.Style[Styles.clear] = "";
                    }
                }

                if (dyn.Controls.Count == 0 || 
                    !e.Params["Parameters"].Contains("Append") ||
                    !e.Params["Parameters"]["Append"].Get<bool>())
                {
                    if (!e.Params["Parameters"].Contains("FreezeContainer") ||
                        !e.Params["Parameters"]["FreezeContainer"].Get<bool>())
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
                        if (e.Params["Parameters"].Contains("PushRight"))
                        {
                            cssClass += " pushRight-" + e.Params["Parameters"]["PushRight"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("PushLeft"))
                        {
                            cssClass += " pushLeft-" + e.Params["Parameters"]["PushLeft"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("SpcBottom"))
                        {
                            cssClass += " spcBottom-" + e.Params["Parameters"]["SpcBottom"].Get<int>();
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
                        if (e.Params["Parameters"].Contains("MarginBottom"))
                        {
                            cssClass += " spcBottom-" + e.Params["Parameters"]["MarginBottom"].Get<int>();
                        }
                        if (e.Params["Parameters"].Contains("PullTop"))
                        {
                            cssClass += " pullTop-" + e.Params["Parameters"]["PullTop"].Get<int>();
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
                        if (e.Params["Parameters"].Contains("CssClass"))
                        {
                            cssClass += " " + e.Params["Parameters"]["CssClass"].Get<string>();
                        }
                        if (string.IsNullOrEmpty(cssClass))
                        {
                            // Defaulting to down-1 ...
                            cssClass = "down-1";
                        }
                        if (e.Params["Parameters"].Contains("DynCssClass"))
                        {
                            string cssClass2 = e.Params["Parameters"]["DynCssClass"].Get<string>();
                            if (!string.IsNullOrEmpty(cssClass2.Trim()))
                                cssClass += " " + cssClass2;
                        }
                        dyn.CssClass = cssClass.Trim();
                    }
                    if (!AjaxManager.Instance.IsCallback)
                    {
                        dyn.Style[Styles.display] = "none";
                        new EffectFadeIn(dyn, 750)
                            .Render();
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
                    dyn.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
            else if (e.Params["Position"].Get<string>() == "fullScreen")
            {
                ClearControls(fullScreen, true);
                new EffectFadeIn(fullScreen, 500)
                    .Render();
                fullScreen.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
            }
            else if (e.Params["Position"].Get<string>() == "child")
            {
                pnlAll.Visible = true;
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

                    if (e.Params["Parameters"].Contains("CloseEvent"))
                    {
                        w.Info = e.Params["Parameters"]["CloseEvent"].Get<string>();
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
                                new EffectFadeIn(w, 250)
                                    .JoinThese(
                                        new EffectSize(width, height))
                                    .Render();
                            }
                            else
                            {
                                w.Style[Styles.display] = "";
                                new EffectSize(w, 250, width, height)
                                    .Render();
                            }
                        }
                        else
                        {
                            int width = e.Params["Parameters"]["ForcedSize"]["width"].Get<int>();
                            w.Style[Styles.width] = width + "px";
                            if (AjaxManager.Instance.IsCallback)
                            {
                                new EffectFadeIn(w, 250)
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
                    ClearControls(toAddInto, true);
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
<meta name=""viewport"" content=""width={0}{1}{2}{3}{4}{5}"" />
<meta name=""apple-mobile-web-app-capable"" content=""yes"" />
<meta name=""apple-mobile-web-app-status-bar-style"" content=""black"" />
<link rel=""apple-touch-icon"" href=""./media/images/icon.png"" />",
                e.Params["Width"].Value.ToString().Replace("px", ""),
                e.Params["InitialScale"].Value != null 
                    ? string.Format(", initial-scale={0}", e.Params["InitialScale"].Get<string>()) 
                    : "",
                e.Params["InitialScale"].Value == null ? ", user-scalable=no" : "",
                e.Params["MaxScale"].Value != null ? string.Format(", maximum-scale={0}", e.Params["MaxScale"].Value) : "",
                e.Params["MinScale"].Value != null ? string.Format(", minimum-scale={0}", e.Params["MinScale"].Value) : "",
                e.Params["TargetDensity"].Value != null ? string.Format(", target-densitydpi={0}", e.Params["TargetDensity"].Value) : "");
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
            if (!string.IsNullOrEmpty(w.Info))
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    w.Info);
                w.Info = "";
            }
            ClearControls(w.Content.Controls[0] as DynamicPanel, true);
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
                Node nn = e.Extra as Node;
                ctrl.Init +=
                    delegate
                    {
                        IModule module = ctrl as IModule;
                        if (module != null)
                        {
                            module.InitialLoading(nn);
                        }
                    };
                ctrl.Load +=
                    delegate
                    {
                        if (nn != null &&
                            nn.Contains("ModuleInitializationEvent") &&
                            !string.IsNullOrEmpty(nn["ModuleInitializationEvent"].Get<string>()))
                        {
                            nn["_ctrl"].Value = ctrl;
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                nn["ModuleInitializationEvent"].Get<string>(),
                                nn);
                            nn["_ctrl"].UnTie();
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



