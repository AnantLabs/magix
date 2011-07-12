<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.TreeViewOfPages" %>

<link href="media/modules/Publisher-Editing.css" rel="stylesheet" type="text/css" />

<mux:TreeView
    runat="server"
    OnSelectedItemChanged="tree_SelectedItemChanged"
    id="tree" />
