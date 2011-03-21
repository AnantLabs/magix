<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ToolTip" %>

<link href="media/modules/ToolTip.css" rel="stylesheet" type="text/css" />

<img 
    src="media/images/marvin-happy.png" 
    class="image" />
<div class="navigation">
    <mux:Button
        runat="server"
        id="previous"
        OnClick="previous_Click"
        CssClass="span-3 navButton"
        Text="&lt;" />
    <mux:Button
        runat="server"
        id="next"
        style="margin-right:345px;"
        OnClick="next_Click"
        CssClass="span-3 navButton"
        Text="&gt;" />
    <mux:Label
        runat="server"
        Tag="div"
        id="lbl" />
</div>
