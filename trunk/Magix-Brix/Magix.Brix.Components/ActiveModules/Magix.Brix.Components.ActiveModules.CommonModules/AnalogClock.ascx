<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.AnalogClock" %>

<mux:Panel 
    runat="server" 
    id="pnl">
    <canvas
        id="myDrawing"
        runat="server"
        width="370"
        height="300">
    </canvas>
</mux:Panel>
