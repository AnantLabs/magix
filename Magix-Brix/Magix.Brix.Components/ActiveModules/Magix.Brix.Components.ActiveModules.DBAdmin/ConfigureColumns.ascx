<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureColumns" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    ID="pnl"
    OnEscKey="EscKey">
    <asp:Repeater
        runat="server"
        id="rep">
        <HeaderTemplate>
            <table class="viewObjects singleInstance noMargin">
        </HeaderTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td class="wide-5">
                    <mux:CheckBox
                        runat="server"
                        OnCheckedChanged="CheckedChange"
                        CssClass="checkboxWithText"
                        Info='<%#Eval("Name") %>'
                        Checked='<%#Eval("[Visible].Value") %>' />
                    <mux:Label 
                        runat="server"
                        Info='<%#Eval("Name") %>'
                        CssClass="checkboxText"
                        OnClick="CheckedChangeFromLabel"
                        Text='<%#Eval("Name") %>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>





