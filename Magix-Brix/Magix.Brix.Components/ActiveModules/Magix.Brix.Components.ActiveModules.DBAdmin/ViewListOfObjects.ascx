<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewListOfObjects" %>

<mux:Button
    runat="server"
    id="focs"
    style="margin-left:-4000px;position:absolute;" />

<mux:Panel
    runat="server"
    id="pnl" />

<mux:Panel
    runat="server"
    id="previousPnl"
    CssClass="pagingButton previous">
    <mux:Button
        runat="server"
        OnClick="PreviousItems"
        Text="&lt;"
        id="previous" />
</mux:Panel>

<mux:Panel
    runat="server"
    id="nextPnl"
    CssClass="pagingButton next">
    <mux:Button
        runat="server"
        OnClick="NextItems"
        Text="&gt;"
        id="next" />
</mux:Panel>

<mux:Panel
    runat="server"
    id="appendPnl"
    CssClass="pagingButton append">
    <mux:Button
        runat="server"
        id="append"
        Text="Append..."
        OnClick="append_Click" />
</mux:Panel>


