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
            Text="Margs"
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
                ID="mpv1">
                <div class="mux-insert first">
                    Hello there ...
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

