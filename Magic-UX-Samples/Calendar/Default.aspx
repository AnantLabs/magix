<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="CalendarSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div style="padding:15px;">
                <mux:Label 
                    runat="server" 
                    ID="lbl" 
                    Text="Watch me..." />
                <mux:Calendar 
                    runat="server" 
                    CssClass="mux-shaded mux-rounded mux-calendar" 
                    OnDateChanged="cal_DateChanged"
                    ID="cal" />
            </div>
        </form>
    </body>
</html>
