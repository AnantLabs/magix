<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="HyperLinkSample" %>

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
                <div class="span-7">
                    <p>
                        <mux:HyperLink 
                            runat="server" 
                            ID="hpl" 
                            Text="Link to google.com" 
                            URL="http://google.com" /> since we're so cool...
                    </p>
                </div>
                <div class="span-10 last">
                    <p>
                        since we're so cool...
                    </p>
                    <p>
                        <mux:Button 
                            runat="server" 
                            ID="btn" 
                            OnClick="btn_Click"
                            Text="Change to Yahho" />
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
