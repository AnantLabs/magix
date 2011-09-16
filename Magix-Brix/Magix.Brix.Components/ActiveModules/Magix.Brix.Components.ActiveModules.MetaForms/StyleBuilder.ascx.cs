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
        protected SelectList textAlign;
        protected SelectList textVerticalAlign;
        protected TextBox fontSize;
        protected TextBox width;
        protected TextBox height;
        protected TextBox left;
        protected TextBox top;

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

        protected Button finish;

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
                _ctrl = l;
                l.Tag = "div";
                l.Text = "Howdie There ... :)";
                l.ToolTip = "Depending upon which Widget you attach this Style Collection to later, some things might not necessary look exactly like they do here, since we do not know which Widget type you intend to attach this Style collection to, and we just have to 'guess' on a 'div' HTML element. If that's not the case, some parts of your style collection might not necessary render exactly like they do here ...";
                preview.Controls.Add(l);
            }

            if (_ctrl != null)
            {
                if (DataSource.Contains("Properties"))
                {
                    foreach (Node idx in DataSource["Properties"])
                    {
                        // Skipping 'empty stuff' ...
                        if (idx.Value == null)
                            continue;

                        PropertyInfo info = _ctrl.GetType().GetProperty(
                            idx.Name,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Public);

                        if (info != null)
                        {
                            object tmp = idx.Value;

                            if (tmp.GetType() != info.GetGetMethod(true).ReturnType)
                            {
                                switch (info.GetGetMethod(true).ReturnType.FullName)
                                {
                                    case "System.Boolean":
                                        tmp = bool.Parse(tmp.ToString());
                                        break;
                                    case "System.DateTime":
                                        tmp = DateTime.ParseExact(tmp.ToString(), "yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Int32":
                                        tmp = int.Parse(tmp.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Decimal":
                                        tmp = decimal.Parse(tmp.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    default:
                                        if (info.GetGetMethod(true).ReturnType.BaseType == typeof(Enum))
                                            tmp = Enum.Parse(info.GetGetMethod(true).ReturnType, tmp.ToString());
                                        else
                                            throw new ApplicationException("Unsupported type for serializing to Widget, type was: " + info.GetGetMethod(true).ReturnType.FullName);
                                        break;
                                }
                                info.GetSetMethod(true).Invoke(_ctrl, new object[] { tmp });
                            }
                            else
                                info.GetSetMethod(true).Invoke(_ctrl, new object[] { tmp });
                        }
                    }
                }
                if (FirstLoad && 
                    DataSource.Contains("Properties") &&
                    DataSource["Properties"].Contains("Style"))
                {
                    foreach (Node idx in DataSource["Properties"]["Style"])
                    {
                        string val = idx.Get<string>();

                        switch (idx.Name)
                        {
                            case "margin-left":
                                marginLeft.Text = val.Replace("px", "");
                                break;
                            case "margin-right":
                                marginRight.Text = val.Replace("px", "");
                                break;
                            case "margin-top":
                                marginTop.Text = val.Replace("px", "");
                                break;
                            case "margin-bottom":
                                marginBottom.Text = val.Replace("px", "");
                                break;
                            case "border-width":
                                borderWidth.Text = val.Replace("px", "");
                                break;
                            case "border-style":
                                borderStyle.SetSelectedItemAccordingToValue(val);
                                break;
                            case "border-color":
                                borderColorPnl.Style[Styles.backgroundColor] = val;
                                break;
                            case "padding-left":
                                paddingLeft.Text = val.Replace("px", "");
                                break;
                            case "padding-right":
                                paddingRight.Text = val.Replace("px", "");
                                break;
                            case "padding-top":
                                paddingTop.Text = val.Replace("px", "");
                                break;
                            case "padding-bottom":
                                paddingBottom.Text = val.Replace("px", "");
                                break;
                            case "float":
                                chkFloat.Checked = (val == "left");
                                break;
                            case "clear":
                                chkClear.Checked = (val == "both");
                                break;
                            case "display":
                                if (val == "block")
                                {
                                    chkBlock.Checked = true;
                                    chkInline.Checked = false;
                                }
                                else if (val == "inline")
                                {
                                    chkBlock.Checked = false;
                                    chkInline.Checked = true;
                                }
                                break;
                            case "font-family":
                                fontName.SetSelectedItemAccordingToValue(val);
                                break;
                            case "font-weight":
                                if (val == "bold")
                                    chkBold.Checked = true;
                                break;
                            case "font-style":
                                if (val.Contains("italic"))
                                    chkItalic.Checked = true;
                                break;
                            case "text-decoration":
                                if (val.Contains("underline"))
                                    chkUnderline.Checked = true;
                                if (val.Contains("line-through"))
                                    chkStrikethrough.Checked = true;
                                break;
                            case "text-align":
                                textAlign.SetSelectedItemAccordingToValue(val);
                                break;
                            case "vertical-align":
                                textVerticalAlign.SetSelectedItemAccordingToValue(val);
                                break;
                            case "font-size":
                                fontSize.Text = val.Replace("px", "");
                                break;
                            case "width":
                                width.Text = val.Replace("px", "");
                                break;
                            case "height":
                                height.Text = val.Replace("px", "");
                                break;
                            case "left":
                                left.Text = val.Replace("px", "");
                                break;
                            case "top":
                                top.Text = val.Replace("px", "");
                                break;
                            case "color":
                                fgText.Style[Styles.backgroundColor] = val;
                                break;
                            case "background-color":
                                bgText.Style[Styles.backgroundColor] = val;
                                break;
                            case "background-image":
                                {
                                    // Might contain also gradient(s) ...
                                    if (val.Contains("linear-gradient"))
                                    {
                                        gradientStart.Style[Styles.backgroundColor] = 
                                            val.Substring(val.IndexOf('#'), 7);
                                        gradientStop.Style[Styles.backgroundColor] = 
                                            val.Substring(val.LastIndexOf('#'), 7);
                                        if (val.Contains("url("))
                                            val = val.Trim().Split(',')[0];
                                        else
                                            val = "";
                                    }
                                    if (!string.IsNullOrEmpty(val))
                                        bgText.Style[Styles.backgroundImage] = val;
                                } break;
                            case "box-shadow":
                                shadowHorizontalOffset.Text = val.Trim().Split(' ')[0].Replace("px", "");
                                shadowVerticalOffset.Text = val.Trim().Split(' ')[1].Replace("px", "");
                                shadowBlur.Text = val.Trim().Split(' ')[2].Replace("px", "");
                                shadowColor.Style[Styles.backgroundColor] = val.Trim().Split(' ')[3];
                                break;
                            case "border-radius":
                                roundedCornersTopLeft.Text = val.Trim().Split(' ')[0].Replace("px", "");
                                roundedCornersTopRight.Text = val.Trim().Split(' ')[1].Replace("px", "");
                                roundedCornersBottomRight.Text = val.Trim().Split(' ')[2].Replace("px", "");
                                roundedCornersBottomLeft.Text = val.Trim().Split(' ')[3].Replace("px", "");
                                break;
                        }
                    }
                }
            }
        }

        private void SetStylesForPreviewWidget()
        {
            if (_ctrl == null)
                return;

            if (DataSource.Contains("CSSClass"))
                _ctrl.CssClass = DataSource["CSSClass"].Get<string>();
            else
                _ctrl.CssClass = "";

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
                _ctrl.Style[Styles.borderColor] = borderColorPnl.Style[Styles.backgroundColor];
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

            if (!string.IsNullOrEmpty(width.Text))
                _ctrl.Style[Styles.width] = width.Text + "px";
            else
                _ctrl.Style[Styles.width] = "";

            if (!string.IsNullOrEmpty(height.Text))
                _ctrl.Style[Styles.height] = height.Text + "px";
            else
                _ctrl.Style[Styles.height] = "";

            if (!string.IsNullOrEmpty(left.Text))
                _ctrl.Style[Styles.left] = left.Text + "px";
            else
                _ctrl.Style[Styles.left] = "";

            if (!string.IsNullOrEmpty(top.Text))
                _ctrl.Style[Styles.top] = top.Text + "px";
            else
                _ctrl.Style[Styles.top] = "";

            if (textAlign.SelectedIndex != 0)
                _ctrl.Style[Styles.textAlign] = textAlign.SelectedItem.Value;
            else
                _ctrl.Style[Styles.textAlign] = "";

            if (textVerticalAlign.SelectedIndex != 0)
                _ctrl.Style[Styles.verticalAlign] = textVerticalAlign.SelectedItem.Value;
            else
                _ctrl.Style[Styles.verticalAlign] = "";

            if (!string.IsNullOrEmpty(fgText.Style[Styles.backgroundColor]))
                _ctrl.Style[Styles.color] = fgText.Style[Styles.backgroundColor];
            else
                _ctrl.Style[Styles.color] = "";

            if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundColor]))
            {
                _ctrl.Style[Styles.backgroundColor] = bgText.Style[Styles.backgroundColor];
                bgText.Style[Styles.backgroundImage] = "";
            }
            else if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundImage]))
            {
                _ctrl.Style[Styles.backgroundImage] = bgText.Style[Styles.backgroundImage];
                _ctrl.Style[Styles.backgroundAttachment] = "scroll";
                _ctrl.Style[Styles.backgroundPosition] = "0 0";
                _ctrl.Style[Styles.backgroundRepeat] = "no-repeat";
                _ctrl.Style[Styles.backgroundColor] = "";
            }
            else
            {
                _ctrl.Style[Styles.backgroundImage] = "";
                _ctrl.Style[Styles.backgroundColor] = "";
            }

            if (!string.IsNullOrEmpty(gradientStart.Style[Styles.backgroundColor]))
            {
                string gradient = string.Format("linear-gradient({0} 0%, {1} 100%)",
                    gradientStart.Style[Styles.backgroundColor],
                    gradientStop.Style[Styles.backgroundColor]);
                if (!string.IsNullOrEmpty(_ctrl.Style["background-image"]))
                    gradient = _ctrl.Style["background-image"] + "," + gradient;
                _ctrl.Style["background-image"] = gradient;
            }

            if (!string.IsNullOrEmpty(shadowColor.Style[Styles.backgroundColor]))
            {
                string stl = "";
                stl += shadowHorizontalOffset.Text + "px ";
                stl += shadowVerticalOffset.Text + "px ";
                stl += shadowBlur.Text + "px ";
                stl += shadowColor.Style[Styles.backgroundColor];
                _ctrl.Style["box-shadow"] = stl;
            }
            else
            {
                _ctrl.Style["box-shadow"] = "";
            }

            string rounded = "";
            if (!string.IsNullOrEmpty(roundedCornersTopLeft.Text))
                rounded += roundedCornersTopLeft.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersTopRight.Text))
                rounded += roundedCornersTopRight.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersBottomRight.Text))
                rounded += roundedCornersBottomRight.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersBottomLeft.Text))
                rounded += roundedCornersBottomLeft.Text + "px";
            else
                rounded += "0px";

            if (rounded != "0px 0px 0px 0px")
            {
                _ctrl.Style["border-radius"] = rounded;
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
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(finish))
                        .Render();
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
                _ctrl.CssClass = DataSource["CSSClass"].Get<string>() + " " + cssClass;
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
            Node node = new Node();

            node["CssClass"].Value = _ctrl.CssClass;

            if (!string.IsNullOrEmpty(marginLeft.Text))
                node["Style"]["margin-left"].Value = marginLeft.Text + "px";

            if (!string.IsNullOrEmpty(marginTop.Text))
                node["Style"]["margin-top"].Value = marginTop.Text + "px";

            if (!string.IsNullOrEmpty(marginRight.Text))
                node["Style"]["margin-right"].Value = marginRight.Text + "px";

            if (!string.IsNullOrEmpty(marginBottom.Text))
                node["Style"]["margin-bottom"].Value = marginBottom.Text + "px";

            if (!string.IsNullOrEmpty(borderWidth.Text))
                node["Style"]["border-width"].Value = borderWidth.Text + "px";

            if (borderStyle.SelectedIndex != 0)
                node["Style"]["border-style"].Value = borderStyle.SelectedItem.Value;

            if (!string.IsNullOrEmpty(borderColorPnl.Style[Styles.backgroundColor]))
                node["Style"]["border-color"].Value = borderColorPnl.Style[Styles.backgroundColor];

            if (!string.IsNullOrEmpty(paddingLeft.Text))
                node["Style"]["padding-left"].Value = paddingLeft.Text + "px";

            if (!string.IsNullOrEmpty(paddingTop.Text))
                node["Style"]["padding-top"].Value = paddingTop.Text + "px";

            if (!string.IsNullOrEmpty(paddingRight.Text))
                node["Style"]["padding-right"].Value = paddingRight.Text + "px";

            if (!string.IsNullOrEmpty(paddingBottom.Text))
                node["Style"]["padding-bottom"].Value = paddingBottom.Text + "px";

            if (chkFloat.Checked)
                node["Style"]["float"].Value = "left";

            if (chkClear.Checked)
                node["Style"]["clear"].Value = "both";

            if (chkBlock.Checked)
                node["Style"]["display"].Value = "block";
            else if (chkInline.Checked)
                node["Style"]["display"].Value = "inline";

            if (fontName.SelectedIndex != 0)
                node["Style"]["font-family"].Value = fontName.SelectedItem.Value;

            if (chkBold.Checked)
                node["Style"]["font-weight"].Value = "bold";

            if (chkItalic.Checked)
                node["Style"]["font-style"].Value = "italic";

            if (chkUnderline.Checked)
                node["Style"]["text-decoration"].Value = "underline";

            if (chkStrikethrough.Checked)
            {
                string t = node["Style"]["text-decoration"].Value as string;
                t += " line-through";
                node["Style"]["text-decoration"].Value = t;
            }

            if (!string.IsNullOrEmpty(fontSize.Text))
                node["Style"]["font-size"].Value = fontSize.Text + "px";

            if (!string.IsNullOrEmpty(width.Text))
                node["Style"]["width"].Value = width.Text + "px";

            if (!string.IsNullOrEmpty(height.Text))
                node["Style"]["height"].Value = height.Text + "px";

            if (!string.IsNullOrEmpty(left.Text))
                node["Style"]["left"].Value = left.Text + "px";

            if (!string.IsNullOrEmpty(top.Text))
                node["Style"]["top"].Value = top.Text + "px";

            if (textAlign.SelectedIndex != 0)
                node["Style"]["text-align"].Value = textAlign.SelectedItem.Value;

            if (textVerticalAlign.SelectedIndex != 0)
                node["Style"]["vertical-align"].Value = textVerticalAlign.SelectedItem.Value;

            if (!string.IsNullOrEmpty(fgText.Style[Styles.backgroundColor]))
                node["Style"]["color"].Value = fgText.Style[Styles.backgroundColor];

            if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundColor]))
            {
                node["Style"]["background-color"].Value = bgText.Style[Styles.backgroundColor];
            }
            else if (!string.IsNullOrEmpty(bgText.Style[Styles.backgroundImage]))
            {
                node["Style"]["background-image"].Value = bgText.Style[Styles.backgroundImage];
                node["Style"]["background-attachment"].Value = "scroll";
                node["Style"]["background-position"].Value = "0 0";
                node["Style"]["background-repeat"].Value = "no-repeat";
                node["Style"]["background-color"].Value = "";
            }

            if (!string.IsNullOrEmpty(shadowColor.Style[Styles.backgroundColor]))
            {
                string stl = "";
                stl += shadowHorizontalOffset.Text + "px ";
                stl += shadowVerticalOffset.Text + "px ";
                stl += shadowBlur.Text + "px ";
                stl += shadowColor.Style[Styles.backgroundColor];

                node["Style"]["box-shadow"].Value = stl;
            }
            if (!string.IsNullOrEmpty(gradientStart.Style[Styles.backgroundColor]))
            {
                string gradient = string.Format("linear-gradient({0} 0%, {1} 100%)",
                    gradientStart.Style[Styles.backgroundColor],
                    gradientStop.Style[Styles.backgroundColor]);
                if (node["Style"]["background-image"].Value != null)
                    gradient = node["Style"]["background-image"].Value as string + "," + gradient;
                node["Style"]["background-image"].Value = gradient;
            }

            string rounded = "";
            if (!string.IsNullOrEmpty(roundedCornersTopLeft.Text))
                rounded += roundedCornersTopLeft.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersTopRight.Text))
                rounded += roundedCornersTopRight.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersBottomRight.Text))
                rounded += roundedCornersBottomRight.Text + "px ";
            else
                rounded += "0px ";

            if (!string.IsNullOrEmpty(roundedCornersBottomLeft.Text))
                rounded += roundedCornersBottomLeft.Text + "px ";
            else
                rounded += "0px";

            if (rounded != "0px 0px 0px 0px")
            {
                node["Style"]["border-radius"].Value = rounded;
            }

            node["ID"].Value = DataSource["ID"].Get<int>();

            RaiseSafeEvent(
                "Magix.MetaForms.SetWidgetStyles",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");

            RaiseSafeEvent("Magix.MetaForms.RefreshEditableMetaForm");
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
