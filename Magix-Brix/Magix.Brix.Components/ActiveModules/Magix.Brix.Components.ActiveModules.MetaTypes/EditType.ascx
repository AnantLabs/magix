<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaTypes" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaTypes.EditType" %>

<link href="media/modules/MetaType-Editing.css" rel="stylesheet" type="text/css" />

<mux:InPlaceEdit
    runat="server"
    OnTextChanged="lbl_TextChanged"
    CssClass="mux-in-place-edit larger"
    id="lbl" />

<mux:Panel
    runat="server"
    CssClass="span-12 down-2"
    id="values" />

<mux:Button
    runat="server"
    id="create"
    CssClass="span-6 down-1"
    OnClick="create_Click"
    Text="New Value ..." />