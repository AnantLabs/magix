<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaView" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaView.SingleView" %>

<link href="media/modules/MetaView.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    id="wrp"
    CssClass="span-10">
    <mux:Panel
        runat="server"
        CssClass="span-10 padding-6 last down-1 clear-left"
        id="ctrls" />
</mux:Panel>
