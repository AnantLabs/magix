<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.SliderMenu" %>

<link href="media/modules/SlidingMenu.css" rel="stylesheet" type="text/css" />

<mux:SlidingMenu 
    runat="server" 
    OnMenuItemClicked="slid_MenuItemClicked"
    ID="slid">
    <mux:SlidingMenuLevel 
        runat="server" 
        ID="root" />
</mux:SlidingMenu>










