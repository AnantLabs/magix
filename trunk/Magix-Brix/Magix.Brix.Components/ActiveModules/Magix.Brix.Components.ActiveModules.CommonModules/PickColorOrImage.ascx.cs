/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
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
using System.Drawing;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    /**
     */
    [ActiveModule]
    public class PickColorOrImage : ActiveModule
    {
        protected ColorPicker clr;
        protected Button getImage;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(getImage))
                        .Render();
                    if (DataSource.Contains("Color"))
                        clr.Value = System.Drawing.ColorTranslator.FromHtml(DataSource["Color"].Get<string>());
                    else
                        clr.Value = Color.FromArgb(255, 255, 255, 255);
                };
        }

        protected void getImage_Click(object sender, EventArgs e)
        {
            RaiseSafeEvent("Magix.Core.FilePicker.SelectFileInsteadOfColor");
        }

        [ActiveEvent(Name = "Magix.Core.ColorPicker.FileSelected")]
        protected void Magix_Core_ColorPicker_FileSelected(object sender, ActiveEventArgs e)
        {
            string file = e.Params["Folder"].Get<string>() + e.Params["FileName"].Get<string>();

            Node node = new Node();
            node["FileName"].Value = file;

            RaiseSafeEvent(
                DataSource["SelectEvent"].Get<string>(),
                node);
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            if (clr.HasValue())
            {
                Node node = new Node();
                node["Color"].Value = System.Drawing.ColorTranslator.ToHtml(clr.Value);

                RaiseSafeEvent(
                    DataSource["SelectEvent"].Get<string>(),
                    node);
            }
            else
            {
                Node node = new Node();
                node["Message"].Value = "You need to pick a value, either a color, or a texture ...";

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
        }
    }
}
