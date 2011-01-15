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

<div class="container main showgrid" style="height:1000px;">
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        style="position:absolute;"
        id="content1" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        style="position:absolute;"
        id="content2" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        style="position:absolute;"
        id="content3" />
    <mux:Panel
        runat="server"
        CssClass="span-24 last childContainer"
        id="pnlAll" />
    <mux:Window 
        runat="server" 
        CssClass="mux-shaded mux-rounded mux-window message prepend-top push-8 span-8 last"
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
