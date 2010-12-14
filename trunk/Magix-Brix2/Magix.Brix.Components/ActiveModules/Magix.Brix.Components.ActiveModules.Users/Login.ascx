<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Users" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Users.Login" %>

<mux:Panel 
    runat="server" 
    DefaultWidget="submit"
    id="wrp">
    <table>
        <tr>
            <td>
                Username
            </td>
            <td>
                <mux:TextBox 
                    runat="server" 
                    AutoCapitalize="false"
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
                    id="password" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:right;">
                <asp:Button 
                    runat="server" 
                    id="submit"
                    OnClick="submit_Click"
                    Text="Submit" />
            </td>
        </tr>
    </table>
    <mux:Label 
        runat="server" 
        id="err" 
        style="color:Red;" />
</mux:Panel>










