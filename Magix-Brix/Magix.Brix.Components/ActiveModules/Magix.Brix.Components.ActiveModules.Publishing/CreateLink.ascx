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
    <mux:Button
        runat="server"
        CssClass="clear-both span-5"
        AccessKey="b"
        OnClick="obj_Click"
        Text="Files ..."
        id="obj" />
    <label class="span-5">Page</label>
    <label class="span-5">URL</label>
    <mux:SelectList
        runat="server"
        CssClass="span-5"
        AccessKey="p"
        OnSelectedIndexChanged="lst_SelectedIndexChanged"
        id="lst">
        <mux:ListItem Text="Select Page ..." Value="-1" />
    </mux:SelectList>
    <mux:TextBox
        runat="server"
        CssClass="span-6 last"
        PlaceHolder="URL ..."
        id="txt" />
    <mux:Button
        runat="server"
        id="ok"
        style="margin-bottom:18px;"
        OnClick="ok_Click"
        AccessKey="o"
        CssClass="clear-both span-5 push-11 down-1"
        Text="OK" />
</mux:Panel>
