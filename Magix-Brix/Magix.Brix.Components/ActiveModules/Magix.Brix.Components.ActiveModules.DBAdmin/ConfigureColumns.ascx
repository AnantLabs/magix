<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureColumns" %>

<asp:Repeater
    runat="server"
    id="rep">
    <HeaderTemplate>
        <table>
            <tr>
                <td>Name</td>
                <td>Visible</td>
            </tr>
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <%#Eval("Name") %>
            </td>
            <td>
                <mux:CheckBox
                    runat="server"
                    OnCheckedChanged="CheckedChange"
                    Info='<%#Eval("Name") %>'
                    Checked='<%#Eval("[Visible].Value") %>' />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>