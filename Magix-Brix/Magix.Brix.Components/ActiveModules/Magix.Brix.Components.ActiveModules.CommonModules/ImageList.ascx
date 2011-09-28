<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.ImageList" %>

<mux:Panel
    runat="server"
    id="wrp">
    <asp:Repeater
        runat="server"
        id="rep">
        <ItemTemplate>
            <mux:Image
                runat="server"
                CssClass='<%#Eval("[CSS].Value") %>' 
                ImageURL='<%#Eval("[Image].Value") %>' 
                Info='<%#Eval("[Event].Value") %>' 
                AccessKey='<%#Eval("[Shortcut].Value") %>' 
                OnClick="ImageClicked"
                AlternateText='<%#Eval("[Text].Value") %>'
                ToolTip='<%#GetTooltip(Eval("[Text].Value")) %>' />
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>