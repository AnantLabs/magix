<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="TreeViewSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div 
                class="mux-rounded mux-shaded"
                style="padding:15px;width:250px;display:block;margin-top:50px;margin-left:auto;margin-right:auto;">
                <mux:Label 
                    runat="server" 
                    ID="lbl" 
                    style="display:block;margin:15px;" 
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
            </div>
        </form>
    </body>
</html>
