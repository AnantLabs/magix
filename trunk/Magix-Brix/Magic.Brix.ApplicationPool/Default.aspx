<%@ Page 
    Language="C#" 
    AutoEventWireup="true" 
    ValidateRequest="false"
    Inherits="Magic.Brix.ApplicationPool.MainWebPage"
    CodeFile="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <title>Your Application Pool</title>
        <base runat="server" id="baseElement" />
        <link href="media/magic-ux-skins/default.css" rel="stylesheet" type="text/css" />

        <!-- Apple stuff ... -->
        <meta name="apple-mobile-web-app-capable" content="yes">
        <meta name="viewport" content="width=700, user-scalable=no">
        <link rel="apple-touch-icon" href="./media/images/icon.png" />
        <link rel="apple-touch-startup-image" href="./media/images/splash.png" />
    </head>
    <body>
        <form 
            id="form1" 
            runat="server" 
            enctype="multipart/form-data" />
    </body>
</html>
