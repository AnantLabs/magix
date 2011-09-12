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
        protected TextBox borderWidth;
        protected SelectList borderStyle;
        protected Panel borderColorPnl;
        protected TextBox paddingLeft;
        protected TextBox paddingTop;
        protected TextBox paddingRight;
        protected TextBox paddingBottom;
        protected CheckBox chkFloat;
        protected CheckBox chkClear;
        protected CheckBox chkBlock;
        protected CheckBox chkInline;

        protected SelectList fontName;
        protected CheckBox chkBold;
        protected CheckBox chkItalic;
        protected CheckBox chkUnderline;
        protected CheckBox chkStrikethrough;
        protected TextBox fontSize;
        protected SelectList textAlign;
        protected SelectList textVerticalAlign;

        protected Panel fgText;
        protected Panel bgText;
        protected TextBox shadowHorizontalOffset;
        protected TextBox shadowVerticalOffset;
        protected TextBox shadowBlur;
        protected Panel shadowColor;
        protected Panel gradientStart;
        protected Panel gradientStop;
        protected TextBox roundedCornersTopLeft;
        protected TextBox roundedCornersTopRight;
        protected TextBox roundedCornersBottomLeft;
        protected TextBox roundedCornersBottomRight;

        protected SelectList animations;
        protected Panel preview;

        protected BaseWebControl _ctrl;

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
                    if (nn["Control"].Get<Control>() is BaseWebControl)
                    {
                        _ctrl = nn["Control"].Get<BaseWebControl>();
                        nn["Control"].Get<BaseWebControl>().CssClass = DataSource["CSSClass"].Get<string>();
                    }
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
            if (_ctrl == null)
                return;

            if (!string.IsNullOrEmpty(marginLeft.Text))
                _ctrl.Style[Styles.marginLeft] = marginLeft.Text + "px";
            else
                _ctrl.Style[Styles.marginLeft] = "";

            if (!string.IsNullOrEmpty(marginTop.Text))
                _ctrl.Style[Styles.marginTop] = marginTop.Text + "px";
            else
                _ctrl.Style[Styles.marginTop] = "";

            if (!string.IsNullOrEmpty(marginRight.Text))
                _ctrl.Style[Styles.marginRight] = marginRight.Text + "px";
            else
                _ctrl.Style[Styles.marginRight] = "";

            if (!string.IsNullOrEmpty(marginBottom.Text))
                _ctrl.Style[Styles.marginBottom] = marginBottom.Text + "px";
            else
                _ctrl.Style[Styles.marginBottom] = "";

            if (!string.IsNullOrEmpty(borderWidth.Text))
                _ctrl.Style[Styles.borderWidth] = borderWidth.Text + "px";
            else
                _ctrl.Style[Styles.borderWidth] = "";

            if (borderStyle.SelectedIndex != 0)
                _ctrl.Style[Styles.borderStyle] = borderStyle.SelectedItem.Value;
            else
                _ctrl.Style[Styles.borderStyle] = "";

            if (!string.IsNullOrEmpty(borderColorPnl.Style[Styles.backgroundColor]))
                _ctrl.Style[Styles.backgroundColor] = borderColorPnl.Style[Styles.backgroundColor];
            else
                _ctrl.Style[Styles.backgroundColor] = "";

            if (!string.IsNullOrEmpty(paddingLeft.Text))
                _ctrl.Style[Styles.paddingLeft] = paddingLeft.Text + "px";
            else
                _ctrl.Style[Styles.paddingLeft] = "";

            if (!string.IsNullOrEmpty(paddingTop.Text))
                _ctrl.Style[Styles.paddingTop] = paddingTop.Text + "px";
            else
                _ctrl.Style[Styles.paddingTop] = "";

            if (!string.IsNullOrEmpty(paddingRight.Text))
                _ctrl.Style[Styles.paddingRight] = paddingRight.Text + "px";
            else
                _ctrl.Style[Styles.paddingRight] = "";

            if (!string.IsNullOrEmpty(paddingBottom.Text))
                _ctrl.Style[Styles.paddingBottom] = paddingBottom.Text + "px";
            else
                _ctrl.Style[Styles.paddingBottom] = "";

            if (chkFloat.Checked)
                _ctrl.Style[Styles.floating] = "left";
            else
                _ctrl.Style[Styles.floating] = "";

            if (chkClear.Checked)
                _ctrl.Style[Styles.clear] = "both";
            else
                _ctrl.Style[Styles.clear] = "";

            if (chkBlock.Checked)
                _ctrl.Style[Styles.display] = "block";
            else if (chkInline.Checked)
                _ctrl.Style[Styles.display] = "inline";
            else
                _ctrl.Style[Styles.display] = "";

            if (fontName.SelectedIndex != 0)
                _ctrl.Style[Styles.fontFamily] = fontName.SelectedItem.Value;
            else
                _ctrl.Style[Styles.fontFamily] = "";

            if (chkBold.Checked)
                _ctrl.Style[Styles.fontWeight] = "bold";
            else
                _ctrl.Style[Styles.fontWeight] = "";

            if (chkItalic.Checked)
                _ctrl.Style[Styles.fontStyle] = "italic";

            _ctrl.Style[Styles.textDecoration] = "";

            if (chkUnderline.Checked)
                _ctrl.Style[Styles.textDecoration] += " underline";

            if (chkStrikethrough.Checked)
                _ctrl.Style[Styles.textDecoration] += " line-through";

            if (!string.IsNullOrEmpty(fontSize.Text))
                _ctrl.Style[Styles.fontSize] = fontSize.Text + "px";
            else
                _ctrl.Style[Styles.fontSize] = "";

            if (textAlign.SelectedIndex != 0)
                _ctrl.Style[Styles.textAlign] = textAlign.SelectedItem.Value;
            else
                _ctrl.Style[Styles.textAlign] = "";

            if (textVerticalAlign.SelectedIndex != 0)
                _ctrl.Style[Styles.verticalAlign] = textVerticalAlign.SelectedItem.Value;
            else
                _ctrl.Style[Styles.verticalAlign] = "";

            if (!string.IsNullOrEmpty(fgText.Style[Styles.backgroundColor]))
                _ctrl.Style[Styles.backgroundColor] = fgText.Style[Styles.backgroundColor];
            else
                _ctrl.Style[Styles.backgroundColor] = "";

            if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundColor]))
            {
                _ctrl.Style[Styles.backgroundColor] = bgText.Style[Styles.backgroundColor];
                bgText.Style[Styles.backgroundImage] = "";
            }
            else if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundImage]))
            {
                bgText.Style[Styles.backgroundImage] = bgText.Style[Styles.backgroundImage];
                bgText.Style[Styles.backgroundAttachment] = "scroll";
                bgText.Style[Styles.backgroundPosition] = "0 0";
                bgText.Style[Styles.backgroundRepeat] = "no-repeat";
                bgText.Style[Styles.backgroundColor] = "";
            }
            else
            {
                bgText.Style[Styles.backgroundImage] = "";
                bgText.Style[Styles.backgroundColor] = "";
            }

            if (!string.IsNullOrEmpty(shadowColor.Style[Styles.backgroundColor]))
            {
                string stl = "";
                stl += shadowHorizontalOffset.Text + "px ";
                stl += shadowVerticalOffset.Text + "px ";
                stl += shadowBlur.Text + "px ";
                stl += shadowColor.Style[Styles.backgroundColor];
                _ctrl.Style["-moz-box-shadow"] = stl;
                _ctrl.Style["-webkit-box-shadow"] = stl;
                _ctrl.Style["-op-box-shadow"] = stl;
                _ctrl.Style["-ie-box-shadow"] = stl;
                _ctrl.Style["box-shadow"] = stl;
            }
            else
            {
                _ctrl.Style["-moz-box-shadow"] = "";
                _ctrl.Style["-webkit-box-shadow"] = "";
                _ctrl.Style["-op-box-shadow"] = "";
                _ctrl.Style["-ie-box-shadow"] = "";
                _ctrl.Style["box-shadow"] = "";
            }
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

        protected void animations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ctrl != null)
            {
                string cssClass = animations.SelectedItem.Value;
                _ctrl.CssClass += DataSource["CSSClass"].Get<string>() + " " + cssClass;
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
