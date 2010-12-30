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
                    <mux:Panel
                        runat="server"
                        Info='<%#Eval("[Value].Value") + "|" + Eval("[TypeName].Value") + "|" + Eval("[PropertyName].Value") + "|" + Eval("[FullTypeName].Value") %>'
                        CssClass="templateField" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded"
    style="left:150px;top:100px;position:fixed;z-index:1000;width:750px;height:500px;"
    Caption="Details..."
    Visible="false"
    OnClosed="wnd_Closed"
    id="wnd">
    <Content>
        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            style="overflow:auto;width:700px;height:450px;margin-left:auto;margin-right:auto;"
            OnReload="child_LoadControls"
            id="child" />
    </Content>
    <Control>
        <mux:AspectModal
            runat="server"
            id="modal" />
    </Control>
</mux:Window>



