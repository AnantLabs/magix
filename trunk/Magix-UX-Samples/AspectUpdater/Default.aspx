<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="BehaviorUpdaterSample" %>

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
        <style type="text/css">
.updater1
{
	font-size:3em;
	line-height:1;
	margin-bottom:0.5em;
	padding-top:.5em;
    position:fixed;
    top:0;
    left:0;
    width:100%;
    height:100%;
    z-index:100;
    text-align:center;
    background-image: -webkit-gradient(linear, 0% 0%, 0% 100%, from(rgba(180, 180, 180, 0.7)), to(rgba(0, 0, 0, 0.7)));
	background-image: -moz-linear-gradient(rgba(180, 180, 180, 0.7) 0%, rgba(0, 0, 0, 0.7) 100%);
}

.updater2
{
	font-size:3em;
	line-height:1;
	margin-bottom:0.5em;
	padding-top:.5em;
    position:fixed;
    top:0;
    left:0;
    width:100%;
    height:100%;
    z-index:100;
    text-align:center;
	background-image: -webkit-gradient(linear, 0% 0%, 0% 100%, from(rgba(255, 180, 180, 0.7)), to(rgba(0, 0, 0, 0.7)));
	background-image: -moz-linear-gradient(rgba(255, 180, 180, 0.7) 0%, rgba(0, 0, 0, 0.7) 100%);
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="container prepend-top">
                <div id="updater1" style="display:none;" class="updater1">
                    Please wait...
                </div>
                <div id="updater2" style="display:none;" class="updater2">
                    Please wait 2.0... ;)
                </div>
                <div class="span-14 append-1">
                    <mux:Button 
                        runat="server" 
                        id="btn" 
                        OnClick="btn_Click"
                        Text="Click me, will spend 2 seconds on the server...">
                        <mux:AspectAjaxWait 
                            id="updater"
                            Element="updater1"
                            Delay="500" 
                            Opacity="1"
                            runat="server" />
                    </mux:Button>
                </div>
                <div class="span-8 last">
                    <mux:Button 
                        runat="server" 
                        ID="change" 
                        Text="Change updater" 
                        OnClick="change_Click" />
                </div>
                <div class="prepend-4 span-8 prepend-top">
                    <mux:Label 
                        runat="server" 
                        ID="lbl" 
                        Text="Black, .7 opacity, .5 second delay" />
                </div>
            </div>
        </form>
    </body>
</html>
