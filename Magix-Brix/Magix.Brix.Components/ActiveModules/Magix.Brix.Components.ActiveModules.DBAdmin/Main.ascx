<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Settings" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.Main" %>

<mux:TreeView
    runat="server"
    id="tree"
    OnSelectedItemChanged="tree_SelectedItemChanged">
</mux:TreeView>



