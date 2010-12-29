<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ViewObject" %>


<mux:Panel
    runat="server"
    id="pnl">
    <asp:Repeater
        runat="server"
        id="rep">
        <HeaderTemplate>
            <table class="viewObject">
                <tr class="header">
                    <td>Type</td>
                    <td>Property Name</td>
                    <td>Value</td>
                </tr>
        </HeaderTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td title='<%#Eval("[FullTypeName].Value").ToString().Replace("'", "\\'") %>'><%#Eval("[TypeName].Value") %></td>
                <td><%#Eval("[PropertyName].Value")%></td>
                <td>
                    <%#Eval("[Value].Value") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>

