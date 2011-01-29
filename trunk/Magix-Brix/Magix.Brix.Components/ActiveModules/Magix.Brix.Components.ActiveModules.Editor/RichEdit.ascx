<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Editor" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Editor.RichEdit" %>

<link href="media/modules/editor/assets/skins/sam/menu.css" rel="stylesheet" type="text/css" />
<link href="media/modules/editor/assets/skins/sam/button.css" rel="stylesheet" type="text/css" />
<link href="media/modules/editor/assets/skins/sam/container.css" rel="stylesheet" type="text/css" />
<link href="media/modules/editor/assets/skins/sam/editor.css" rel="stylesheet" type="text/css" />
<link href="media/modules/RichEdit.css" rel="stylesheet" type="text/css" />

<script type="text/ecmascript">
(function() {
  var Dom = YAHOO.util.Dom, Event = YAHOO.util.Event;
  var myConfig = {
    dompath: true
  };

  var myEditor = new YAHOO.widget.Editor('<%=txt.ClientID %>', myConfig);

  myEditor.on('toolbarLoaded', function() { 
    var imgConfig = {
      type: 'push', label: 'Save', value: 'saveicon'
    };
    myEditor.toolbar.addButtonToGroup(imgConfig, 'insertitem');
    myEditor.toolbar.on('saveiconClick', function(ev) {
      this.saveHTML();
      MUX.Control.callServerMethod('<%=this.ClientID%>.Save', {
        onSuccess: function(retVal) {
        },
        onError: function(status, fullTrace) {
        }
      }, [encodeURIComponent(MUX.$('<%=txt.ClientID%>').innerHTML)]);
      return false;
    }, myEditor, true);
  });

  myEditor.render();

  MUX.Form.beforeSerialization.push({
    handler:function(){
      this.saveHTML();
    },
    context:myEditor
  });
})();

</script>

<div class="yui-skin-sam mux-rich-editor" style="width:100%;height:100%;">
    <mux:TextArea
        runat="server"
        style="width:100%;height:100%;display:none;"
        id="txt" />
</div>
