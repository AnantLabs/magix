<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassContents" %>

<mux:Button
    runat="server"
    id="focs"
    style="margin-left:-4000px;position:absolute;" />

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
    id="beginning" />

<mux:Button
    runat="server"
    OnClick="PreviousItems"
    CssClass="pagingButton"
    Text="&lt;"
    id="previous" />

<mux:Button
    runat="server"
    OnClick="NextItems"
    CssClass="pagingButton"
    Text="&gt;"
    id="next" />

<mux:Button
    runat="server"
    OnClick="EndItems"
    CssClass="pagingButton"
    Text="&gt;&gt;"
    id="end" />

<mux:Button
    runat="server"
    OnClick="CreateItem"
    Text="+"
    CssClass="createButton"
    id="create" />


