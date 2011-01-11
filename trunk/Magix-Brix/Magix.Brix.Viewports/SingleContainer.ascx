<%@ Assembly 
    Name="Magix.Brix.Viewports" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Viewports.SingleContainer" %>

<link href="media/modules/SingleContainer.css" rel="stylesheet" type="text/css" />

<mux:AjaxWait 
    runat="server" 
    CssClass="ajax-wait"
    MaxOpacity="0.8"
    id="waiter">
    <h2>Please wait while Marvin is thinking ...</h2>
    <img src="media/images/animated_brain.gif" alt="Marvin's brain ..." />
</mux:AjaxWait>

<div class="container showgrid">
    <div class="span-24 prepend-top" style="height:500px;">
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
            style="display:none;z-index:10000;"
            Caption="Message from system"
            Closable="false"
            id="message">
            <Content>
                <mux:Label 
                    runat="server" 
                    id="msgLbl" />
            </Content>
        </mux:Window>
    </div>
</div>
