<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureFilters" %>

<mux:Panel
    runat="server"
    DefaultWidget="ok"
    id="pnl">
    <mux:Button
        runat="server"
        id="ok"
        CssClass="ok"
        Text="OK"
        OnClick="ok_Click" />
    <mux:Button
        runat="server"
        CssClass="cancel"
        id="cancel"
        Text="Cancel"
        OnClick="cancel_Click" />
</mux:Panel>
