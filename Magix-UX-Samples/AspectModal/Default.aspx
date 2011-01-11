<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="AspectModalSample" %>

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
            <div class="container prepend-top">
                <div class="span-14 append-1">
                    <h1>Example of AspectModal</h1>
                    <p>
                        <a href="http://google.com">I cannot be clicked, even though I am a normal link...</a>
                    </p>
                </div>
                <div class="span-9 last">
                    <mux:Button 
                        runat="server" 
                        id="btn" 
                        OnClick="btn_Click"
                        style="position:absolute;z-index:500;"
                        Text="I am a greedy button. I need the WHOLE screen :)">
                        <mux:AspectModal 
                            id="modal"
                            runat="server" />
                    </mux:Button>
                </div>
            </div>
        </form>
    </body>
</html>
