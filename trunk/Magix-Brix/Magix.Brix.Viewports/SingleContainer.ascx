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
    Delay="2500"
    MaxOpacity="0.8"
    id="waiter">
    <h2>Please wait while Marvin is thinking ...</h2>
    <img src="media/images/animated_brain.gif" alt="Marvin's brain ..." />
</mux:AjaxWait>

<mux:Panel
    runat="server"
    id="wrp"
    CssClass="container main">
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        id="content1" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        id="content2" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        id="content3" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        id="content4" />
    <mux:DynamicPanel 
        runat="server" 
        OnReload="dynamic_LoadControls"
        id="content5" />
    <mux:Panel
        runat="server"
        CssClass="span-24 last childContainer"
        id="pnlAll" />
    <mux:Window 
        runat="server" 
        CssClass="mux-shaded mux-rounded mux-window message push-6 span-8 last"
        Caption="Message from Marvin ..."
        style="position:fixed;display:none;top:54px;"
        Closable="false"
        id="message">
        <Content>
            <mux:Label 
                runat="server" 
                id="msgLbl" />
        </Content>
    </mux:Window>
</mux:Panel>
