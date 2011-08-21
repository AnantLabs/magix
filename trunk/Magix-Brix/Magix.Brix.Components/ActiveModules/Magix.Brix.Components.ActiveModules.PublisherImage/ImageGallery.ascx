<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.PublisherImage" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.PublisherImage.ImageGallery" %>

<link href="media/modules/ImageGallery.css" rel="stylesheet" type="text/css" />

<asp:Repeater
    runat="server"
    id="rep">
    <ItemTemplate>
        <img src='<%#Eval("Value") %>' alt='Image Gallery Image ... ' />
    </ItemTemplate>
</asp:Repeater>