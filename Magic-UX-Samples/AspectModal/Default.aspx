<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="AspectModalSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div style="padding:15px;">
                Here's an example of the AspectModal...
                <br/>
                <a href="http://google.com">I cannot be clicked, even though I am a normal link...</a>
                <mux:Button 
                    runat="server" 
                    id="btn" 
                    OnClick="btn_Click"
                    style="margin:15px;position:absolute;z-index:500;"
                    Text="I am a greedy button. I need the WHOLE screen :)">
                    <mux:AspectModal 
                        id="modal"
                        runat="server" />
                </mux:Button>
            </div>
        </form>
    </body>
</html>
