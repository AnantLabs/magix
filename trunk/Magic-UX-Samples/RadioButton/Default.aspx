<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="RadioButtonSample" %>

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
                    Text="Select one thing"
                    ID="lbl" />
                <br />
                <mux:Label 
                    runat="server" 
                    ID="Label1" 
                    Tag="label"
                    For="rdo1"
                    Text="Milk" />
                <mux:RadioButton 
                    runat="server" 
                    GroupName="rdos"
                    id="rdo1"
                    Info="Milk" 
                    OnCheckedChanged="chk_CheckedChanged" />
                <br />
                <mux:Label 
                    runat="server" 
                    ID="Label2" 
                    Tag="label"
                    For="rdo2"
                    Text="Honey" />
                <mux:RadioButton 
                    runat="server" 
                    GroupName="rdos"
                    id="rdo2" 
                    Info="Honey" 
                    OnCheckedChanged="chk_CheckedChanged" />
            </div>
        </form>
    </body>
</html>
