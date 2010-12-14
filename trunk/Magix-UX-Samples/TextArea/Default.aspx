<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TextAreaSample" %>

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
                    Text="Type something" />
                <br />
                <mux:TextArea 
                    runat="server" 
                    ID="txt" />
                <br />
                <mux:Button 
                    runat="server" 
                    ID="btn" 
                    OnClick="btn_Click"
                    Text="Save..." />
            </div>
        </form>
    </body>
</html>
