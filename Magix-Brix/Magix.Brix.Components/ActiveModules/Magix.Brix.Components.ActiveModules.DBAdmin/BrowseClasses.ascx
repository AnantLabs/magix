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
    style="position:absolute;top:25px;right:25px;z-index:500;color:Black;width:350px;" 
    CssClass="mux-shaded mux-rounded transparent-window" 
    id="wnd">
    <mux:TreeView
        runat="server"
        id="tree"
        OnSelectedItemChanged="tree_SelectedItemChanged">
    </mux:TreeView>
</mux:Window>


