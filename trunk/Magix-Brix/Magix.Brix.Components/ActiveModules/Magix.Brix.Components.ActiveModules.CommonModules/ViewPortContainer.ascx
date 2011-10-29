<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ViewPortContainer" %>

<mux:DynamicPanel 
    runat="server" 
    OnReload="dynamic_LoadControls"
    id="dyn" />
