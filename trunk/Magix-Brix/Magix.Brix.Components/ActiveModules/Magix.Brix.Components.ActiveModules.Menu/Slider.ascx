<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Menu" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Menu.Slider" %>

<link href="media/modules/SlidingMenu.css" rel="stylesheet" type="text/css" />

<mux:SlidingMenu 
    runat="server" 
    OnLeafMenuItemClicked="slid_LeafMenuItemClicked"
    ID="slid">
    <mux:SlidingMenuLevel 
        runat="server" 
        ID="root" />
</mux:SlidingMenu>
