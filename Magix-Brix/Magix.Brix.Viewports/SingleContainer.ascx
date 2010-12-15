<%@ Assembly 
    Name="Magix.Brix.Viewports" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Viewports.SingleContainer" %>

<mux:Panel 
    runat="server" 
    CssClass="wine-rater-main-all"
    id="pnlAll">
    <div class="wine-rater-main-content">
        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            OnReload="dynamic_LoadControls"
            id="dyn" />
    </div>
</mux:Panel>

<mux:DynamicPanel 
    runat="server" 
    CssClass="dynamic"
    OnReload="dynamic_LoadControls"
    id="dyn2" />

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded mux-window wine-window"
    style="display:none;"
    Caption="Message from system"
    Closable="false"
    id="message">
    <Content>
        <mux:Label 
            runat="server" 
            id="msgLbl" />
    </Content>
</mux:Window>

