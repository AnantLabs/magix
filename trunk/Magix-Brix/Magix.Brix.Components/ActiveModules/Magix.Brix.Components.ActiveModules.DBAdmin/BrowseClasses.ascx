<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses" %>

<mux:TreeView
    runat="server"
    id="tree"
    OnSelectedItemChanged="tree_SelectedItemChanged">
</mux:TreeView>


