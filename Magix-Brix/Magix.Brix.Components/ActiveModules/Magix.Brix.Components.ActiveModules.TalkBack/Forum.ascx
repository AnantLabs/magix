<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.TalkBack" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.TalkBack.Forum" %>

<link href="media/modules/talkback.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="talkback"
    id="wrp">
    <asp:Repeater
        runat="server"
        id="rep">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <h3 class="header"><%#Eval("Header") %></h3>
            <p class="content">
                <%#Eval("Content")%>
            </p>
            <p class="user">
                <%#Eval("User")%>
            </p>
            <p class="date">
                <%#Eval("Date")%>
            </p>
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>
