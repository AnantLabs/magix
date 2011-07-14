<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaTypes" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaTypes.Dashboard" %>

<mux:Button
    runat="server"
    id="create"
    CssClass="span-5"
    OnClick="create_Click"
    Text="Create New ..." />

