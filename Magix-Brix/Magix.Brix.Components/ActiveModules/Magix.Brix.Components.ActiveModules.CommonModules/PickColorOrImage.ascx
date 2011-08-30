<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.PickColorOrImage" %>

<mux:Panel
    runat="server"
    DefaultWidget="ok"
    id="wrp">
    <mux:ColorPicker
        runat="server"
        id="clr">
        <mux:Button
            runat="server"
            id="getImage"
            Text="Image ..."
            OnClick="getImage_Click"
            CssClass="mux-color-picker-image-picker" />
        <mux:Button
            runat="server"
            id="ok"
            Text="OK"
            OnClick="ok_Click"
            CssClass="mux-color-picker-ok" />
        <mux:Button
            runat="server"
            id="cancel"
            Text="Cancel"
            style="margin-left:10px;"
            OnClick="cancel_Click"
            CssClass="mux-color-picker-ok" />
    </mux:ColorPicker>
</mux:Panel>

