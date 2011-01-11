<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses" %>

<mux:Window 
    runat="server"
    Caption="Browse ActiveType Classes"
    Closable="false"
    style="z-index:500;" 
    CssClass="mux-shaded mux-rounded transparent-window" 
    id="wnd">
    <Content>
        <mux:TreeView
            runat="server"
            id="tree"
            OnSelectedItemChanged="tree_SelectedItemChanged">
        </mux:TreeView>
    </Content>
</mux:Window>


