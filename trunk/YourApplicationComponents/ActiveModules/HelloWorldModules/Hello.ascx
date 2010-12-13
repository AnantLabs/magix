<%@ Assembly 
    Name="HelloWorldModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="HelloWorldModules.Hello" %>

<mux:Window 
    runat="server" 
    Caption="Hello stranger..."
    style="position:absolute;top:25px;left:25px;z-index:500;color:Black;width:250px;" 
    CssClass="mux-shaded mux-rounded" 
    id="wnd">
    <Content>
        Hello there ...!
        <br />
        <mux:Label 
            runat="server"
            id="lbl" />
    </Content>
</mux:Window>
