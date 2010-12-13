<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="BehaviorUpdaterSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
        <style type="text/css">
.updater1
{
    position:fixed;
    top:0;
    left:0;
    width:100%;
    height:100%;
    z-index:100;
    padding:15px;
    font-size:24px;
    text-align:center;
	background-image: -webkit-gradient(linear, 0% 0%, 0% 100%, from(rgba(180, 180, 180, 0.7)), to(rgba(0, 0, 0, 0.7)));
	background-image: -moz-linear-gradient(rgba(180, 180, 180, 0.7) 0%, rgba(0, 0, 0, 0.7) 100%);
}

.updater2
{
    position:fixed;
    top:0;
    left:0;
    width:100%;
    height:100%;
    z-index:100;
    padding:15px;
    font-size:24px;
    text-align:center;
	background-image: -webkit-gradient(linear, 0% 0%, 0% 100%, from(rgba(255, 180, 180, 0.7)), to(rgba(0, 0, 0, 0.7)));
	background-image: -moz-linear-gradient(rgba(255, 180, 180, 0.7) 0%, rgba(0, 0, 0, 0.7) 100%);
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <div id="updater1" style="display:none;" class="updater1">
                Please wait...
            </div>
            <div id="updater2" style="display:none;" class="updater2">
                Please wait 2.0... ;)
            </div>
            <div style="padding:15px;">
                <mux:Button 
                    runat="server" 
                    id="btn" 
                    OnClick="btn_Click"
                    style="margin:15px;"
                    Text="Click me, will spend 2 seconds on the server...">
                    <mux:AspectAjaxWait 
                        id="updater"
                        Element="updater1"
                        Delay="500" 
                        Opacity="1"
                        runat="server" />
                </mux:Button>
                <br />
                <mux:Button 
                    runat="server" 
                    ID="change" 
                    Text="Change updater" 
                    OnClick="change_Click" />
                <br />
                <mux:Label 
                    runat="server" 
                    ID="lbl" 
                    Text="Black, .7 opacity, .5 second delay" />
            </div>
        </form>
    </body>
</html>
