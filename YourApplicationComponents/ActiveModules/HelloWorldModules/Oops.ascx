<%@ Assembly 
    Name="HelloWorldModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="HelloWorldModules.Oops" %>

<mux:Window 
    runat="server" 
    Caption="Hello stranger..."
    style="position:absolute;top:125px;left:125px;z-index:1500;color:Black;width:250px;" 
    CssClass="mux-shaded mux-rounded" 
    id="wnd">
    <Content>
        <div>
            Oops, poets on their way - duck and cover...!!
        </div>
    </Content>
    <Control>
        <mux:AspectModal
            runat="server" />
    </Control>
</mux:Window>
