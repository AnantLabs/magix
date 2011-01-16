<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.FileExplorer" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.FileExplorer.Explorer" %>

<link href="media/modules/FileExplorer.css" rel="stylesheet" type="text/css" />

<div class="span-12">
    <mux:Panel
        runat="server"
        CssClass="span-12 fileExplorer"
        id="pnl" />
    <mux:Button
        runat="server"
        id="previous"
        CssClass="span-2 previous"
        Text="&lt;" />

    <mux:Button
        runat="server"
        id="next"
        CssClass="span-2 next"
        Text="&gt;" />
</div>

<mux:Panel
    runat="server"
    id="prop"
    CssClass="span-8 last fileExplorer-properties">
    <mux:Label
        runat="server"
        id="header"
        CssClass="header"
        Tag="h3" />
    <p>
        <mux:InPlaceEdit
            runat="server"
            OnTextChanged="name_TextChanged"
            id="name" />
    </p>
    <mux:Label 
        runat="server"
        id="extension"
        CssClass="extension small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="size"
        CssClass="size small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="fullUrl"
        CssClass="fullUrl small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="imageSize"
        Tag="p"
        CssClass="imageSize small" />
    <p class="link">
        <mux:LinkButton 
            runat="server"
            id="imageLink"
            OnClick="imageLink_Click"
            Target="_blank"
            CssClass="imageLink" />
    </p>
    <div class="span-4 height-8">
        <mux:Image
            runat="server"
            OnClick="preview_Click"
            id="preview" />
    </div>
    <div class="span-4 last">
        <mux:Label
            runat="server"
            id="imageWarning"
            Tag="p"
            CssClass="imageWarning small" />
    </div>
</mux:Panel>






