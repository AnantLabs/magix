<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Editor" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Editor.RichEdit" %>

<link 
    rel="stylesheet" 
    type="text/css" 
    href="media/modules/editor/assets/skins/sam/menu.css" />
<link 
    rel="stylesheet" 
    type="text/css" 
    href="media/modules/editor/assets/skins/sam/button.css" />
<link 
    rel="stylesheet" 
    type="text/css" 
    href="media/modules/editor/assets/skins/sam/container.css" />
<link 
    rel="stylesheet" 
    type="text/css" 
    href="media/modules/editor/assets/skins/sam/editor.css" />
<link 
    href="media/modules/RichEdit.css" 
    rel="stylesheet" 
    type="text/css" />

<script type="text/ecmascript">
(function() {
    var Dom = YAHOO.util.Dom,
        Event = YAHOO.util.Event;
    
    var myConfig = {
        dompath: true,
        focusAtStart: true
    };

    var myEditor = new YAHOO.widget.Editor('<%=txt.ClientID %>', myConfig);
    myEditor._defaultToolbar.buttonType = 'basic';
    myEditor.render();

})();

</script>

<div class="yui-skin-sam span-16 height-12">
    <mux:TextArea
        runat="server"
        id="txt" />
</div>
<div class="span-16">
    <mux:Button
        runat="server"
        id="save"
        Text="Save"
        OnClick="save_Click" />
</div>

