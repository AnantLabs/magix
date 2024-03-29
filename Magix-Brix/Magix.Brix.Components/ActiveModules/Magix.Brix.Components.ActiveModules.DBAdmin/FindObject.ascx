﻿<%@ Assembly 
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
            OnClick="CreateItem"
            ToolTip="Click to create a New Object ..."
            Text="+"
            AccessKey="v"
            CssClass="mux-paging-button mux-button-plus span-1"
            id="create" />

        <mux:Button
            runat="server"
            OnClick="FirstItems"
            AccessKey="g"
            Text="&lt;&lt;"
            CssClass="mux-paging-button mux-button-beginning span-1"
            id="beginning" />

        <mux:Button
            runat="server"
            OnClick="PreviousItems"
            AccessKey="h"
            Text="&lt;"
            CssClass="mux-paging-button mux-button-previous span-1"
            id="previous" />

        <mux:Button
            runat="server"
            OnClick="NextItems"
            AccessKey="j"
            Text="&gt;"
            CssClass="mux-paging-button mux-button-next span-1"
            id="next" />

        <mux:Button
            runat="server"
            OnClick="EndItems"
            AccessKey="k"
            Text="&gt;&gt;"
            CssClass="mux-paging-button mux-button-end span-1"
            id="end" />

        <mux:Label
            runat="server"
            CssClass="mux-paging-count"
            id="lblCount" />
    </mux:Panel>

</mux:Panel>
