<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TimerSample" %>

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
                <div class="span-10 prepend-top">
                    <mux:Button 
                        runat="server" 
                        ID="btn" 
                        Text="Start timer..." 
                        OnClick="btn_Click" />
                </div>
                <div class="span-10 prepend-top">
                    <mux:Window 
                        runat="server" 
                        Caption="Watch me tick...!"
                        Visible="false"
                        style="position:relative;z-index:500;" 
                        CssClass="mux-shaded mux-rounded mux-window span-6" 
                        ID="wnd">
                        <Content>
                            <p>
                                Hello there, take a look at my accurate clock :)
                            </p>
                            <mux:Label 
                                runat="server" 
                                ID="lbl" 
                                Tag="p"
                                Text="Watch me change..." />
                            <mux:Timer 
                                runat="server" 
                                id="timer" 
                                OnTick="timer_Tick" />
                        </Content>
                        <Control>
                            <mux:AspectModal 
                                runat="server" 
                                ID="modal" />
                        </Control>
                    </mux:Window>
                    <div class="span-3 last">
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                        <p>
                            Hello there
                        </p>
                    </div>
                </div>
            </div>
        </form>
    </body>
</html>
