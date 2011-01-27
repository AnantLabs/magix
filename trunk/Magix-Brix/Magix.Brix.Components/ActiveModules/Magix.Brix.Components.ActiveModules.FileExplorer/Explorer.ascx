<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.FileExplorer" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.FileExplorer.Explorer" %>

<link href="media/modules/FileExplorer.css" rel="stylesheet" type="text/css" />

<div class="span-12">
    <mux:Panel
        runat="server"
        CssClass="span-12 fileExplorer"
        id="pnl" />
</div>

<mux:Panel
    runat="server"
    id="prop"
    CssClass="span-8 last fileExplorer-properties">
    <mux:Label
        runat="server"
        id="header"
        CssClass="header"
        Tag="h3" />
    <p>
        <mux:InPlaceEdit
            runat="server"
            OnTextChanged="name_TextChanged"
            ToolTip="Rename file ..."
            id="name" />
    </p>
    <mux:Label 
        runat="server"
        id="extension"
        CssClass="extension small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="size"
        CssClass="size small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="fullUrl"
        CssClass="fullUrl small"
        Tag="p" />
    <mux:Label 
        runat="server"
        id="imageSize"
        Tag="p"
        CssClass="imageSize small" />
    <div class="span-4 height-8">
        <mux:Image
            runat="server"
            OnClick="preview_Click"
            ToolTip="Click to see image in full size ..."
            id="preview" />
    </div>
    <div class="span-4 last">
        <mux:Label
            runat="server"
            id="imageWarning"
            Tag="p"
            CssClass="imageWarning small" />
    </div>
</mux:Panel>

<div class="span-20 last fileEx">
    <mux:Button
        runat="server"
        id="previous"
        CssClass="span-2 previous"
        OnClick="previous_Click"
        Text="&lt;" />
    <mux:Button
        runat="server"
        id="next"
        CssClass="span-2 next"
        OnClick="next_Click"
        Text="&gt;" />
    <mux:Button
        runat="server"
        id="delete"
        CssClass="span-4 delete"
        OnClick="delete_Click"
        Text="Delete" />
    <div class="fileUploader span-4">
        <asp:FileUpload
            runat="server"
            OnChange="toggleButtons();"
            OnMouseOut="toggleButtons();"
            id="file" />
        <mux:TextBox
            runat="server"
            id="fileReal"
            placeholder="Upload file..."
            CssClass="browse" />
        <asp:Button
            runat="server"
            id="submitFile"
            CssClass="submitButton"
            Enabled="false"
            Text="Upload ..." 
            OnClick="submitFile_Click"/>
        <script type="text/ecmascript">
(function() {
toggleButtons = function() {
  var file = MUX.$('<%=file.ClientID%>');
  var fileReal = MUX.$('<%=fileReal.ClientID%>');
  var vl = file.value;
  if(vl.indexOf('\\') != -1) {
    vl = vl.split('\\');
    vl = vl[vl.length - 1];
  }
  if(vl.indexOf('/') != -1) {
    vl = vl.split('/');
    vl = vl[vl.length - 1];
  }
  fileReal.value = vl;
  var sub = MUX.$('<%=submitFile.ClientID%>');
  if(file.value) {
    sub.disabled = '';
  } else {
    sub.disabled = 'disabled';
  }
}
})();
        </script>
    </div>
    <mux:Button
        runat="server"
        id="select"
        CssClass="span-4 select last"
        OnClick="select_Click"
        Text="Select" />
</div>





