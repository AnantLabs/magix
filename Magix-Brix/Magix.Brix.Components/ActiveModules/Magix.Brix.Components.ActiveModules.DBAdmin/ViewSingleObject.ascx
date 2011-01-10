<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject" %>

<mux:Panel
    runat="server"
    style="font-size:small;"
    id="pnl">
</mux:Panel>

<mux:Button
    runat="server"
    id="focs"
    style="margin-left:-4000px;position:absolute;" />

<mux:Button
    runat="server"
    id="change"
    OnClick="change_Click"
    Text="Change..." />

<mux:Button
    runat="server"
    id="remove"
    OnClick="remove_Click"
    Text="Remove Reference" />


