<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewListOfObjects" %>

<link href="media/modules/grids.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    id="pnl" />

<mux:Button
    runat="server"
    OnClick="PreviousItems"
    CssClass="mux-paging-button previous span-1"
    Text="&lt;"
    id="previous" />

<mux:Button
    runat="server"
    OnClick="NextItems"
    CssClass="mux-paging-button mux-button-next span-1"
    Text="&gt;"
    id="next" />

<mux:Button
    runat="server"
    id="append"
    Text="Append..."
    CssClass="mux-paging-button mux-button-append span-1"
    ToolTip="Click to Append either a New or an Existing Object to List of Objects ..."
    OnClick="append_Click" />
