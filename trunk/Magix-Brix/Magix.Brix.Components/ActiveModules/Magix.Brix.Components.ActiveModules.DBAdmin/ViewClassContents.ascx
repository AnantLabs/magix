<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents" %>

<link href="media/modules/grids.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    id="root"
    CssClass="mux-class-contents">

    <mux:Panel
        runat="server"
        CssClass="mux-class-contents-rows"
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
        CssClass="mux-paging-button mux-button-beginning span-1"
        Text="&lt;&lt;"
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
</mux:Panel>
