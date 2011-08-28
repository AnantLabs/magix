/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: Control for displaying images in your app. Either clickable or static
     * images. Pass in 'ImageURL', 'AlternateText', 'ChildCssClass' and
     * 'Description' to modify it according to your needs. Description, if given,
     * will add a label underneath the image. Use 'Seed' to have multiple Images
     * on same page and to separate between different instances of them.
     * If 'Events/Click' is given, it'll raise that event upon clicking the image
     */
    [ActiveModule]
    public class ImageModule : ActiveModule
    {
        protected Image img;
        protected Label lbl;
        protected Panel root;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load += delegate
            {
                img.ImageUrl = node["ImageURL"].Get<string>();
                img.AlternateText = node["AlternateText"].Get<string>();
                img.ToolTip = node["AlternateText"].Get<string>();

                if (node.Contains("ChildCssClass"))
                    root.CssClass = node["ChildCssClass"].Get<string>();

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
            if (DataSource.Contains("Events") && 
                DataSource["Events"].Contains("Click"))
            {
                img.Click +=
                    delegate(object sender, EventArgs e2)
                    {
                        RaiseSafeEvent(
                            DataSource["Events"]["Click"].Get<string>(),
                            DataSource["Events"]["Click"]);

                        img.Style[Styles.cursor] = "pointer";
                    };
            }
        }

        /**
         * Level2: Updates the image to a new 'ImageURL'. But only if 'Seed' is given and correct
         * according to 'Seed' given when loaded
         */
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
    }
}
