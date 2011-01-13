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

<div class="container main" style="height:500px;">
    <mux:DynamicPanel 
        runat="server" 
        CssClass="span-12 push-6 down-3 last"
        OnReload="dynamic_LoadControls"
        id="content" />
    <mux:Panel
        runat="server"
        CssClass="span-22 push-1 last childContainer"
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
