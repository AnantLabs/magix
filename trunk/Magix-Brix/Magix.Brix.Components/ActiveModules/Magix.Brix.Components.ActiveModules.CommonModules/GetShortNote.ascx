<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.GetShortNote" %>

<mux:TextArea
    runat="server"
    id="txt" />
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
