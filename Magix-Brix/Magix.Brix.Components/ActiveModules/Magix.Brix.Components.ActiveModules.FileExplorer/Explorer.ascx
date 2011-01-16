<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.FileExplorer" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.FileExplorer.Explorer" %>

<link href="media/modules/FileExplorer.css" rel="stylesheet" type="text/css" />


<mux:Panel
    runat="server"
    CssClass="span-12 fileExplorer"
    id="pnl" />

<mux:Panel
    runat="server"
    id="prop"
    CssClass="span-8 last fileExplorer-properties">
    <mux:Label
        runat="server"
        id="header"
        CssClass="header"
        Tag="h2" />
    <mux:Label 
        runat="server"
        id="extension"
        CssClass="extension"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="size"
        CssClass="size"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="imageSize"
        Tag="p"
        CssClass="imageSize" />
    <p>
        <mux:LinkButton 
            runat="server"
            id="imageLink"
            OnClick="imageLink_Click"
            Target="_blank"
            CssClass="imageLink" />
    </p>
    <mux:Label
        runat="server"
        id="imageWarning"
        Tag="p"
        CssClass="imageWarning" />
</mux:Panel>




