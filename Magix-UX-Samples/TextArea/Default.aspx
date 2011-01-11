<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TextAreaSample" %>

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
            <div class="container" style="height:500px;">
                <div class="span-8 prepend-top">
                    <mux:Label 
                        runat="server" 
                        ID="lbl"
                        Tag="p"
                        Text="Type something" />
                    <p>
                        <mux:TextArea 
                            runat="server" 
                            CssClass="span-8"
                            ID="txt" />
                    </p>
                    <p>
                        <mux:Button 
                            runat="server" 
                            ID="btn" 
                            OnClick="btn_Click"
                            Text="Save..." />
                    </p>
                </div>
                <div class="span-16 last prepend-top">
                    <p>
                        Hello there stranger...
                    </p>
                    <p>
                        Hello there stranger...
                    </p>
                    <p>
                        Hello there stranger...
                    </p>
                    <p>
                        Hello there stranger...
                    </p>
                    <p>
                        Hello there stranger...
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
