/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;
using System.Reflection;
using System.Drawing;
using Magix.UX.Effects;
using Magix.UX.Aspects;

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     * Level2: Style builder for creating CSS style collections and such for animations, 
     * CSS classes, styles etc
     */
    [ActiveModule]
    public class StyleBuilder : ActiveModule
    {
        protected MultiPanel mp;
        protected TabStrip mub;
        protected TabButton mb1;
        protected TabButton mb2;
        protected TabButton mb3;
        protected TabButton mb4;

        protected TextBox marginLeft;
        protected TextBox marginTop;
        protected TextBox marginRight;
        protected TextBox marginBottom;
        protected TextBox paddingLeft;
        protected TextBox paddingTop;
        protected TextBox paddingRight;
        protected TextBox paddingBottom;
        protected TextBox borderWidth;

        protected SelectList fontName;
        protected CheckBox chkBold;
        protected CheckBox chkItalic;
        protected CheckBox chkUnderline;
        protected CheckBox chkStrikethrough;
        protected TextBox lineHeight;
        protected SelectList textAlign;
        protected SelectList textVerticalAlign;

        protected Panel fgText;
        protected Panel bgText;
        protected Panel borderColorPnl;
        protected TextBox shadowHorizontalOffset;
        protected TextBox shadowVerticalOffset;
        protected TextBox shadowBlur;
        protected Panel gradientStart;
        protected Panel gradientStop;
        protected Panel shadowColor;

        protected SelectList animations;
        protected Panel preview;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    SetActiveMultiViewIndex(0);
                    DataBindAnimations();
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreatePreviewControl();
        }

        private void CreatePreviewControl()
        {
            bool hasControl = false;
            if (DataSource.Contains("TypeName") &&
                !string.IsNullOrEmpty(DataSource["TypeName"].Get<string>()))
            {
                Node nn = new Node();

                nn["TypeName"].Value = DataSource["TypeName"].Get<string>();

                RaiseSafeEvent(
                    "Magix.MetaForms.CreateControl",
                    nn);

                if (nn.Contains("Control"))
                {
                    preview.Controls.Add(nn["Control"].Get<Control>());
                    hasControl = true;
                }
            }
            if (!hasControl)
            {
                Label l = new Label();
                l.Tag = "div";
                l.Text = "Howdie There ... :)";
                l.ToolTip = "Depending upon which Widget you attach this Style Collection to later, some things might not necessary look exactly like they do here, since we do not know which Widget type you intend to attach this Style collection to, and we just have to 'guess' on a 'div' HTML element. If that's not the case, some parts of your style collection might not necessary render exactly like they do here ...";
                preview.Controls.Add(l);
            }
        }

        private void SetStylesForPreviewWidget()
        {
        }

        private void DataBindAnimations()
        {
            if (DataSource.Contains("Animations"))
            {
                foreach (Node idx in DataSource["Animations"])
                {
                    ListItem li = new ListItem();
                    li.Text = idx["Name"].Get<string>();
                    li.Value = idx["CSSClass"].Get<string>();
                    animations.Items.Add(li);
                }
            }
        }

        private void SetActiveMultiViewIndex(int index)
        {
            mp.SetActiveView(index);

            switch (index)
            {
                case 0:
                    mb1.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view";
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(marginLeft))
                        .Render();
                    break;
                case 1:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view";
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(fontName))
                        .Render();
                    break;
                case 2:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb4.CssClass = "mux-multi-button-view";
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(shadowHorizontalOffset))
                        .Render();
                    break;
                case 3:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    SetStylesForPreviewWidget();
                    break;
            }
        }

        protected void mp_MultiButtonClicked(object sender, EventArgs e)
        {
            if (mub.ActiveMultiButtonViewIndex == 3)
            {
                SetActiveMultiViewIndex(3);
            }
            else if (mub.ActiveMultiButtonViewIndex == 2)
            {
                SetActiveMultiViewIndex(2);
            }
            else if (mub.ActiveMultiButtonViewIndex == 1)
            {
                SetActiveMultiViewIndex(1);
            }
            else if (mub.ActiveMultiButtonViewIndex == 0)
            {
                SetActiveMultiViewIndex(0);
            }
        }

        protected void borderColorPnl_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;

            node["SelectEvent"].Value = "Magix.MetaForms.FGColorColorWasPickedForBorder";
            node["Caption"].Value = "Pick Foreground Color for Border";
            node["NoImage"].Value = true;

            if (DataSource["Style"].Contains("border-color"))
                node["Color"].Value = DataSource["Style"]["border-color"].Value;
            else
                node["Color"].Value = "#000000";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void next1_Click(object sender, EventArgs e)
        {
            SetActiveMultiViewIndex(1);
        }

        protected void next2_Click(object sender, EventArgs e)
        {
            SetActiveMultiViewIndex(2);
        }

        protected void next3_Click(object sender, EventArgs e)
        {
            SetActiveMultiViewIndex(3);
        }

        protected void finish_Click(object sender, EventArgs e)
        {
        }

        protected void fgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;

            node["SelectEvent"].Value = "Magix.MetaForms.FGColorColorWasPicked";
            node["Caption"].Value = "Pick Foreground Color for Widget";
            node["NoImage"].Value = true;

            if (DataSource["Style"].Contains("color"))
                node["Color"].Value = DataSource["Style"]["color"].Value;
            else
                node["Color"].Value = "#000000";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void bgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;

            node["SelectEvent"].Value = "Magix.MetaForms.BGColorColorWasPicked";
            node["Caption"].Value = "Pick Background Color for Widget";

            if (DataSource["Style"].Contains("color"))
                node["Color"].Value = DataSource["Style"]["color"].Value;
            else
                node["Color"].Value = "#FFFFFF";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void gradientStart_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;
            node["NoImage"].Value = true;

            node["SelectEvent"].Value = "Magix.MetaForms.BGGradientStartColorWasPicked";
            node["Caption"].Value = "Pick Start Color for Gradient";

            if (DataSource.Contains("Gradient") &&
                DataSource["Gradient"].Contains("StartColor") &&
                !string.IsNullOrEmpty(DataSource["Gradient"]["StartColor"].Get<string>()))
                node["Color"].Value = DataSource["Gradient"]["StartColor"].Value;
            else
                node["Color"].Value = "#EEEEEE";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void gradientStop_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;
            node["NoImage"].Value = true;

            node["SelectEvent"].Value = "Magix.MetaForms.BGGradientStopColorWasPicked";
            node["Caption"].Value = "Pick End Color for Gradient";

            if (DataSource.Contains("Gradient") &&
                DataSource["Gradient"].Contains("EndColor") &&
                !string.IsNullOrEmpty(DataSource["Gradient"]["EndColor"].Get<string>()))
                node["Color"].Value = DataSource["Gradient"]["EndColor"].Value;
            else
                node["Color"].Value = "#CCCCCC";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }


        protected void shadowColor_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;
            node["NoImage"].Value = true;

            node["SelectEvent"].Value = "Magix.MetaForms.ShadowColorWasPicked";
            node["Caption"].Value = "Pick Color for Shadow";

            if (DataSource["Style"].Contains("box-shadow"))
                node["Color"].Value = DataSource["Style"]["box-shadow"]["Color"].Value;
            else
                node["Color"].Value = "#000000";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        /**
         * Level2: Sets the border-color property of the Widget
         */
        [ActiveEvent(Name = "Magix.MetaForms.FGColorColorWasPickedForBorder")]
        protected void Magix_MetaForms_FGColorColorWasPickedForBorders(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            borderColorPnl.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will change the Foreground Color setting of the builder to the given 
         * 'Color' parameter
         */
        [ActiveEvent(Name = "Magix.MetaForms.FGColorColorWasPicked")]
        protected void Magix_MetaForms_FGColorColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            fgText.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will set the Background Gradient Color selection to the given 'Color' 
         * parameter value
         */
        [ActiveEvent(Name = "Magix.MetaForms.BGGradientStartColorWasPicked")]
        protected void Magix_MetaForms_BGGradientStartColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            gradientStart.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will set the Background Gradient Color selection to the given 'Color' 
         * parameter value
         */
        [ActiveEvent(Name = "Magix.MetaForms.BGGradientStopColorWasPicked")]
        protected void Magix_MetaForms_BGGradientStopColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            gradientStop.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will set the Shadow Color for the widget to the 'Color' value given
         */
        [ActiveEvent(Name = "Magix.MetaForms.ShadowColorWasPicked")]
        protected void Magix_MetaForms_ShadowColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            shadowColor.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will change the Background Color setting of the builder to the given 
         * 'Color' parameter
         */
        [ActiveEvent(Name = "Magix.MetaForms.BGColorColorWasPicked")]
        protected void Magix_MetaForms_BGColorColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");

            if (e.Params.Contains("Color"))
            {
                string color = e.Params["Color"].Get<string>();
                bgText.Style[Styles.backgroundColor] = color;

                bgText.Style[Styles.backgroundImage] = "";
            }
            else
            {
                string img = e.Params["FileName"].Get<string>();

                bgText.Style[Styles.backgroundImage] = "url(" + img + ")";
                bgText.Style[Styles.backgroundAttachment] = "scroll";
                bgText.Style[Styles.backgroundPosition] = "0 0";
                bgText.Style[Styles.backgroundRepeat] = "no-repeat";

                bgText.Style[Styles.backgroundColor] = "";

                // There are TWO popups here now ...
                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }
    }
}
