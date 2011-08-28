<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.PublisherImage" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.PublisherImage.Galleries" %>

<link href="media/modules/ImageGallery.css" rel="stylesheet" type="text/css" />

<asp:Repeater
    runat="server"
    id="rep">
    <ItemTemplate>
        <mux:Panel
            runat="server"
            OnClick="OpenGallery"
            Info='<%#Eval("[ID].Value") %>'
            CssClass="mux-gallery">
            <img
                src='<%#Eval("[UserAvatarURL].Value") %>' 
                class="mux-user-gallery-avatar"
                alt="Avatar for user" />
            <p class="mux-gallery-name"><%#Eval("[Name].Value") %></p>
            <p class="mux-gallery-user"><%#Eval("[User].Value") %></p>
            <p class="mux-gallery-count"><%#Eval("[Count].Value") %> images...</p>
            <p class="mux-gallery-date"><%#Eval("[Created].Value") %></p>
        </mux:Panel>
    </ItemTemplate>
</asp:Repeater>
