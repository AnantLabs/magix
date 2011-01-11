<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="CalendarSample" %>

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
            <div class="container prepend-top" style="height:500px;">
                <div class="span-5 append-1">
                    <h2>January 2010</h2>
                    <mux:Label 
                        runat="server" 
                        ID="lbl" 
                        Text="Watch me..." />
                    <p>
                        wdfig wefiyg wefiyg wfig fig sd,h sdfkjh sdfiug sdfiuyg 
                        wefiugw efiys d,mvsd jhsd fky dfiyg ewr7t23487t23 
                        7wt fygsd fikygsd f5
                    </p>
                </div>
                <div class="span-18 last">
                    <mux:Calendar 
                        runat="server" 
                        CssClass="mux-shaded mux-rounded mux-calendar" 
                        OnDateChanged="cal_DateChanged"
                        ID="cal" />
                </div>
            </div>
        </form>
    </body>
</html>
