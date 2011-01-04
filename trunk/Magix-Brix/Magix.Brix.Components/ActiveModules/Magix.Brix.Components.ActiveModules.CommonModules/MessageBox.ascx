<%@ Assembly 
    Name="WineTasting.Modules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.MessageBox" %>

<mux:Label
    runat="server"
    id="lbl" />
<mux:Button
    runat="server"
    id="ok"
    CssClass="ok"
    OnClick="ok_Click"
    Text="OK" />
<mux:Button
    runat="server"
    id="cancel"
    CssClass="cancel"
    OnClick="cancel_Click"
    Text="Cancel" />
