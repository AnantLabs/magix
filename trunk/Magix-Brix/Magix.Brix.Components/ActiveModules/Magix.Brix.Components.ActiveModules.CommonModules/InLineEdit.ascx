<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.InLineEdit" %>

<mux:InPlaceEdit
    runat="server"
    OnTextChanged="edit_TextChanged"
    id="edit" />
