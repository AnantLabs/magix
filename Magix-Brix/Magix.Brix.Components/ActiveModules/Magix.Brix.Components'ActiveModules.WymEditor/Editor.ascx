<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.WymEditor" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.WymEditor.Editor" %>

<link href="media/modules/WymEditor.css" rel="stylesheet" type="text/css" />

<script type="text/ecmascript">
(function() {
  if (WYMeditor.INSTANCES.length > 0) {
    // Already initialized before, second or later loading ...
    // Because of fuck ups in WYMEditor, we'll need to run through this garbage to be safe ...
    WYMeditor.INSTANCES = [];
  }

  jQuery('.wymeditor').wymeditor({
    logoHtml: '',
    basePath: 'media/Js/wymeditor/',
    skinPath: 'media/Js/wymeditor/skins/default/',
    wymPath: 'media/Js/wymeditor/',
    iframeBasePath: 'media/Js/wymeditor/iframe/default/',
    stylesheet: 'media/modules/wym-styles.css',
    toolsItems: [
      {'name': 'Bold', 'title': 'Strong', 'css': 'wym_tools_strong'}, 
      {'name': 'Italic', 'title': 'Emphasis', 'css': 'wym_tools_emphasis'},
      {'name': 'Superscript', 'title': 'Superscript', 'css': 'wym_tools_superscript'},
      {'name': 'Subscript', 'title': 'Subscript', 'css': 'wym_tools_subscript'},
      {'name': 'InsertOrderedList', 'title': 'Ordered_List', 'css': 'wym_tools_ordered_list'},
      {'name': 'InsertUnorderedList', 'title': 'Unordered_List', 'css': 'wym_tools_unordered_list'},
      {'name': 'Undo', 'title': 'Undo', 'css': 'wym_tools_undo'},
      {'name': 'Redo', 'title': 'Redo', 'css': 'wym_tools_redo'},
      {'name': 'MuxAddLink', 'title': 'Link', 'css': 'wym_tools_link'},
      {'name': 'Unlink', 'title': 'Unlink', 'css': 'wym_tools_unlink'},
      {'name': 'MuxAddImage', 'title': 'Image', 'css': 'wym_tools_image'},
      {'name': 'ToggleHtml', 'title': 'HTML', 'css': 'wym_tools_html'}
    ]
  });
  if(!WYMeditor.editor.prototype._exec2) {
    WYMeditor.editor.prototype._exec2 = WYMeditor.editor.prototype.exec;
  }
  WYMeditor.editor.prototype.exec = function(cmd) {
    switch(cmd) {
      case 'MuxAddLink':
        MUX.CustomerWYM.createLink.apply(this);
        break;
      case 'MuxAddImage':
        MUX.CustomerWYM.createImage.apply(this);
        break;
      break;
      default:
        this._exec2(cmd);
      break;
    }
  }
  MUX.CustomerWYM = {};
  MUX.CustomerWYM.save = function() {
    WYMeditor.INSTANCES[0].update();
    var val = WYMeditor.INSTANCES[0].xhtml();
    MUX.$('<%=txt.ClientID%>').innerHTML = val;
    MUX.Control.callServerMethod('<%=this.ClientID%>.Save', {
      onSuccess: function(retVal) {
      }
    }, []);
  }
  MUX.CustomerWYM.createLink = function() {
    var container = this.container();
    if(container || this._selected_image) {
      var val = WYMeditor.INSTANCES[0].xhtml();
      MUX.$('<%=txt.ClientID%>').innerHTML = val;
      MUX.Control.callServerMethod('<%=this.ClientID%>.CreateLink', {
        onSuccess: function(retVal) {
        }
      }, []);
    }
  }
  MUX.CustomerWYM.createImage = function() {
    var container = this.container();
    if(container || this._selected_image) {
      var val = WYMeditor.INSTANCES[0].xhtml();
      MUX.$('<%=txt.ClientID%>').innerHTML = val;
      MUX.Control.callServerMethod('<%=this.ClientID%>.CreateImage', {
        onSuccess: function(retVal) {
        }
      }, []);
    }
  }
  MUX.CustomerWYM.afterLink = function(sUrl) {
    var selected = WYMeditor.INSTANCES[0].selected();
    var link;
    var sStamp = WYMeditor.INSTANCES[0].uniqueStamp();
    if (selected[0] && selected[0].tagName.toLowerCase() == WYMeditor.A) {
      link = selected;
    } else {
      WYMeditor.INSTANCES[0]._exec(WYMeditor.CREATE_LINK, sStamp);
      link = jQuery("a[href=" + sStamp + "]", WYMeditor.INSTANCES[0]._doc.body);
      link.attr(WYMeditor.HREF, sUrl);
    }
    link.attr(WYMeditor.HREF, sUrl);
    WYMeditor.INSTANCES[0]._iframe.contentWindow.focus();
    WYMeditor.INSTANCES[0].update();
  }
  MUX.CustomerWYM.afterImage = function(sUrl) {
    var selected = WYMeditor.INSTANCES[0].selected();
    var link;
    var sStamp = WYMeditor.INSTANCES[0].uniqueStamp();
    if (selected[0] && selected[0].tagName.toLowerCase() == WYMeditor.IMG) {
      link = selected;
    } else {
      WYMeditor.INSTANCES[0]._exec(WYMeditor.INSERT_IMAGE, sStamp);
      link = jQuery("img[src=" + sStamp + "]", WYMeditor.INSTANCES[0]._doc.body);
      link.attr(WYMeditor.SRC, sUrl);
    }
    link.attr(WYMeditor.SRC, sUrl);
    WYMeditor.INSTANCES[0]._iframe.contentWindow.focus();
    WYMeditor.INSTANCES[0].update();
  }
})();
</script>

<mux:Panel
    runat="server"
    id="wrp"
    CssClass="">
    <mux:TextArea
        runat="server"
        CssClass="wymeditor"
        id="txt" />
    <input
        type="button"
        id="submit"
        accesskey="s"
        class="span-5 push-17 last"
        style="margin-bottom:18px;"
        onclick="MUX.CustomerWYM.save();"
        value="Save" />
</mux:Panel>
