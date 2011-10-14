<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.FindObject" %>

<link href="media/modules/grids.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    DefaultWidget="ok"
    id="pnlWrp">

    <mux:TextBox
        runat="server"
        id="query"
        AccessKey="S"
        PlaceHolder="Search ..." />

    <mux:Button
        runat="server"
        id="ok"
        style="margin-left:-4000px;"
        OnClick="ok_Click" />

    <mux:Panel
        runat="server"
        id="root"
        CssClass="dbAdminTableWrapper">

        <mux:Panel
            runat="server"
            id="pnl" />

        <mux:Button
            runat="server"
            OnClick="FirstItems"
            Text="&lt;&lt;"
            CssClass="mux-paging-button mux-button-beginning span-1"
            id="beginning" />

        <mux:Button
            runat="server"
            OnClick="PreviousItems"
            Text="&lt;"
            CssClass="mux-paging-button mux-button-previous span-1"
            id="previous" />

        <mux:Button
            runat="server"
            OnClick="NextItems"
            Text="&gt;"
            CssClass="mux-paging-button mux-button-next span-1"
            id="next" />

        <mux:Button
            runat="server"
            OnClick="EndItems"
            Text="&gt;&gt;"
            CssClass="mux-paging-button mux-button-end span-1"
            id="end" />
    </mux:Panel>

</mux:Panel>
