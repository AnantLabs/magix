<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.ConfigureFilters" %>

<link href="media/modules/DBAdmin.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    DefaultWidget="ok"
    id="pnlWrp">
    <mux:Panel
        runat="server"
        id="pnl">
    </mux:Panel>
    <div class="windowActionButton">
        <mux:Button
            runat="server"
            CssClass="cancel"
            id="cancel"
            Text="Cancel"
            OnClick="cancel_Click" />
    </div>
    <div class="windowActionButton">
        <mux:Button
            runat="server"
            id="ok"
            CssClass="ok"
            Text="OK"
            OnClick="ok_Click" />
    </div>
</mux:Panel>
