<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="WindowSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <mux:Window 
                runat="server" 
                Caption="Window control"
                style="width:500px;position:absolute;top:25px;left:25px;" 
                CssClass="mux-shaded mux-rounded mux-window" 
                ID="wnd">
                <Content>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Donec fermentum dignissim cursus. Duis eu velit eu magna 
                    adipiscing posuere. Sed sit amet magna velit. Sed vel 
                    nisl eu nibh tincidunt posuere. Vivamus quis lacus suscipit 
                    velit egestas interdum. Sed scelerisque, quam ullamcorper 
                    vestibulum vestibulum, eros nisl ornare metus, non laoreet 
                    purus diam ut neque. Ut erat augue, malesuada ac cursus ac, 
                    porta a nulla. Nulla ultricies orci ut ligula facilisis id 
                    tempus turpis vestibulum. Morbi faucibus elit vitae nisi 
                    condimentum ornare. Donec aliquet convallis pretium. Donec 
                    dui enim, placerat nec ultricies eget, pellentesque ut magna. 
                    Etiam vel quam et metus varius tristique. Proin eget ipsum 
                    nec felis blandit semper eu nec felis. Morbi eget rhoncus quam. 
                    Phasellus arcu sapien, semper a consectetur nec, iaculis 
                    sit amet urna.
                </Content>
            </mux:Window>
            
            <mux:Button 
                runat="server" 
                id="btn" 
                OnClick="btn_Click"
                style="width:500px;position:absolute;top:225px;left:225px;"
                Text="Click me..." />
            
             <mux:Window 
                runat="server" 
                Caption="Window control"
                style="width:500px;position:absolute;top:225px;left:225px;z-index:500;" 
                Movable="true" 
                CssClass="mux-shaded mux-rounded mux-window"
                ID="Window2"
                visible="false">
                <Content>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                    Donec fermentum dignissim cursus. Duis eu velit eu magna 
                    adipiscing posuere. Sed sit amet magna velit. Sed vel 
                    nisl eu nibh tincidunt posuere. Vivamus quis lacus suscipit 
                    velit egestas interdum. Sed scelerisque, quam ullamcorper 
                    vestibulum vestibulum, eros nisl ornare metus, non laoreet 
                    purus diam ut neque. Ut erat augue, malesuada ac cursus ac, 
                    porta a nulla. Nulla ultricies orci ut ligula facilisis id 
                    tempus turpis vestibulum. Morbi faucibus elit vitae nisi 
                    condimentum ornare. Donec aliquet convallis pretium. Donec 
                    dui enim, placerat nec ultricies eget, pellentesque ut magna. 
                    Etiam vel quam et metus varius tristique. Proin eget ipsum 
                    nec felis blandit semper eu nec felis. Morbi eget rhoncus quam. 
                    Phasellus arcu sapien, semper a consectetur nec, iaculis 
                    sit amet urna.
                </Content>
                <Control>
                    <mux:AspectModal 
                        ID="BehaviorModal1" 
                        runat="server" />
                </Control>
            </mux:Window>
            
            
        </form>
    </body>
</html>
