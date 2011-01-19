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
    OnEscKey="wnd_EscKey"
    CssClass="mux-shaded mux-rounded mux-window tooltip" 
    id="wnd">
    <Content>
        <img 
            src="media/images/marvin-happy.png" 
            class="image" />
        <mux:Label
            runat="server"
            Tag="div"
            id="lbl" />
        <div class="navigation">
            <mux:Button
                runat="server"
                id="next"
                OnClick="next_Click"
                CssClass="span-3 navButton"
                Text="&gt;" />
            <mux:Button
                runat="server"
                id="previous"
                OnClick="previous_Click"
                CssClass="span-3 navButton"
                Text="&lt;" />
            <mux:Button
                runat="server"
                id="ok"
                OnClick="ok_Click"
                CssClass="span-3 navButton"
                Text="OK" />
        </div>
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>
