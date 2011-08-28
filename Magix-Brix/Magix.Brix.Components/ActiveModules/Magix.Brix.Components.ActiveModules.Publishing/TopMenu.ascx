<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.TopMenu" %>

<link href="media/modules/web-part-templates.css" rel="stylesheet" type="text/css" />

<mux:Menu 
    runat="server" 
    OnMenuItemClicked="slid_OnMenuItemClicked"
    ExpansionMode="Hover"
    ID="slid">
</mux:Menu>
