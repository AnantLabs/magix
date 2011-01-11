<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="MenuSample" %>

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
                <div class="span-10 prepend-top">
                    <h2>File ... Edit ... Windows ...</h2>
                    <h2>Open</h2>
                    <mux:Label 
                        runat="server" 
                        ID="lbl" 
                        Text="Watch me while you choose items..." />
                </div>
                <div class="span-14 prepend-top last">
                    <mux:Menu
                        runat="server" 
                        OnLeafMenuItemClicked="MenuItemClicked"
                        ID="menu">
                        <mux:MenuItem 
                            runat="server" 
                            Text="File"
                            ID="file">
                            <mux:SubMenu 
                                runat="server" 
                                ID="fileSub">
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="open" 
                                    Text="Open" />
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="save" 
                                    URL="http://google.com"
                                    Text="Save to Google" />
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="saveas" 
                                    Text="Save as..." />
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="lastopened" 
                                    Text="Last open items">
                                    <mux:SubMenu 
                                        runat="server" 
                                        ID="loi">
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="cv"
                                            Text="Curriculum Vitae.pdf" />
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="acc"
                                            Text="Year report.odf" />
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="htm"
                                            Text="Corporate website.html">
                                            <mux:SubMenu 
                                                runat="server" 
                                                ID="subxx">
                                                <mux:MenuItem 
                                                    runat="server" 
                                                    ID="menuxx1"
                                                    Text="Website as XHTML" />
                                                <mux:MenuItem 
                                                    runat="server" 
                                                    ID="menuxxx2"
                                                    Text="Website as HTML5" />
                                            </mux:SubMenu>
                                        </mux:MenuItem>
                                    </mux:SubMenu>
                                </mux:MenuItem>
                            </mux:SubMenu>
                        </mux:MenuItem>
                        <mux:MenuItem 
                            runat="server" 
                            Text="Edit"
                            ID="edit">
                            <mux:SubMenu 
                                runat="server" 
                                ID="editSub">
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="copy"
                                    Text="Copy" /> 
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="cut"
                                    Text="Cut" /> 
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="smartPaste" 
                                    Text="Smart Paste">
                                    <mux:SubMenu 
                                        runat="server" 
                                        ID="smartPasteSub">
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="pasteHTM"
                                            Text="Paste as HTML" />
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="pasteTxt"
                                            Text="Paste as Text" />
                                        <mux:MenuItem 
                                            runat="server" 
                                            ID="pasteBitmap"
                                            Text="Paste Bitmap" />
                                    </mux:SubMenu>
                                </mux:MenuItem>
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="paste"
                                    Text="Paste" /> 
                            </mux:SubMenu>
                        </mux:MenuItem>
                        <mux:MenuItem 
                            runat="server" 
                            Text="Windows"
                            ID="windows">
                            <mux:SubMenu 
                                runat="server" 
                                ID="windowsSub">
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="wnd1"
                                    Text="Window 1" /> 
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="wnd2"
                                    Text="Window 2" /> 
                                <mux:MenuItem 
                                    runat="server" 
                                    ID="wnd3"
                                    Text="Window 3" /> 
                            </mux:SubMenu>
                        </mux:MenuItem>
                    </mux:Menu>
                </div>
            </div>
        </form>
    </body>
</html>
