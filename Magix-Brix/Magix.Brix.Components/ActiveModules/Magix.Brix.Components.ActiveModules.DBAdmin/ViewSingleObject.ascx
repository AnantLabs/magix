<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewSingleObject" %>

<link href="media/modules/grids.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="mux-grid-single-object"
    id="pnl" />

<mux:Button
    runat="server"
    id="change"
    CssClass="mux-paging-button mux-button-change span-3"
    OnClick="change_Click"
    Text="Change ..." />

<mux:Button
    runat="server"
    id="remove"
    CssClass="mux-paging-button mux-button-remove span-3"
    OnClick="remove_Click"
    Text="Remove" />

<mux:Button
    runat="server"
    id="delete"
    CssClass="mux-paging-button mux-button-delete span-3"
    OnClick="delete_Click"
    Text="Delete" />

