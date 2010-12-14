<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="HiddenFieldSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div style="padding:15px;">
                <mux:HiddenField 
                    runat="server" 
                    ID="hid" 
                    Value="hidden field value" />
                <mux:Label 
                    runat="server" 
                    ID="lbl" />
                <br />
                <mux:TextBox 
                    runat="server" 
                    ID="txt" />
                <mux:Button 
                    runat="server" 
                    ID="btn" 
                    OnClick="btn_Click"
                    Text="Save hidden field" />
                <mux:Button 
                    runat="server" 
                    ID="btn2" 
                    OnClick="btn2_Click"
                    Text="Retrieve hidden field" />
            </div>
        </form>
    </body>
</html>
