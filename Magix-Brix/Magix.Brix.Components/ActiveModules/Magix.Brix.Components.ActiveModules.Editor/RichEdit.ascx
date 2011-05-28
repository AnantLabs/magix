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

    var imgConfig2 = {
      type: 'push', label: 'HTML', value: 'htmlicon'
    };
    myEditor.toolbar.addButtonToGroup(imgConfig2, 'insertitem');
    myEditor.toolbar.on('htmliconClick', function(ev) {

      this.saveHTML();
      var el = MUX.$('<%=rawTxt.ClientID%>');
      el.value = 
        myEditor.get('textarea').value;
      el.setStyle('display', '');
      el.focus();

      var txt = MUX.$('toggle');
      txt.setStyle('display', '');

      return false;
    }, myEditor, true);
  });

  myEditor.render();

  window.close1 = function()
  {
    var el = MUX.$('<%=rawTxt.ClientID%>');
    var txt = MUX.$('toggle');
    myEditor.setEditorHTML(el.value); 
    el.setStyle('display', 'none');

    txt.setStyle('display', 'none');
  }

  MUX.Form.beforeSerialization.push({
    handler:function(){
      this.saveHTML();
    },
    context:myEditor
  });
})();

</script>

<mux:Panel
    runat="server"
    id="wrp"
    CssClass="yui-skin-sam mux-rich-editor" style="width:100%;height:100%;">
    <mux:TextArea
        runat="server"
        style="width:100%;height:100%;display:none;"
        id="txt" />
    <mux:TextArea
        runat="server"
        style="display:none;"
        CssClass="rawText"
        id="rawTxt" />
    <input 
        type="button" 
        id="toggle"
        onclick="window.close1();" 
        class="span-3 last"
        style="position:absolute;right:5%;bottom:5%;display:none;z-index:998;margin-bottom:-24px;margin-right:-10px;"
        value="Close" />
</mux:Panel>
