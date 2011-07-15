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

