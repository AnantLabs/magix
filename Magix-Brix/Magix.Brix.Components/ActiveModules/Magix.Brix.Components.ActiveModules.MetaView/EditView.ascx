<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaView" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaView.EditView" %>

<link href="media/modules/MetaView.css" rel="stylesheet" type="text/css" />

<label class="span-2">ViewType: </label>

<mux:SelectList
    runat="server"
    CssClass="span-4"
    Enabled="false"
    OnSelectedIndexChanged="lst_SelectedIndexChanged"
    id="lst">
    <mux:ListItem Text="List view ..." Value="ListView" />
    <mux:ListItem Text="Single view ..." Value="SingleView" />
</mux:SelectList>

<label class="span-2 pushLeft-1">MetaType: </label>

<mux:InPlaceTextAreaEdit
    runat="server"
    CssClass="mux-in-place-edit span-4 type-editor"
    OnTextChanged="type_TextChanged"
    id="type" />

<div class="span-3">
    <mux:CheckBox
        runat="server"
        OnCheckedChanged="hasSearch_CheckedChanged"
        Enabled="false"
        id="hasSearch" />
    <mux:Label
        runat="server"
        id="lblS"
        Tag="label"
        Text="Has Search ..." />
</div>

<mux:InPlaceTextAreaEdit
    runat="server"
    CssClass="mux-in-place-edit span-4 type-editor down-1 clear-left push-2"
    style="display:none;"
    ToolTip="Caption of Form"
    OnTextChanged="caption_TextChanged"
    id="caption" />

<mux:Panel
    runat="server"
    CssClass="span-21 last clear-left down-1"
    id="properties" />

<mux:Button
    runat="server"
    id="create"
    CssClass="span-6 down-1 clear-left"
    OnClick="create_Click"
    Text="New Property ..." />

<mux:Button
    runat="server"
    id="view"
    CssClass="span-6 down-1"
    OnClick="view_Click"
    Text="View ..." />


