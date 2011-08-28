<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.BrowseClasses" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:TreeView
    runat="server"
    id="tree"
    OnSelectedItemChanged="tree_SelectedItemChanged">
</mux:TreeView>
