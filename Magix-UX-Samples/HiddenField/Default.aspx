<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="HiddenFieldSample" %>

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
                <div class="span-24 last">
                    <mux:HiddenField 
                        runat="server" 
                        ID="hid" 
                        Value="hidden field value" />
                    <mux:Label 
                        runat="server" 
                        Tag="p"
                        ID="lbl" />
                    <p>
                        Type in here: 
                        <mux:TextBox 
                            runat="server" 
                            CssClass="text"
                            ID="txt" />
                    </p>
                    <p>
                        <mux:Button 
                            runat="server" 
                            ID="btn" 
                            OnClick="btn_Click"
                            Text="Save hidden field" />
                    </p>
                    <p>
                        <mux:Button 
                            runat="server" 
                            ID="btn2" 
                            OnClick="btn2_Click"
                            Text="Retrieve hidden field" />
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
