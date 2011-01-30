<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="panelSingleObject"
    id="pnl">
</mux:Panel>

<mux:Panel
    runat="server"
    id="changePnl"
    CssClass="pagingButton change">
    <mux:Button
        runat="server"
        id="change"
        OnClick="change_Click"
        Text="Change..." />
</mux:Panel>

<mux:Panel
    runat="server"
    id="removePnl"
    CssClass="pagingButton remove">
    <mux:Button
        runat="server"
        id="remove"
        OnClick="remove_Click"
        Text="Remove Reference" />
</mux:Panel>

