<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="mux-grid-single-object"
    id="pnl" />

<mux:Button
    runat="server"
    id="change"
    CssClass="mux-paging-button mux-button-change span-1"
    OnClick="change_Click"
    Text="Change..." />

<mux:Button
    runat="server"
    id="remove"
    CssClass="mux-paging-button mux-button-remove span-1"
    OnClick="remove_Click"
    Text="Remove Reference" />
