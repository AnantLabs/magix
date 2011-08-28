<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.FileExplorer" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.FileExplorer.Explorer" %>

<link href="media/modules/FileExplorer.css" rel="stylesheet" type="text/css" />

<mux:Uploader
    runat="server"
    OnUploaded="uploader_Uploaded"
    id="uploader">
    <p>Please wait while Marvin is pushing your files through ...</p>
    <mux:Image
        runat="server"
        id="ajaxWait2"
        ImageUrl="media/images/ajax.gif" 
        AlternateText="Marvin's brain ..." />
</mux:Uploader>

<div class="span-12">
    <mux:Panel
        runat="server"
        CssClass="span-12 fileExplorer"
        id="pnl" />
</div>
<mux:Panel
    runat="server"
    id="prop"
    CssClass="span-6 last fileExplorer-properties">
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
    <mux:HyperLink 
        runat="server"
        id="fullUrl"
        Target="_blank"
        CssClass="fullUrl small" />
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
</mux:Panel>
<div class="span-18 last fileEx">
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
    <input
        type="file"
        runat="server"
        multiple="multiple"
        class="file-control"
        OnChange="MUX.uploadFile();"
        id="file" />
    <input 
        type="button" 
        OnClick='MUX.$("<%=file.ClientID %>").click();'
        class="span-4"
        value="Upload ..." />

    <script type="text/ecmascript">
(function() {
MUX.uploadFile = function() {

  var fileEl = MUX.$('<%=file.ClientID %>');
  var files = MUX.$('<%=file.ClientID %>').files;

  for (var i = 0, f; f = files[i]; i++) {
    var reader = new FileReader();

    reader.onload = (function(idxF) {
      return function(e) {
        var img = e.target.result;
        MUX.Control.callServerMethod('<%=this.ClientID%>.SubmitFile', {
          onSuccess: function(retVal) {
          },
          onError: function(status, fullTrace) {
          },
        }, [encodeURIComponent(idxF.name), encodeURIComponent(img)]);
      };
    })(f);
    reader.readAsDataURL(f);
  }
}
})();
    </script>

    <mux:Button
        runat="server"
        id="select"
        style="display:block;margin-right:10px !iomportant;"
        CssClass="span-4 select"
        ToolTip="Click to select active file"
        OnClick="select_Click"
        Text="Select" />
    <mux:Button
        runat="server"
        id="newCss"
        style="display:block;"
        ToolTip="Click to create a new empty CSS file"
        CssClass="span-1 last"
        OnClick="newCss_Click"
        Text="+" />
</div>






