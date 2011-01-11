<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="ButtonSample" %>

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
                <div class="span-11 append-1">
                    <mux:Button 
                        runat="server" 
                        id="btn" 
                        OnClick="btn_Click"
                        Text="Click me..." />
                    <br />
                    <p>
                        Click the button going...
                    </p>
                </div>
                <div class="span-12 last">
                    <h2>Click him ...</h2>
                    <p>
                        Click the button going...
                    </p>
                    <p>
                        Click the button going...
                    </p>
                    <p>
                        Click the button going...
                    </p>
                    <p>
                        Click the button going...
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
