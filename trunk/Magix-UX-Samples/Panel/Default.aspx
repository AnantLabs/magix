<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="PanelSample" %>

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
                <div class="span-20 last prepend-top last">
                    <mux:Panel 
                        runat="server" 
                        CssClass="span-10 last"
                        style="background-color:LightBlue;padding-top:1.5em;padding-bottom:1.5em;"
                        OnMouseOver="pnl_MouseOver"
                        OnMouseOut="pnl_MouseOut"
                        ID="pnl">
                        <mux:Label 
                            runat="server" 
                            ID="lbl" 
                            CssClass="mux-insert"
                            Text="Try to hoover over the panel" />
                    </mux:Panel>
                </div>
            </div>
        </form>
    </body>
</html>
