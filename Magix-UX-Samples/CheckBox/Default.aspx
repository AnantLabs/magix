<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="CheckBoxSample" %>

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
            <div class="container prepend-top">
                <div class="span-24 last" style="height:500px;">
                    <mux:CheckBox 
                        runat="server" 
                        id="chk" 
                        OnCheckedChanged="chk_CheckedChanged" />
                    <mux:Label 
                        runat="server" 
                        ID="lbl" 
                        Tag="label"
                        For="chk"
                        Text="Click me" />
                </div>
            </div>
        </form>
    </body>
</html>
