<%@ Assembly 
    Name="Magic.Brix.Components.ActiveModules.Users" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magic.Brix.Components.ActiveModules.Users.Logout" %>

<mux:Window 
    runat="server" 
    Visible="true"
    style="position:absolute;top:25px;left:750px;z-index:500;color:Black;" 
    CssClass="mux-shaded mux-rounded transparent-window-small"
    Closable="false"
    Caption="Logout"
    ID="wndLogout">
    <Content>
        <mux:Button 
            runat="server" 
            id="logout" 
            Text="Logout" 
            OnClick="logout_Click" />
    </Content>
</mux:Window>

