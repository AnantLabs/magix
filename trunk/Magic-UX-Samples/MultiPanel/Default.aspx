<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="MultiPanelSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="mux-multi-panel-wrapper" style="width:500px;display:table;margin-left:auto;margin-right:auto;">
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
                    style="width:500px;overflow:hidden;"
                    ID="mp">
                    <Content>
                        <mux:MultiPanelView 
                            runat="server"
                            style="width:500px;" 
                            ID="mpv1">
                            <div style="padding:15px;">
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
                            style="width:500px;" 
                            ID="mpv2">
                            <div style="padding:15px;">
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
                            style="width:500px;" 
                            ID="mpv3">
                            <div style="padding:15px;">
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
        </form>
    </body>
</html>
