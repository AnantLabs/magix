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
using System.Drawing;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.QRGenerator
{
    /**
     * Level2: Vanity QR Code Generator for creating 'beautiful' QR Codes. Also contains 
     * an algorithm [burn] which creates more reciliant QR codes that scans faster than 
     * conventional square dots
     */
    [ActiveModule]
    [PublisherPlugin(CanBeEmpty = true)]
    public class Generator : ActiveModule
    {
        protected MultiPanel mp;
        protected TextBox url;
        protected Panel fgText;
        protected Panel bgText;
        protected CheckBox burn;
        protected TextBox rotate;
        protected TextBox description;
        protected TextBox borderRadius;
        protected Magix.UX.Widgets.Image qrCode;
        protected TabStrip mub;
        protected TextBox urlCode;
        protected Button next2;
        protected TabButton mb1;
        protected TabButton mb2;
        protected TabButton mb3;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    if (DataSource.Contains("FGImage"))
                    {
                        fgText.Style[Styles.background] =
                            "Transparent url(" +
                            DataSource["FGImage"].Get<string>() +
                            ") no-repeat 0 0";
                    }
                    else
                    {
                        if (DataSource.Contains("FGColor"))
                        {
                            fgText.Style[Styles.backgroundColor] = DataSource["FGColor"].Get<string>();
                        }
                        else
                        {
                            DataSource["FGColor"].Value = "#000000";
                            fgText.Style[Styles.backgroundColor] = DataSource["FGColor"].Get<string>();
                        }
                    }
                    if (DataSource.Contains("BGImage"))
                    {
                        fgText.Style[Styles.background] =
                            "Transparent url(" +
                            DataSource["BGImage"].Get<string>() +
                            ") no-repeat 0 0";
                    }
                    else
                    {
                        if (DataSource.Contains("BGColor"))
                        {
                            bgText.Style[Styles.backgroundColor] = DataSource["BGColor"].Get<string>();
                            DataSource["BGColor"].Value = "#ffffff";
                            bgText.Style[Styles.backgroundColor] = DataSource["BGColor"].Get<string>();
                        }
                        else
                        {
                            DataSource["BGColor"].Value = "#ffffff";
                            bgText.Style[Styles.backgroundColor] = DataSource["BGColor"].Get<string>();
                        }
                    }
                    burn.Checked = DataSource["Animate"].Get(true);
                    rotate.Text = DataSource["Rotate"].Get(12).ToString();

                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(url))
                        .Render();

                    FileNameGuid = Guid.NewGuid();
                    SetActiveMultiViewIndex(0);
                };
        }

        private Guid FileNameGuid
        {
            get
            {
                return (Guid)ViewState["FileNameGuid"];
            }
            set
            {
                ViewState["FileNameGuid"] = value;
            }
        }

        protected void mp_MultiButtonClicked(object sender, EventArgs e)
        {
            if (mub.ActiveMultiButtonViewIndex == 2)
            {
                SetActiveMultiViewIndex(2);

                CreateQRCode();

                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(urlCode))
                    .Render();
            }
            else if (mub.ActiveMultiButtonViewIndex == 1)
            {
                SetActiveMultiViewIndex(1);

                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(next2))
                    .Render();
            }
            else if (mub.ActiveMultiButtonViewIndex == 0)
            {
                SetActiveMultiViewIndex(0);

                new EffectTimeout(500)
                    .ChainThese(
                        new EffectFocusAndSelect(url))
                    .Render();
            }
        }

        private void SetActiveMultiViewIndex(int index)
        {
            switch (index)
            {
                case 0:
                    mb1.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view";
                    break;
                case 1:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb3.CssClass = "mux-multi-button-view";
                    break;
                case 2:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    break;
            }
        }

        protected void next1_Click(object sender, EventArgs e)
        {
            mp.SetActiveView(1);

            new EffectTimeout(500)
                .ChainThese(
                    new EffectFocusAndSelect(next2))
                .Render();

            SetActiveMultiViewIndex(1);
        }

        protected void next2_Click(object sender, EventArgs e)
        {
            mp.SetActiveView(2);

            CreateQRCode();

            new EffectTimeout(500)
                .ChainThese(
                    new EffectFocusAndSelect(urlCode))
                .Render();

            SetActiveMultiViewIndex(2);
        }

        protected void next3_Click(object sender, EventArgs e)
        {
            mp.SetActiveView(0);

            new EffectTimeout(500)
                .ChainThese(
                    new EffectFocusAndSelect(url))
                .Render();

            // Creating new 'filename' ...
            FileNameGuid = Guid.NewGuid();

            SetActiveMultiViewIndex(0);
        }

        protected void next4_Click(object sender, EventArgs e)
        {
            mp.SetActiveView(0);

            new EffectTimeout(500)
                .ChainThese(
                    new EffectFocusAndSelect(url))
                .Render();

            SetActiveMultiViewIndex(0);
        }

        private void CreateQRCode()
        {
            DataSource["Scale"].Value = 5;
            DataSource["ErrCor"].Value = "Q";
            DataSource["URL"].Value = url.Text;
            DataSource["Text"].Value = description.Text;
            if (borderRadius.Text.Length > 0)
                DataSource["RoundedCorners"].Value = int.Parse(borderRadius.Text);
            else
                DataSource["RoundedCorners"].Value = 0;
            DataSource["AntiPixelated"].Value = burn.Checked;
            DataSource["FontName"].Value = "Arial Black";
            DataSource["FontSize"].Value = 25;
            DataSource["FontColor"].Value = "#000000";
            if (rotate.Text.Length > 0)
                DataSource["Rotate"].Value = int.Parse(rotate.Text);
            else
                DataSource["Rotate"].Value = 0;
            DataSource["FontSize"].Value = 25;
            DataSource["FileName"].Value = null; // Intentionally, will generate new filename ...
            if (!DataSource.Contains("FGImage"))
                DataSource["FGImage"].Value = "";
            if (!DataSource.Contains("BGImage"))
                DataSource["BGImage"].Value = "";

            DataSource["FileName"].Value = "Tmp/qr-" + FileNameGuid + ".png";

            RaiseSafeEvent(
                "Magix.QRCodes.CreateQRCode",
                DataSource);

            qrCode.ImageUrl = DataSource["FileName"].Get<string>() + "?x=" + Guid.NewGuid();

            urlCode.Text = GetApplicationBaseUrl() + DataSource["FileName"].Get<string>();

            Node node = new Node();
            node["Message"].Value = "Please be patient while your QR Code is rendering. It might take some time to render and download the code ..";
            node["Milliseconds"].Value = 5000;

            RaiseEvent(
                "Magix.Core.ShowMessage",
                node);
        }

        protected void fgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["SelectEvent"].Value = "Magix.QRCodes.FGColorColorOrTextureWasPicked";
            if (DataSource.Contains("FGColor"))
                node["Color"].Value = DataSource["FGColor"].Value;

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void bgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["SelectEvent"].Value = "Magix.QRCodes.BGColorColorOrTextureWasPicked";
            if (DataSource.Contains("BGColor"))
                node["Color"].Value = DataSource["BGColor"].Value;

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        [ActiveEvent(Name = "Magix.QRCodes.FGColorColorOrTextureWasPicked")]
        protected void Magix_QRCodes_FGColorColorOrTextureWasPicked(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("Color"))
            {
                fgText.Style[Styles.backgroundImage] = "none";
                fgText.Style[Styles.backgroundColor] = e.Params["Color"].Get<string>();
                DataSource["FGColor"].Value = e.Params["Color"].Get<string>();
                DataSource["FGImage"].UnTie();
            }
            else
            {
                fgText.Style[Styles.backgroundImage] = "";
                fgText.Style[Styles.background] = "Transparent url(" + e.Params["FileName"].Get<string>() + ") no-repeat 0 0";
                DataSource["FGImage"].Value = e.Params["FileName"].Get<string>();
                DataSource["FGColor"].UnTie();
                ActiveEvents.Instance.RaiseClearControls("child");
            }
            ActiveEvents.Instance.RaiseClearControls("child");
            next2.Focus();
        }

        [ActiveEvent(Name = "Magix.QRCodes.BGColorColorOrTextureWasPicked")]
        protected void Magix_QRCodes_BGColorColorOrTextureWasPicked(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("Color"))
            {
                bgText.Style[Styles.backgroundImage] = "none";
                bgText.Style[Styles.backgroundColor] = e.Params["Color"].Get<string>();
                DataSource["BGColor"].Value = e.Params["Color"].Get<string>();
                DataSource["BGImage"].UnTie();
            }
            else
            {
                bgText.Style[Styles.backgroundImage] = "";
                bgText.Style[Styles.background] = "Transparent url(" + e.Params["FileName"].Get<string>() + ") no-repeat 0 0";
                DataSource["BGImage"].Value = e.Params["FileName"].Get<string>();
                DataSource["BGColor"].UnTie();
                ActiveEvents.Instance.RaiseClearControls("child");
            }
            ActiveEvents.Instance.RaiseClearControls("child");
            next2.Focus();
        }
    }
}
