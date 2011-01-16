/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    [ActiveModule]
    public class ImageModule : UserControl, IModule
    {
        protected Image img;
        protected Button focs;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                img.ImageUrl = node["ImageUrl"].Get<string>();
                img.AlternateText = node["AlternateText"].Get<string>();
                img.ToolTip = node["AlternateText"].Get<string>();
                if (node.Contains("ChildCssClass"))
                    img.CssClass = node["ChildCssClass"].Get<string>();
                DataSource = node;
                if (node.Contains("SetFocus") && node["SetFocus"].Get<bool>())
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(focs))
                        .Render();
                }
                if (node.Contains("styles"))
                {
                    foreach (Node idx in node["styles"])
                    {
                        img.Style[idx.Name] = idx.Get<string>();
                    }
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource.Contains("Events") && DataSource["Events"].Contains("Click"))
            {
                img.Click +=
                    delegate(object sender, EventArgs e2)
                    {
                        Node node = new Node();
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            DataSource["Events"]["Click"].Get<string>(),
                            DataSource["Events"]["Click"]);
                        img.Style[Styles.cursor] = "pointer";
                    };
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}



