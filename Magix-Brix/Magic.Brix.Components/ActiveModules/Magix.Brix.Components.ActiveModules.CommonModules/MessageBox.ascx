<%@ Assembly 
    Name="WineTasting.Modules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.MessageBox" %>

<mux:Window 
    runat="server" 
    Closable="true"
    style="position:absolute;top:25px;right:25px;z-index:1000;color:Black;width:350px;" 
    CssClass="mux-shaded mux-rounded wine-admin-message-box" 
    id="wnd">
    <Content>
        <mux:Label
            runat="server"
            id="lbl" />
        <mux:Button
            runat="server"
            id="ok"
            CssClass="ok"
            OnClick="ok_Click"
            Text="OK" />
        <mux:Button
            runat="server"
            id="cancel"
            CssClass="cancel"
            OnClick="cancel_Click"
            Text="Cancel" />
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>
