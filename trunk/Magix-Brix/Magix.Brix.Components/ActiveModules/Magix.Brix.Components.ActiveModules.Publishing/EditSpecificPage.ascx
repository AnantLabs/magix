<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.EditSpecificPage" %>

<mux:InPlaceTextAreaEdit
    runat="server"
    CssClass="span-3 mux-in-place-edit"
    ToolTip="Name"
    OnTextChanged="header_TextChanged"
    id="header" />

<mux:InPlaceTextAreaEdit
    runat="server"
    CssClass="span-6 mux-in-place-edit"
    ToolTip="URL"
    OnTextChanged="url_TextChanged"
    id="url" />

<mux:SelectList
    runat="server"
    CssClass="span-3"
    ToolTip="Template ..."
    OnSelectedIndexChanged="sel_SelectedIndexChanged"
    id="sel">
    <mux:ListItem Text="Choose template ..." Value="-1" />
</mux:SelectList>

<mux:Button
    runat="server"
    id="createChild"
    Text="Create Child ..."
    ToolTip="Creates a new child of the currently active Page ... "
    OnClick="createChild_Click"
    CssClass="span-6" />

<mux:Button
    runat="server"
    id="delete"
    Text="Delete!"
    ToolTip="Deletes the currently active Page ... "
    OnClick="delete_Click"
    CssClass="span-6 last" />

<mux:Panel
    runat="server"
    CssClass="span-24 last down-2"
    id="parts" />






