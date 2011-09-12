<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaForms" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaForms.StyleBuilder" %>

<link href="media/modules/edit-meta-forms.css" rel="stylesheet" type="text/css" />

<div class="mux-multi-panel-wrapper mux-style-builder">
    <mux:TabStrip 
        runat="server" 
        MultiPanelID="mp"
        OnMultiButtonClicked="mp_MultiButtonClicked"
        ID="mub">
        <mux:TabButton 
            runat="server" 
            ToolTip="Allows you to set margins, padding and border of Widget"
            Text="Spacing"
            ID="mb1" />
        <mux:TabButton
            runat="server" 
            ToolTip="Allows you to change name, size and style of the Font used in the Widget"
            Text="Typography"
            ID="mb2" />
        <mux:TabButton
            runat="server" 
            ToolTip="Allows you to change Bling of Widget, such as color, background-color, background images, 3D Shadow, rounded borders, etc"
            Text="Bling"
            ID="mb3" />
        <mux:TabButton
            runat="server" 
            ToolTip="Allows you to attach Animations and such for your Widget, before you wrap up. [ Be Careful!! These guys tends to steal a lot of battery and power, especially on smaller devices, which again increases our collective Carbon Footprint ... They are fun [Animations], I know. And I want you to have fun! Just don't 'blow up the world' while you're 'having fun' due to increasing our collective energy usage by orders of magnitudes due to 'funny Animations'... ;) ]"
            Text="Finish"
            ID="mb4" />
    </mux:TabStrip>
    <mux:MultiPanel 
        runat="server" 
        AnimationMode="Slide"
        ID="mp">
        <Content>
            <mux:MultiPanelView 
                runat="server"
                DefaultWidget="next1"
                ID="mpv1">
                <div class="mux-insert first">
                    <div class="span-4 mux-editable-part">
                        <h5 title="Margin are outside of borders and basically serves as spacing between your widgets. Useful for creating space between widgets as a[n] {much better!} alternative to absolute positioning of Widgets">Margins</h5>
                        <span class="span-2">
                            Left: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="marginLeft"
                            PlaceHolder="Left ..."
                            ToolTip="Left Margin ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Top: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="marginTop"
                            PlaceHolder="Top ..."
                            ToolTip="Top Margin ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Right: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="marginRight"
                            PlaceHolder="Right ..."
                            ToolTip="Right Margin ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Bottom: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="marginBottom"
                            PlaceHolder="Bottom ..."
                            ToolTip="Bottom Margin ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="The Borders around the Widget. Borders are rendered between the Padding and the Margin of the Widget. Meaning just outside of the background-color parts">Borders</h5>
                        <span class="span-2">
                            Width: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="borderWidth"
                            TextMode="Number"
                            PlaceHolder="Width ..."
                            ToolTip="Width of Borders ..."
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Style: 
                        </span>
                        <mux:SelectList
                            runat="server"
                            ToolTip="Select a Border Style for your Widget ... "
                            style="margin-bottom:18px;"
                            CssClass="span-2 last"
                            id="borderStyle">
                            <mux:ListItem Text="Solid" Value="solid" />
                            <mux:ListItem Text="Dashed" Value="dashed" />
                            <mux:ListItem Text="Dotted" Value="dotted" />
                            <mux:ListItem Text="Double" Value="double" />
                            <mux:ListItem Text="Groove" Value="groove" />
                            <mux:ListItem Text="Hidden" Value="hidden" />
                            <mux:ListItem Text="Inset" Value="inset" />
                            <mux:ListItem Text="Outset" Value="outset" />
                            <mux:ListItem Text="Ridge" Value="ridge" />
                        </mux:SelectList>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            OnClick="borderColorPnl_Click"
                            ToolTip="Click to Change Color for Borders"
                            style="padding-top:9px;padding-bottom:9px;"
                            id="borderColorPnl">
                            Border Color
                        </mux:Panel>
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="Padding is spacing between inside of the element before children or textual fragments are rendered. Useful for creating distance between the borders of a Widget and where child fragments and text properties are being rendered">Padding</h5>
                        <span class="span-2">
                            Left: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="paddingLeft"
                            PlaceHolder="Left ..."
                            ToolTip="Left Padding ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Top: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="paddingTop"
                            PlaceHolder="Top ..."
                            ToolTip="Top Padding ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Right: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="paddingRight"
                            PlaceHolder="Right ..."
                            ToolTip="Right Padding ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Bottom: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="paddingBottom"
                            PlaceHolder="Bottom ..."
                            ToolTip="Bottom Padding ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                    </div>
                    <div class="span-4 mux-editable-part last">
                        <h5 title="Miscelanous properties of importance, but not really belonging to any particular category">Misc.</h5>
                        <mux:CheckBox
                            runat="server"
                            style="width:20px;display:block;float:left;"
                            id="chkFloat" />
                        <mux:Label
                            runat="server"
                            id="lblChkFloat"
                            Text="Float Left"
                            Tag="label"
                            For="chkFloat"
                            CssClass="span-3 last"
                            ToolTip="Whether or not the Widget will be in 'float mode' or not. Float mode means that it'll float to the left as long as there's space enough to display it in its entirety, including its margins and such to the right of the last element within the same float hierarchy. To use floating of elements, you need to also use Block Displaying of the widget, plus also an explicit width or height of the element. Float mode is in general i good thing, and the way you _should_ prefer, in general, to display your Widgets, since it give you more flexibility later" />
                        <mux:CheckBox
                            runat="server"
                            style="width:20px;display:block;float:left;clear:both;"
                            id="chkClear" />
                        <mux:Label
                            runat="server"
                            id="lblChkClear"
                            Text="Clear Floating"
                            Tag="label"
                            For="chkClear"
                            CssClass="span-3 last"
                            ToolTip="Whether or not the Widget will clear the floating hierarchy before being rendered. Think of clearing the floating hierarchy as a very hard Carriage Return that will bring you all the way to the left, regardless of whether or not there is room to the right of whatever widget you're currently to the right of" />
                        <mux:CheckBox
                            runat="server"
                            style="width:20px;display:block;float:left;clear:both;"
                            id="chkBlock" />
                        <mux:Label
                            runat="server"
                            id="lblChkBlock"
                            Text="Force Block"
                            Tag="label"
                            For="chkBlock"
                            CssClass="span-3 last"
                            ToolTip="If true, the Widget will be rendered as a 'Block Level Element', which among other things is a prerequisite for being able to 'float' the element" />
                        <mux:CheckBox
                            runat="server"
                            style="width:20px;display:block;float:left;clear:both;"
                            id="chkInline" />
                        <mux:Label
                            runat="server"
                            id="lblChkInline"
                            Text="Force Inline"
                            Tag="label"
                            For="chkInline"
                            CssClass="span-3 last"
                            ToolTip="If true, the Widget will be rendered as an 'Inline Level Element', which is the opposite of Block Level Element, and intended for widgets that are supposed to just be fragments of other widgets" />
                    </div>
                    <p class="span-6 clear-both last mux-info-text" title="... Sorry guys. But hey, feel free to override it somehow, and change it if you like to :)">
                        (*) All units are in pixels ...
                    </p>
                    <mux:Button
                        runat="server"
                        Text="Next >>"
                        OnClick="next1_Click"
                        CssClass="span-4 last next-button"
                        ToolTip="Brings you to the next section of the Style Builder Wizard ..."
                        id="next1" />
                </div>
            </mux:MultiPanelView>
            <mux:MultiPanelView 
                runat="server" 
                DefaultWidget="next2"
                ID="mpv2">
                <div class="mux-insert">
                    <div class="span-4 mux-editable-part pushLeft-4">
                        <h5 title="The styles for your font, such as Type Name and whether or not it should render Bold, Italics and so on">Style</h5>
                        <span 
                            title="Select a Typeface for your Font ..."
                            class="span-2">Font: </span>
                        <mux:SelectList
                            runat="server"
                            ToolTip="Select a Typeface for your Font ..."
                            CssClass="span-2 last"
                            id="fontName">
                            <mux:ListItem Text="" Value="" />
                            <mux:ListItem Text="Arial" Value="Arial" />
                            <mux:ListItem Text="Helvetica" Value="Helvetica" />
                            <mux:ListItem Text="Times New Roman" Value="Times New Roman" />
                            <mux:ListItem Text="Courier New" Value="Courier New" />
                            <mux:ListItem Text="Palatino" Value="Palatino" />
                            <mux:ListItem Text="Garamond" Value="Garamond" />
                            <mux:ListItem Text="Bookman" Value="Bookman" />
                            <mux:ListItem Text="Avant Garde" Value="Avant Garde" />
                            <mux:ListItem Text="Verdana" Value="Verdana" />
                            <mux:ListItem Text="Georgia" Value="Georgia" />
                            <mux:ListItem Text="Comic Sans MS" Value="Comic Sans MS" />
                            <mux:ListItem Text="Trebuchet MS" Value="Trebuchet MS" />
                            <mux:ListItem Text="Arial Black" Value="Arial Black" />
                            <mux:ListItem Text="Impact" Value="Impact" />
                            <mux:ListItem Text="Lexograph" Value="Lexograph" />
                            <mux:ListItem Text="Bleeding Cowboys" Value="Bleeding Cowboys" />
                            <mux:ListItem Text="Ultra Classic" Value="Ultra Classic" />
                            <mux:ListItem Text="Groovieee" Value="Groovieee" />
                            <mux:ListItem Text="Bananas" Value="Bananas" />
                        </mux:SelectList>
                        <mux:Label
                            runat="server"
                            id="lblChkBold"
                            Text="Bold"
                            Tag="label"
                            For="chkBold"
                            CssClass="span-2"
                            ToolTip="If true, the Widget will be rendered with Bold typeface" />
                        <mux:CheckBox
                            runat="server"
                            style="width:16px;display:block;float:right;margin-right:0;"
                            id="chkBold" />
                        <mux:Label
                            runat="server"
                            id="lblChkItalic"
                            Text="Italic"
                            style="font-weight:normal;font-style:italic;clear:both;"
                            Tag="label"
                            For="chkItalic"
                            CssClass="span-2"
                            ToolTip="If true, the Widget will be rendered with Italic typeface" />
                        <mux:CheckBox
                            runat="server"
                            style="width:16px;display:block;float:right;margin-right:0;"
                            id="chkItalic" />
                        <mux:Label
                            runat="server"
                            id="lblChkUnderline"
                            Text="Underline"
                            style="font-weight:normal;font-style:normal;clear:both;text-decoration:underline;"
                            Tag="label"
                            For="chkUnderline"
                            CssClass="span-2"
                            ToolTip="If true, the Widget will be rendered underlined" />
                        <mux:CheckBox
                            runat="server"
                            style="width:16px;display:block;float:right;margin-right:0;"
                            id="chkUnderline" />
                        <mux:Label
                            runat="server"
                            id="lblChkStrikethrough"
                            Text="Strikethrough"
                            style="font-weight:normal;font-style:normal;clear:both;text-decoration:line-through;"
                            Tag="label"
                            For="chkStrikethrough"
                            CssClass="span-2"
                            ToolTip="If true, the Widget will be rendered with a line crossing out its text out, signyfying e.g. 'erased content' etc" />
                        <mux:CheckBox
                            runat="server"
                            style="width:16px;display:block;float:right;margin-right:0;"
                            id="chkStrikethrough" />
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="Alignment of text within the element and other values, such as height of a line of text, etc">Alignment</h5>
                        <span 
                            title="Size of your Font ..."
                            class="span-2">Size: </span>
                        <mux:TextBox
                            runat="server"
                            id="fontSize"
                            PlaceHolder="Height ..."
                            ToolTip="Font Size ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span 
                            title="Select an Alignment for your Text ..."
                            style="margin-top:18px;"
                            class="span-2">Alignment: </span>
                        <mux:SelectList
                            runat="server"
                            ToolTip="Select an Alignment for your Text ..."
                            CssClass="span-2 last"
                            style="margin-top:18px;"
                            id="textAlign">
                            <mux:ListItem Text="" Value="" />
                            <mux:ListItem Text="Left" value="left" />
                            <mux:ListItem Text="Center" value="center" />
                            <mux:ListItem Text="Justify" value="justify" />
                            <mux:ListItem Text="Right" value="right" />
                        </mux:SelectList>
                        <span 
                            title="Select a Vertical Alignment for your Text ..."
                            style="margin-top:18px;"
                            class="span-2">Vertical: </span>
                        <mux:SelectList
                            runat="server"
                            ToolTip="Select a Vertical Alignment for your Text ..."
                            CssClass="span-2 last"
                            style="margin-top:18px;"
                            id="textVerticalAlign">
                            <mux:ListItem Text="" Value="" />
                            <mux:ListItem Text="Baseline" Value="baseline" />
                            <mux:ListItem Text="Bottom" Value="bottom" />
                            <mux:ListItem Text="Middle" Value="middle" />
                            <mux:ListItem Text="Sub" Value="sub" />
                            <mux:ListItem Text="Super" Value="super" />
                            <mux:ListItem Text="Text Bottom" Value="text-bottom" />
                            <mux:ListItem Text="Text Top" Value="text-top" />
                            <mux:ListItem Text="Top" Value="top" />
                        </mux:SelectList>
                    </div>
                    <p class="span-6 pushLeft-4 clear-both last mux-info-text" title="... Sorry Guys 2.0! ;)">
                        (*) All units are in pixels ...
                    </p>
                    <mux:Button
                        runat="server"
                        Text="Next >>"
                        OnClick="next2_Click"
                        CssClass="span-4 last next-button"
                        ToolTip="Brings you to the next section of the Style Builder Wizard ..."
                        id="next2" />
                </div>
            </mux:MultiPanelView>
            <mux:MultiPanelView 
                runat="server" 
                DefaultWidget="next3"
                ID="mpv3">
                <div class="mux-insert">
                    <div class="span-4 mux-editable-part">
                        <h5 title="The Colors for your Widget, both Foreground color [text-color] and background color or image. You can use an Image as an element's 'background' instead of a color">Colors</h5>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            style="padding-top:9px;padding-bottom:9px;"
                            OnClick="fgText_Click"
                            ToolTip="Click to Change Color for Widget"
                            id="fgText">
                            Text Color
                        </mux:Panel>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            style="margin-top:18px;padding-top:9px;padding-bottom:9px;"
                            OnClick="bgText_Click"
                            ToolTip="Click to Change Background Color or Background Image for your Widget"
                            id="bgText">
                            BG Color / Image
                        </mux:Panel>
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="Shadow for your Widget">Shadow</h5>
                        <span 
                            title="Horizontal Offset. If it's a positive number, the shadow will be that many pixels to the right of the widget. If the offset is negative, the shadow will be to the left of the widget. Doing a 45 degrees shadow with e.g. 3x3x2 will mimick the way the sun sits on the sky psychologically for our retinas, and make the result seem more 'fresh'. According to some theories ... ;)"
                            class="span-2">Horz. Offset: </span>
                        <mux:TextBox
                            runat="server"
                            id="shadowHorizontalOffset"
                            PlaceHolder="Hors."
                            ToolTip="Horizontal Offset ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span 
                            title="Verical Offset. If it's a positive number, the shadow will be that many pixels below the widget. If the offset is negative, the shadow will be above the widget. Doing a 45 degrees shadow with e.g. 3x3x2 will mimicks the way the sun sits on the sky psychologically for our retinas, and make the result seem more 'fresh'. According to some theories ... ;)"
                            class="span-2">Vert. Offset: </span>
                        <mux:TextBox
                            runat="server"
                            id="shadowVerticalOffset"
                            PlaceHolder="Vert."
                            ToolTip="Vertical Offset ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span 
                            title="Blur Radius, the higher the number, the more 'fuzzy edges' your Shadow will have..."
                            class="span-2">Blur: </span>
                        <mux:TextBox
                            runat="server"
                            id="shadowBlur"
                            PlaceHolder="Blur ..."
                            ToolTip="Blur ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        </mux:Panel>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            style="padding-top:9px;padding-bottom:9px;"
                            OnClick="shadowColor_Click"
                            ToolTip="Click to Change the Shadow Color for your Widget"
                            id="shadowColor">
                            Shadow Color
                        </mux:Panel>
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="Gradient Background color settings for your widget, which will come in addition to any images or colors you have previously defined. Meaning, the Gradient will be rendered 'first', and then any other colors and background images will be rendered 'on top of' the gradient, meaning unless your color or image has transparency within, or is completely lacking, then the Gradient won't show">Gradient Background</h5>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            style="padding-top:9px;padding-bottom:9px;margin-bottom:18px;"
                            OnClick="gradientStart_Click"
                            ToolTip="Click to Change starting color for your Gradient"
                            id="gradientStart">
                            Start Color
                        </mux:Panel>
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            style="padding-top:9px;padding-bottom:9px;"
                            OnClick="gradientStop_Click"
                            ToolTip="Click to Change ending color for your Gradient"
                            id="gradientStop">
                            End Color
                        </mux:Panel>
                    </div>
                    <div class="span-4 mux-editable-part">
                        <h5 title="Rounded corners. All values here are 'radius' values, and will basically create rounded corners on your widget. Magix allows for setting the Radius Property for all four different corners to different values, which might create some nifty opportunities for the creative soul ... ;)">Rounded Corners</h5>
                        <span class="span-2">
                            Top Left: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="roundedCornersTopLeft"
                            PlaceHolder="T-L ..."
                            ToolTip="Top Left Corner Radius ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Top Right: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="roundedCornersTopRight"
                            PlaceHolder="T-R ..."
                            ToolTip="Top Right Corner Radius ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Bot. Right: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="roundedCornersBottomRight"
                            PlaceHolder="B-R ..."
                            ToolTip="Bottom Right Corner Radius ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                        <span class="span-2">
                            Bot. Left: 
                        </span>
                        <mux:TextBox
                            runat="server"
                            id="roundedCornersBottomLeft"
                            PlaceHolder="B-L ..."
                            ToolTip="Bottom Left Corner Radius ..."
                            TextMode="Number"
                            CssClass="span-2 last" />
                    </div>
                    <p class="span-6 clear-both last mux-info-text" title="... Sorry Guys 3.0! ;)">
                        (*) All units are in pixels ...
                    </p>
                    <mux:Button
                        runat="server"
                        Text="Next >>"
                        OnClick="next3_Click"
                        CssClass="span-4 last next-button"
                        ToolTip="Brings you to the next section of the Style Builder Wizard ..."
                        id="next3" />
                </div>
            </mux:MultiPanelView>
            <mux:MultiPanelView 
                runat="server" 
                DefaultWidget="finish"
                ID="mpv4">
                <div class="mux-insert">
                    <div class="span-4 mux-editable-part">
                        <span class="span-2">
                            Animations:
                        </span>
                        <mux:SelectList
                            runat="server"
                            ToolTip="Add an Animation to your Widget from your Animation Storage ... "
                            CssClass="span-2 last"
                            OnSelectedIndexChanged="animations_SelectedIndexChanged"
                            id="animations">
                            <mux:ListItem Text="" Value="" />
                        </mux:SelectList>
                        <p class="span-4 last mux-explanation">
                            Above are your pre-defined Animations which you can choose 
                            from. Be _careful_ with Animations. They tend to use much 
                            power on Smart Phones, Tablets but also other computers.
                        </p>
                        <p class="span-4 last mux-explanation">
                            You can create your own Animation using the Animation Builder ...
                        </p>
                    </div>
                    <mux:Panel
                        runat="server"
                        id="preview"
                        CssClass="span-15 height-10 last yellow-background mux-widget-preview" />
                    <mux:Button
                        runat="server"
                        Text="Finish!"
                        OnClick="finish_Click"
                        CssClass="span-4 last next-button"
                        ToolTip="Wraps it up, and saves the Styles as properties onto the Widget. PS! This might _overwrite_ any existing settings you have in the Style collection of your Widget ... !!"
                        id="finish" />
                </div>
            </mux:MultiPanelView>
        </Content>
    </mux:MultiPanel>
</div>

