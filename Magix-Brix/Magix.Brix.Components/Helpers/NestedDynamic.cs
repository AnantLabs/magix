/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Effects;

namespace Magix.Brix.Components
{
    public abstract class NestedDynamic : UserControl, IModule
    {
        protected Window wnd;
        protected AspectModal modal;
        protected DynamicPanel child;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            wnd = new Window();
            wnd.CssClass = "mux-shaded mux-rounded";
            wnd.Style[Styles.left] = "15px";
            wnd.Style[Styles.top] = "15px";
            wnd.Style[Styles.minWidth] = "950px";
            wnd.Style[Styles.minHeight] = "450px";
            wnd.Style[Styles.position] = "absolute";
            wnd.Style[Styles.zIndex] = "1000";
            wnd.Visible = false;
            wnd.ID = "wnd";
            wnd.Closed += wnd_Closed;

            modal = new AspectModal();
            wnd.Controls.Add(modal);

            child = new DynamicPanel();
            child.CssClass = "dynamic";
            child.Reload += child_LoadControls;
            child.ID = "child";
            wnd.Content.Controls.Add(child);
            this.Controls.Add(wnd);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindObjects();
        }

        protected abstract void DataBindObjects();

        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }

        protected int TotalCount
        {
            get { return DataSource["TotalCount"].Get<int>(); }
        }

        protected string FullTypeName
        {
            get { return DataSource["FullTypeName"].Get<string>(); }
        }

        protected string TypeName
        {
            get { return DataSource["FullTypeName"].Get<string>().Substring(DataSource["FullTypeName"].Get<string>().LastIndexOf(".") + 1); }
        }

        protected string ParentPropertyName
        {
            get { return DataSource["ParentPropertyName"].Get<string>(); }
        }

        protected string ParentType
        {
            get { return DataSource["ParentType"].Get<string>(); }
        }

        protected string ParentFullType
        {
            get { return DataSource["ParentFullType"].Get<string>(); }
        }

        protected int ParentID
        {
            get { return DataSource["ParentID"].Get<int>(); }
        }

        [ActiveEvent(Name = "ClearControlsForSpecificDynamic")]
        protected void ClearControlsForSpecificDynamic(object sender, ActiveEventArgs e)
        {
            if (child.Controls.Count > 0 && child.Controls[0].ClientID == e.Params["ClientID"].Get<string>())
            {
                // Yup, we're it ...!
                ClearControls(child);
                wnd.Visible = false;

                // Yup, we're it ...!
                ReDataBind();
            }
        }

        [ActiveEvent(Name = "UpdateSpecificNestedDynamic")]
        protected void UpdateSpecificNestedDynamic(object sender, ActiveEventArgs e)
        {
            if (child.ClientID == e.Params["ClientID"].Get<string>())
            {
                // Yup, we're it ...!
                ReDataBind();
            }
        }

        protected abstract void ReDataBind();

        [ActiveEvent(Name = "LoadControl")]
        protected void LoadControl(object sender, ActiveEventArgs e)
        {
            if (e.Params["Position"].Get<string>() == "child" && child.Controls.Count == 0)
            {
                wnd.Visible = true;
                wnd.Style[Styles.display] = "none";
                new EffectFadeIn(wnd, 1500)
                    .Render();
                if (e.Params["Parameters"].Contains("Caption"))
                {
                    wnd.Caption = e.Params["Parameters"]["Caption"].Get<string>();
                }
                if (true.Equals(e.Params["Parameters"].Contains("Append")))
                    child.AppendControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                else
                {
                    ClearControls(child);
                    child.LoadControl(e.Params["Name"].Value.ToString(), e.Params["Parameters"]);
                }
            }
        }

        protected void child_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
        {
            DynamicPanel dynamic = sender as DynamicPanel;
            System.Web.UI.Control ctrl = PluginLoader.Instance.LoadActiveModule(e.Key);
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

        protected void ClearControls(DynamicPanel dynamic)
        {
            foreach (System.Web.UI.Control idx in dynamic.Controls)
            {
                ActiveEvents.Instance.RemoveListener(idx);
            }
            dynamic.ClearControls();
        }

        protected void wnd_Closed(object sender, EventArgs e)
        {
            ClearControls(child);
        }
    }

}
