<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="MultiPanelSample" %>

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
            <div class="container">
                <div class="span-6 prepend-top">
                    <h2>TabView1</h2>
                </div>
                <div 
                    class="span-18 last prepend-top">
                    <div class="span-18 last mux-multi-panel-wrapper">
                        <mux:TabStrip 
                            runat="server" 
                            MultiPanelID="mp"
                            ID="mub">
                            <mux:TabButton 
                                runat="server" 
                                Text="TabView 1"
                                ID="mb1" />
                            <mux:TabButton
                                runat="server" 
                                Text="TabView 2"
                                ID="mb2" />
                            <mux:TabButton
                                runat="server" 
                                Text="TabView 3"
                                ID="mb3" />
                        </mux:TabStrip>
                        <mux:MultiPanel 
                            runat="server" 
                            AnimationMode="Slide"
                            style="width:710px;"
                            ID="mp">
                            <Content>
                                <mux:MultiPanelView 
                                    runat="server"
                                    style="width:710px;"
                                    ID="mpv1">
                                    <div class="mux-insert">
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
                                    </div>
                                </mux:MultiPanelView>
                                <mux:MultiPanelView 
                                    runat="server" 
                                    style="width:710px;"
                                    ID="mpv2">
                                    <div class="mux-insert">
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
                                    </div>
                                </mux:MultiPanelView>
                                <mux:MultiPanelView 
                                    runat="server" 
                                    style="width:710px;"
                                    ID="mpv3">
                                    <div class="mux-insert">
                                        This is an example of an English text...
                                        <br />
                                        <mux:Button 
                                            runat="server" 
                                            Text="Click me..."
                                            OnClick="btn_Click"
                                            ID="btn" />
                                    </div>
                                </mux:MultiPanelView>
                            </Content>
                        </mux:MultiPanel>
                    </div>
                    <p>
                        Hello there ...
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
