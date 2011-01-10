<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents" %>

<mux:Panel
    runat="server"
    style="font-size:small;"
    id="pnl">
</mux:Panel>
<mux:Button
    runat="server"
    OnClick="FirstItems"
    CssClass="pagingButton firstPagingButton"
    Text="&lt;&lt;"
    ToolTip="First items"
    id="beginning" />

<mux:Button
    runat="server"
    OnClick="PreviousItems"
    CssClass="pagingButton"
    Text="&lt;"
    ToolTip="Previous items"
    id="previous" />

<mux:Button
    runat="server"
    OnClick="NextItems"
    CssClass="pagingButton"
    Text="&gt;"
    ToolTip="Next items"
    id="next" />

<mux:Button
    runat="server"
    OnClick="EndItems"
    CssClass="pagingButton"
    Text="&gt;&gt;"
    ToolTip="Last items"
    id="end" />

<mux:Button
    runat="server"
    OnClick="CreateItem"
    Text="Create new item..."
    id="create" />


