<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewClassDetails" %>

<mux:Label 
    runat="server"
    id="count" />
<br />
<mux:Button
    runat="server"
    id="select"
    Text="View all instances..."
    OnClick="select_Click" />

