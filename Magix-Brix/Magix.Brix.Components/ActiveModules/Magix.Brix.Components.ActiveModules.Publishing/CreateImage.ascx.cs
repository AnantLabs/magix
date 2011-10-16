/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    /**
     * Level2: Encapsulates a module for selecting an image
     */
    [ActiveModule]
    public class CreateImage : ActiveModule
    {
        protected TextBox txt;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    new EffectTimeout(250)
                        .ChainThese(
                            new EffectFocusAndSelect(txt))
                        .Render();
                };
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["URL"].Value = txt.Text;

            RaiseSafeEvent(
                "Magix.Core.ImageWasSelected",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");
        }

        protected void obj_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 33;
            node["IsSelect"].Value = true;
            node["Width"].Value = 24;
            node["Folder"].Value = "/";
            node["Filter"].Value = "*.png;*.jpg;*.jpeg;";
            node["SelectEvent"].Value = "Magix.Core.SelectImage";

            RaiseEvent(
                "Magix.FileExplorer.LaunchExplorer",
                node);
        }

        /**
         * Level2: Sink fallback for allowing the user to 
         * select to link to any object within his filestructure
         */
        [ActiveEvent(Name = "Magix.Core.SelectImage")]
        protected void Magix_Core_SelectImageForHyperLink(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["URL"].Value = 
                GetApplicationBaseUrl() + 
                e.Params["Folder"].Get<string>() + 
                e.Params["FileName"].Get<string>();

            RaiseSafeEvent(
                "Magix.Core.ImageWasSelected",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");
            ActiveEvents.Instance.RaiseClearControls("child");
        }
    }
}
