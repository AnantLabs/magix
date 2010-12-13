<%@ Assembly 
    Name="WineTasting.Modules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ToolTip" %>

<mux:Window 
    runat="server" 
    Closable="true"
    style="position:absolute;top:25px;right:25px;z-index:1000;color:Black;width:350px;" 
    CssClass="mux-shaded mux-rounded wine-admin-message-box" 
    id="wnd">
    <Content>
        <mux:Label
            runat="server"
            id="lbl" />
        <div class="center-aligned-div" style="width:150px;margin-bottom:-59px;">
            <mux:Button
                runat="server"
                id="previous"
                OnClick="previous_Click"
                Text="&lt;&lt;" />
            <mux:Button
                runat="server"
                id="next"
                OnClick="next_Click"
                Text="&gt;&gt;" />
        </div>
        <mux:Button
            runat="server"
            id="ok"
            CssClass="cancel"
            OnClick="ok_Click"
            Text="OK" />
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>
