<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaView" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaView.MetaView_Single" %>

<link href="media/modules/MetaView.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="mux-web-part-content"
    id="ctrls" />

<mux:HiddenField
    runat="server"
    id="oId"
    Value="0" />

