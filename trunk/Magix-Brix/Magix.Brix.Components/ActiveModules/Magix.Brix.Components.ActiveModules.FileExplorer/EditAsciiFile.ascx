<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.FileExplorer.EditAsciiFile" %>

<mux:TextArea
    runat="server"
    id="txt"
    CssClass="ascii-editor span-22" />


<mux:Button
    runat="server"
    id="save"
    Text="Save"
    OnClick="save_Click"
    CssClass="save-btn span-4 push-18 last" />
