<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="WindowSample" %>

<!DOCTYPE html>

<html>
    <head runat="server">
        <title>Untitled Page</title>
        <link rel="stylesheet" href="../media/blueprint/screen.css" type="text/css" media="screen, projection" />
        <link rel="stylesheet" href="../media/blueprint/print.css" type="text/css" media="print" />
        <!--[if lt IE 8]>
        <link rel="stylesheet" href="../media/blueprint/ie.css" type="text/css" media="screen, projection" />
        <![endif]-->

        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="container" style="height:500px;">
                <div class="span-10 prepend-top" style="position:relative;">
                    <p>
                        Hello there ...
                    </p>
                    <mux:Window 
                        runat="server" 
                        Caption="Window control"
                        CssClass="mux-shaded mux-rounded mux-window" 
                        style="width:390px;position:absolute;"
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
                </div>
                
                <div class="span-12 last prepend-top" style="position:relative;">
                    <mux:Button 
                        runat="server" 
                        id="btn" 
                        OnClick="btn_Click"
                        Text="Click me..." />
                    <mux:Window 
                        runat="server" 
                        Caption="Window control"
                        style="width:390px;position:absolute;top:0px;left:0px;z-index:500;" 
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
                </div>
            </div>
        </form>
    </body>
</html>
