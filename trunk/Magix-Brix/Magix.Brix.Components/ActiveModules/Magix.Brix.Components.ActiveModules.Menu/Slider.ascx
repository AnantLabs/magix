<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Menu" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Menu.Slider" %>

<link href="media/modules/sliding-menu.css" rel="stylesheet" type="text/css" />

<mux:SlidingMenu 
    runat="server" 
    OnLeafMenuItemClicked="slid_LeafMenuItemClicked"
    CssClass="mux-sliding-menu mux-rounded mux-shaded"
    ID="slid">
    <mux:SlidingMenuLevel 
        runat="server" 
        ID="root" />
</mux:SlidingMenu>
