<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.Tree" %>

<mux:Label
    runat="server"
    Tag="h2"
    CssClass="no-bottom-margin"
    id="header" />

<mux:TreeView
    runat="server"
    OnSelectedItemChanged="tree_SelectedItemChanged"
    id="tree" />


