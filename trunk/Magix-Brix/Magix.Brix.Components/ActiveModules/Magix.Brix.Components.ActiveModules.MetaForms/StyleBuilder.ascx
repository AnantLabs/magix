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
            ToolTip="Allows you to attach Animations and such for your Widget. [ Be Careful!! These guys tends to steal a lot of battery and power, especially on smaller devices, which again increases our collective Carbon Footprint ... They are fun [Animations], I know. And I want you to have fun! Just don't 'blow up the world' while you're 'having fun' due to increasing our collective energy usage by orders of magnitudes due to 'funny Animations'... ;) ]"
            Text="Animations"
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
                            style="margin-bottom:18px;"
                            CssClass="span-2 last" />
                        <mux:Panel
                            runat="server"
                            CssClass="span-3 height-1 texture-panel last"
                            OnClick="borderColorPnl_Click"
                            ToolTip="Click to Change Color for Borders"
                            style="background-color:Black;color:White;"
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
                            ToolTip="Whether or not the Widget will be in 'float mode' or not. Float mode means that it'll float to the left as long as there's space enough to display it in its entirety, including its margins and such to the right of the last element within the same float hierarchy. To use floating of elements, you need to also use Block Displaying of the widget, plus also an explicit width or height of the element" />
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
                            Text="Display Block"
                            Tag="label"
                            For="chkBlock"
                            CssClass="span-3 last"
                            ToolTip="If true, the Widget will be rendered as a 'Block Level Element', which among other things is a prerequisite for being able to 'float' the element" />
                    </div>
                    <p class="span-16 clear-both last mux-info-text" title="... Sorry guys. But hey, feel free to override it somehow, and change it if you like to :)">
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
                ID="mpv2">
                <div class="mux-insert">
                    Hello there ...
                </div>
            </mux:MultiPanelView>
            <mux:MultiPanelView 
                runat="server" 
                ID="mpv3">
                <div class="mux-insert">
                    <mux:Panel
                        runat="server"
                        CssClass="span-2 height-4 texture-panel"
                        OnClick="fgText_Click"
                        ToolTip="Click to Change Color for Widget"
                        id="fgText">
                        Foreground Color
                    </mux:Panel>
                    <mux:Panel
                        runat="server"
                        CssClass="span-2 height-4 texture-panel"
                        OnClick="bgText_Click"
                        ToolTip="Click to Change Background Color or Background Image for your Widget"
                        id="bgText">
                        Background Color/Image
                    </mux:Panel>
                </div>
            </mux:MultiPanelView>
            <mux:MultiPanelView 
                runat="server" 
                ID="mpv4">
                <div class="mux-insert">
                    Hello there ...
                </div>
            </mux:MultiPanelView>
        </Content>
    </mux:MultiPanel>
</div>

