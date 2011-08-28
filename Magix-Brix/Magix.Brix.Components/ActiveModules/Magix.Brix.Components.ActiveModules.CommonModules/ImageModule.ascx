<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ImageModule" %>

<mux:Panel
    runat="server"
    id="root">
    <mux:Image
        runat="server"
        id="img" />

    <mux:Label
        runat="server"
        Tag="p"
        CssClass="imageDescription small"
        id="lbl" />
</mux:Panel>
