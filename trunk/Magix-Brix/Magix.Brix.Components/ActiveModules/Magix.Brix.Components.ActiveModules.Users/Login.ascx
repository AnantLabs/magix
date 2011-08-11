<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Users" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Users.Login" %>

<link href="media/modules/Login.css" rel="stylesheet" type="text/css" />

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded mux-window"
    Caption="Please login..."
    Closable="false"
    id="wrp">
    <Content>
        <table class="loginTable">
            <tr>
                <td class="wide-2">
                    Username
                </td>
                <td class="span-4 txt">
                    <mux:TextBox 
                        runat="server" 
                        AutoCapitalize="false"
                        PlaceHolder="Username"
                        CssClass="span-4 txt"
                        id="username" />
                </td>
            </tr>
            <tr>
                <td>
                    Password
                </td>
                <td>
                    <mux:TextBox 
                        runat="server" 
                        TextMode="Password" 
                        PlaceHolder="Password"
                        CssClass="span-4 txt"
                        id="password" />
                </td>
            </tr>
            <tr>
                <td>
                    Or ...
                </td>
                <td>
                    <mux:TextBox 
                        runat="server" 
                        AutoCapitalize="false"
                        PlaceHolder="OpenID"
                        CssClass="span-4 open-id"
                        id="openID" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button 
                        runat="server" 
                        id="submit"
                        CssClass="span-3 loginOkButton"
                        OnClick="submit_Click"
                        Text="Submit" />
                </td>
            </tr>
        </table>
    </Content>
</mux:Window>










