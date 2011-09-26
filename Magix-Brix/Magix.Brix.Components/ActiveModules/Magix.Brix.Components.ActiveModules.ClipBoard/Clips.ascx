<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.ClipBoard" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.ClipBoard.Clips" %>

<link href="media/modules/clipboard.css" rel="stylesheet" type="text/css" />

<div class="mux-clipboard-wrapper">
    <asp:Repeater
        runat="server"
        id="rep">
        <ItemTemplate>
            <mux:Panel
                runat="server"
                CssClass="span-3 height-4 clipboard-item">
                <div 
                    class="span-3 last">
                    <label 
                        class="span-3 last"
                        title='<%#Eval("[ToolTip]") %>'>
                        <%#Eval("[Name].Value") %>
                    </label>
                    <mux:LinkButton
                        runat="server"
                        ToolTip="Click me to Paste the Node into whatever specific context you've got selected currently on your page ..."
                        CssClass="span-3 last"
                        OnClick="PasteClipboardItem"
                        Info='<%#Eval("[ID].Value") %>'
                        style="margin-top:9px;margin-bottom:9px;"
                        Text="Paste" />
                    <mux:LinkButton
                        runat="server"
                        ToolTip="Click me to delete the Clipboard Item from your Clipboard ..."
                        OnClick="DeleteClipboardItem"
                        Info='<%#Eval("[ID].Value") %>'
                        CssClass="span-3 last"
                        Text="Delete" />
                </div>
            </mux:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>