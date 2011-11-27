<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.PublisherUploader" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.PublisherUploader.Uploader" %>

<mux:Uploader
    runat="server"
    OnUploaded="uploader_Uploaded"
    id="uploader">
    <p>Please wait ...</p>
    <mux:Image
        runat="server"
        id="ajaxWait2"
        ImageUrl="media/images/ajax.gif" 
        AlternateText="Marvin's brain ..." />
</mux:Uploader>
