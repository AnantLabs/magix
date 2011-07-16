<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.Tree" %>

<mux:TreeView
    runat="server"
    OnSelectedItemChanged="tree_SelectedItemChanged"
    id="tree" />


