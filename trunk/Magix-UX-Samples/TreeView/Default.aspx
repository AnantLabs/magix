<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TreeViewSample" %>

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
                <mux:Window 
                    runat="server" 
                    Caption="Window control"
                    CssClass="mux-shaded mux-rounded mux-window span-10 prepend-top" 
                    ID="wnd">
                    <Content>
                        <mux:Label 
                            runat="server" 
                            ID="lbl" 
                            Tag="p"
                            Text="Watch me..." />
                        <mux:TreeView 
                            runat="server" 
                            OnSelectedItemChanged="tree_SelectedItemChanged"
                            ID="tree">
                            <mux:TreeItem 
                                runat="server" 
                                ID="item1" 
                                Text="File">
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item2" 
                                    Text="Open" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item3" 
                                    Text="Save" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item4" 
                                    Text="Save as...">
                                    <mux:TreeItem 
                                        runat="server" 
                                        ID="item6" 
                                        Text="PDF" />
                                    <mux:TreeItem 
                                        runat="server" 
                                        ID="item7" 
                                        Text="ODF" />
                                    <mux:TreeItem 
                                        runat="server" 
                                        ID="item8" 
                                        Text="HTML">
                                        <mux:TreeItem 
                                            runat="server" 
                                            ID="item9" 
                                            Text="XHTML" />
                                        <mux:TreeItem 
                                            runat="server" 
                                            ID="item10" 
                                            Text="HTML5" />
                                    </mux:TreeItem>
                                </mux:TreeItem>
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item5" 
                                    Text="Print" />
                            </mux:TreeItem>
                            <mux:TreeItem 
                                runat="server" 
                                ID="item11" 
                                Text="Edit">
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item13" 
                                    Text="Copy" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item14" 
                                    Text="Cut" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item15" 
                                    Text="Paste" />
                            </mux:TreeItem>
                            <mux:TreeItem 
                                runat="server" 
                                ID="item12" 
                                Text="Windows">
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item16" 
                                    Text="Window 1" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item17" 
                                    Text="Window 2" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item18" 
                                    Text="Window 3" />
                                <mux:TreeItem 
                                    runat="server" 
                                    ID="item19" 
                                    Text="Window 4" />
                            </mux:TreeItem>
                        </mux:TreeView>
                    </Content>
                </mux:Window>
                <div class="span-14 last prepend-top">
                    <p>
                        Hello there...
                    </p>
                    <p>
                        Ganger gikk
                    </p>
                    <p>
                        Hello there...
                    </p>
                    <p>
                        Ganger gikk
                    </p>
                    <p>
                        Hello there...
                    </p>
                    <p>
                        Ganger gikk
                    </p>
                    <p>
                        Hello there...
                    </p>
                    <p>
                        Ganger gikk
                    </p>
                    <p>
                        Hello there...
                    </p>
                    <p>
                        Ganger gikk
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
