/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
        protected Label lbl;
        protected Panel root;

        public void InitialLoading(Node node)
        {
            Load += delegate
            {
                img.ImageUrl = node["ImageUrl"].Get<string>();
                img.AlternateText = node["AlternateText"].Get<string>();
                img.ToolTip = node["AlternateText"].Get<string>();

                if (node.Contains("ChildCssClass"))
                    root.CssClass = node["ChildCssClass"].Get<string>();

                DataSource = node;
                if (node.Contains("styles"))
                {
                    foreach (Node idx in node["styles"])
                    {
                        img.Style[idx.Name] = idx.Get<string>();
                    }
                }
                if (node.Contains("Description") &&
                    !string.IsNullOrEmpty(node["Description"].Get<string>()))
                {
                    lbl.Visible = true;
                    lbl.Text = node["Description"].Get<string>();
                }
                else
                {
                    lbl.Visible = false;
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

        [ActiveEvent(Name = "Magix.Core.ChangeImage")]
        protected void Magix_Core_ChangeImage(object sende, ActiveEventArgs e)
        {
            if ((e.Params.Contains("Seed") && 
                e.Params["Seed"].Value.Equals(DataSource["Seed"].Value)) ||
                !e.Params.Contains("Seed"))
            {
                img.ImageUrl = e.Params["ImageURL"].Get<string>();
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}



