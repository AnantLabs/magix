<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.TipOfToday" %>

<mux:Panel
    runat="server"
    id="pnl"
    CssClass="navigation">

    <mux:Button
        runat="server"
        id="next"
        OnClick="next_Click"
        ToolTip="Next Carpe Diem ..."
        CssClass="navButton"
        Text="&gt;" />

    <mux:Button
        runat="server"
        id="previous"
        OnClick="previous_Click"
        ToolTip="Previous Carpe Diem ..."
        CssClass="navButton"
        Text="&lt;" />

    <mux:Label
        runat="server"
        Tag="div"
        CssClass="tip-text"
        id="lbl" />

</mux:Panel>
