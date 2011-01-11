<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="SlidingMenu" %>

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
                <div class="span-10 prepend-top">
                    <mux:Label 
                        runat="server" 
                        ID="lbl" 
                        Tag="p"
                        style="display:block;"
                        Text="Watch me..." />
                    <mux:SlidingMenu 
                        runat="server" 
                        OnLeafMenuItemClicked="sliding_LeafMenuItemClicked"
                        style="height:190px;"
                        ID="sliding">
                        <mux:SlidingMenuLevel 
                            runat="server" 
                            ID="lev1">
                            <mux:SlidingMenuItem 
                                runat="server" 
                                Text="File"
                                ID="item1">
                                <mux:SlidingMenuLevel 
                                    runat="server" 
                                    ID="lev4">
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Open"
                                        ID="item4" />
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Save"
                                        ID="item5" />
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Save as..."
                                        ID="item6">
                                        <mux:SlidingMenuLevel 
                                            runat="server" 
                                            ID="lvlx">
                                            <mux:SlidingMenuItem 
                                                runat="server" 
                                                Text="PDF"
                                                ID="lvlx1" />
                                            <mux:SlidingMenuItem 
                                                runat="server" 
                                                Text="ODF"
                                                ID="lvlx2" />
                                            <mux:SlidingMenuItem 
                                                runat="server" 
                                                Text="Word"
                                                ID="lvlx3" />
                                            <mux:SlidingMenuItem 
                                                runat="server" 
                                                Text="HTML"
                                                ID="lvlx4" />
                                        </mux:SlidingMenuLevel>
                                    </mux:SlidingMenuItem>
                                </mux:SlidingMenuLevel>
                            </mux:SlidingMenuItem>
                            <mux:SlidingMenuItem 
                                runat="server" 
                                Text="Edit"
                                ID="item2">
                                <mux:SlidingMenuLevel 
                                    runat="server" 
                                    ID="lev5">
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Copy"
                                        ID="item7" />
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Paste"
                                        ID="item8" />
                                </mux:SlidingMenuLevel>
                            </mux:SlidingMenuItem>
                            <mux:SlidingMenuItem 
                                runat="server" 
                                Text="Windows"
                                ID="item3">
                                <mux:SlidingMenuLevel 
                                    runat="server" 
                                    ID="lev6">
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Window 1"
                                        ID="item9">
                                    </mux:SlidingMenuItem>
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Window 2"
                                        ID="item10">
                                    </mux:SlidingMenuItem>
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Window 3"
                                        ID="item11">
                                    </mux:SlidingMenuItem>
                                    <mux:SlidingMenuItem 
                                        runat="server" 
                                        Text="Window 4"
                                        ID="item12">
                                    </mux:SlidingMenuItem>
                                </mux:SlidingMenuLevel>
                            </mux:SlidingMenuItem>
                        </mux:SlidingMenuLevel>
                    </mux:SlidingMenu>
                </div>
                <div class="span-14 prepend-top last">
                    <h3>Mumbo</h3>
                    <h3>Jumbo</h3>
                    <h3>File</h3>
                    <h3>Edit</h3>
                    <p>
                        Hello, this is a test...
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
