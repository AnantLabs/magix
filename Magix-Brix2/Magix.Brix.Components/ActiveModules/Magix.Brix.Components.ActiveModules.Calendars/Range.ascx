<%@ Assembly 
    Name="WineTasting.Modules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Calendars.Range" %>

<mux:Window 
    runat="server" 
    Caption="Period"
    Closable="false"
    style="position:absolute;top:25px;right:25px;z-index:500;color:Black;" 
    CssClass="mux-shaded mux-rounded transparent-window" 
    id="wnd">
    <Content>
        <table>
            <tr>
                <th style="font-size:22px;">
                    Start
                </th>
                <th style="font-size:22px;">
                    End
                </th>
            </tr>
            <tr>
                <td style="vertical-align:top;">
                    <mux:Calendar 
                        runat="server" 
                        CssClass="mux-shaded mux-rounded mux-calendar" 
                        OnDateSelected="DateSelected"
                        id="calStart" />
                </td>
                <td style="vertical-align:top;">
                    <mux:Calendar 
                        runat="server" 
                        CssClass="mux-shaded mux-rounded mux-calendar" 
                        OnDateSelected="DateSelected"
                        id="calEnd" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align:right;">
                    <mux:Button
                        runat="server"
                        id="changeDate"
                        Text="Update" 
                        OnClick="changeDate_Click" />
                </td>
            </tr>
        </table>
    </Content>
</mux:Window>
