<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ToolTip" %>

<link href="media/modules/ToolTip.css" rel="stylesheet" type="text/css" />

<mux:Window 
    runat="server" 
    Closable="true"
    style="position:absolute;z-index:830;" 
    CssClass="mux-shaded mux-rounded mux-window" 
    id="wnd">
    <Content>
        <mux:Label
            runat="server"
            Tag="div"
            id="lbl" />
        <mux:Button
            runat="server"
            id="previous"
            OnClick="previous_Click"
            CssClass="span-3 navButton last"
            Text="&lt;&lt;" />
        <mux:Button
            runat="server"
            id="next"
            OnClick="next_Click"
            CssClass="span-3 navButton"
            Text="&gt;&gt;" />
        <mux:Button
            runat="server"
            id="ok"
            OnClick="ok_Click"
            CssClass="span-3 navButton"
            Text="OK" />
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>
