<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="SelectListSample" %>

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
                    Text="Select an item" />
                <br />
                <mux:SelectList 
                    runat="server"
                    OnSelectedIndexChanged="lst_SelectedIndexChanged" 
                    ID="lst">
                    <mux:ListItem Text="USA" Value="us" />
                    <mux:ListItem Text="Canada" Value="ca" />
                    <mux:ListItem Text="Britain" Value="br" />
                </mux:SelectList>
                <br />
                <mux:Button 
                    runat="server" 
                    ID="btn" 
                    OnClick="btn_Click"
                    Text="Select Britain" />
            </div>
        </form>
    </body>
</html>
