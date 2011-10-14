<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.EditSpecificPage" %>

<link href="media/modules/Publisher-Editing.css" rel="stylesheet" type="text/css" />


<mux:Window
    runat="server"
    Draggable="false"
    CssClass="mux-shaded mux-rounded mux-window" 
    Caption="Properties & Actions"
    style="overflow-y:auto;overflow-x:hidden;"
    Closable="false">
    <Content>
        <label class="span-5">Name</label>
        <label class="span-4">URL</label>
        <mux:InPlaceEdit
            runat="server"
            CssClass="clear-both span-5 mux-in-place-edit"
            ToolTip="Name"
            style="position:relative;height:18px;"
            OnTextChanged="header_TextChanged"
            id="header" />

        <mux:InPlaceEdit
            runat="server"
            CssClass="span-4 mux-in-place-edit"
            ToolTip="URL"
            style="position:relative;height:18px;"
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
            CssClass="span-5 push-1 down--1" />

        <mux:Button
            runat="server"
            id="delete"
            Text="Delete!"
            ToolTip="Deletes the currently active Page ... "
            OnClick="delete_Click"
            CssClass="span-4 last down--1" />

        <hr class="span-22 last" style="margin-top:18px;margin-bottom:18px;" />
        <h4 class="span-22 last">Override Access Rights</h4>
        <mux:Panel
            runat="server"
            CssClass="span-22 last bottom-1"
            id="roles" />
    </Content>
</mux:Window>

<mux:Panel
    runat="server"
    CssClass="span-24 last down-1 mux-wysiwyg-surface"
    id="parts" />
