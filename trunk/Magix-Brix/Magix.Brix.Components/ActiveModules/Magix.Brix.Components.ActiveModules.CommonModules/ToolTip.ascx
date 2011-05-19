<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ToolTip" %>

<link href="media/modules/ToolTip.css" rel="stylesheet" type="text/css" />

<div class="navigation">
    <mux:Button
        runat="server"
        id="next"
        style="float:right;"
        OnClick="next_Click"
        CssClass="span-3 navButton"
        Text="&gt;" />
    <mux:Button
        runat="server"
        id="previous"
        style="float:right;margin-left:10px;"
        OnClick="previous_Click"
        CssClass="span-3 navButton"
        Text="&lt;" />
    <mux:Label
        runat="server"
        Tag="div"
        id="lbl" />
</div>
