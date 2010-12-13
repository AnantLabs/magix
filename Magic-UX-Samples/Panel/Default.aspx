<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="PanelSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <mux:Panel 
                runat="server" 
                style="padding:15px;border:solid 1px Black;width:100px;height:100px;margin:50px;"
                OnMouseOver="pnl_MouseOver"
                OnMouseOut="pnl_MouseOut"
                ID="pnl">
                <mux:Label 
                    runat="server" 
                    ID="lbl" 
                    Text="Try to hoover over the panel" />
            </mux:Panel>
        </form>
    </body>
</html>
