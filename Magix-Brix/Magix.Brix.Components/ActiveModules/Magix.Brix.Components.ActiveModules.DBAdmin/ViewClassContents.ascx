<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    id="root"
    CssClass="dbAdminTableWrapper">
    <mux:Button
        runat="server"
        id="focs"
        style="margin-left:-4000px;position:absolute;" />

    <mux:Panel
        runat="server"
        id="pnl" />

    <mux:Panel
        runat="server"
        id="createPnl"
        CssClass="pagingButton plus">
        <mux:Button
            runat="server"
            OnClick="CreateItem"
            Text="+"
            id="create" />
    </mux:Panel>

    <mux:Panel
        runat="server"
        id="beginningPnl"
        CssClass="pagingButton beginning">
        <mux:Button
            runat="server"
            OnClick="FirstItems"
            Text="&lt;&lt;"
            id="beginning" />
    </mux:Panel>

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
        id="endPnl"
        CssClass="pagingButton end">
        <mux:Button
            runat="server"
            OnClick="EndItems"
            Text="&gt;&gt;"
            id="end" />
    </mux:Panel>
</mux:Panel>

