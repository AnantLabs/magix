<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Publishing" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Publishing.CreateLink" %>


<mux:Panel
    runat="server"
    id="pnl"
    DefaultWidget="ok"
    CssClass="span-16">
    <div class="span-5" style="margin-bottom:18px;">
        <label class="span-5">Page</label>
        <mux:SelectList
            runat="server"
            CssClass="clear-both span-5"
            OnSelectedIndexChanged="lst_SelectedIndexChanged"
            id="lst">
            <mux:ListItem Text="Select Page ..." Value="-1" />
        </mux:SelectList>
    </div>
    <div class="span-5" style="margin-bottom:18px;">
        <label class="span-5">URL</label>
        <mux:TextBox
            runat="server"
            CssClass="clear-both span-5"
            PlaceHolder="URL ..."
            id="txt" />
    </div>
    <mux:Button
        runat="server"
        id="ok"
        style="margin-bottom:18px;"
        OnClick="ok_Click"
        AccessKey="o"
        CssClass="span-5 push-11"
        Text="OK" />
</mux:Panel>
