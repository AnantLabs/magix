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
    style="position:absolute;top:20px;left:25px;z-index:500;color:Black;min-width:750px;min-height:450px;" 
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


