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
     * Level2: Colorpicker Module for allowing the end user [or you] to pick colors in the system. Also
     * has support for picking images for use as textures as alternatives to picking color
     */
    [ActiveModule]
    public class PickColorOrImage : ActiveModule
    {
        protected ColorPicker clr;
        protected Button getImage;
        protected Button ok;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    if (DataSource.Contains("NoImage") && 
                        DataSource["NoImage"].Get<bool>())
                    {
                        getImage.Visible = false;

                        new EffectTimeout(500)
                            .ChainThese(
                                new EffectFocusAndSelect(ok))
                            .Render();
                    }
                    else
                    {
                        new EffectTimeout(500)
                            .ChainThese(
                                new EffectFocusAndSelect(getImage))
                            .Render();
                    }
                    if (DataSource.Contains("Color"))
                        clr.Value = System.Drawing.ColorTranslator.FromHtml(DataSource["Color"].Get<string>());
                    else
                        clr.Value = Color.FromArgb(255, 255, 255, 255);
                };
        }

        protected void getImage_Click(object sender, EventArgs e)
        {
            Node tmp = new Node();
            if (DataSource.Contains("Top"))
                tmp["Top"].Value = DataSource["Top"].Value;

            RaiseSafeEvent(
                "Magix.Core.FilePicker.SelectFileInsteadOfColor",
                tmp);
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
            else if (DataSource.Contains("DeselectEvent") &&
                DataSource["DeselectEvent"].Value != null)
            {
                Node node = new Node();

                RaiseSafeEvent(
                    DataSource["DeselectEvent"].Get<string>(),
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

        /**
         * Level2: Will raise the SelectEvent with the 'FileName' property set to an image
         * file the end user has just selected
         */
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
    }
}
