<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TimerSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div style="padding:15px;">
                <mux:Button 
                    runat="server" 
                    ID="btn" 
                    Text="Start timer..." 
                    OnClick="btn_Click" />
                <mux:Window 
                    runat="server" 
                    Caption="Watch me tick...!"
                    Visible="false"
                    style="width:500px;position:absolute;top:25px;left:25px;z-index:500;" 
                    CssClass="mux-shaded mux-rounded mux-window" 
                    ID="wnd">
                    <Content>
                        Hello there, take a look at my accurate clock :)
                        <br />
                        <mux:Label 
                            runat="server" 
                            ID="lbl" 
                            style="text-size:1.2em;font-weight:bold;margin:10px;display:block;padding:15px;"
                            CssClass="mux-shaded mux-rounded"
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
            </div>
        </form>
    </body>
</html>
