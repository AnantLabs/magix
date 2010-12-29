<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewInstances" %>

<mux:Panel
    runat="server"
    style="font-size:small;"
    id="pnl">
</mux:Panel>
<mux:Button
    runat="server"
    OnClick="PreviousItems"
    id="previous" />

<mux:Button
    runat="server"
    OnClick="NextItems"
    id="next" />

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded"
    style="left:150px;top:100px;position:fixed;z-index:1000;width:750px;height:500px;"
    Caption="Details..."
    Visible="false"
    OnClosed="wnd_Closed"
    id="wnd">
    <Content>
        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            style="overflow:auto;width:700px;height:450px;margin-left:auto;margin-right:auto;"
            OnReload="child_LoadControls"
            id="child" />
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>



