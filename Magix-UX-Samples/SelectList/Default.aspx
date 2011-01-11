<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="SelectListSample" %>

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
                <div class="span-10 last">
                    <p>
                        <mux:Label 
                            runat="server" 
                            ID="lbl"
                            Text="Select an item" />
                    </p>
                    <p>
                        Select Country: <mux:SelectList 
                            runat="server"
                            OnSelectedIndexChanged="lst_SelectedIndexChanged" 
                            ID="lst">
                            <mux:ListItem Text="USA" Value="us" />
                            <mux:ListItem Text="Canada" Value="ca" />
                            <mux:ListItem Text="Britain" Value="br" />
                        </mux:SelectList>
                    </p>
                    <p>
                        <mux:Button 
                            runat="server" 
                            ID="btn" 
                            OnClick="btn_Click"
                            Text="Select Britain" />
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
