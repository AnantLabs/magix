<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Documentation" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Documentation.ShowClassDetails" %>


<div class="span-9 last;">
    <mux:Label
        runat="server"
        id="header"
        CssClass="span-9 last"
        style="display:block;"
        Tag="h3" />
    <mux:Label
        runat="server"
        id="content"
        Tag="div" />
</div>
