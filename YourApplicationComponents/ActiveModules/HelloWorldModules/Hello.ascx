<%@ Assembly 
    Name="HelloWorldModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="HelloWorldModules.Hello" %>

<mux:Window 
    runat="server" 
    Caption="Hello stranger..."
    style="position:absolute;top:25px;left:25px;z-index:500;color:Black;width:350px;" 
    CssClass="mux-shaded mux-rounded" 
    id="wnd">
    <Content>
        <div>
            Hello there ...!
            <br />
            <mux:Label 
                runat="server"
                id="lbl" />
        </div>
        <div>
            <mux:Button
                runat="server"
                id="btn"
                OnClick="btn_Click"
                Text="Go Hitchhiking ..." />
        </div>
    </Content>
</mux:Window>
