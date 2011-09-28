<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Documentation" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Documentation.ShowClassDetails" %>


<div class="span-18 last">
    <mux:Label
        runat="server"
        id="header"
        CssClass="span-18 last"
        style="display:block;"
        Tag="h3" />
    <mux:Label
        runat="server"
        id="content"
        Tag="div" />
    <div class="mux-dox-methods">
        <asp:Repeater
            runat="server"
            id="rep">
            <ItemTemplate>
                <div class="mux-one-dox-method">
                    <span class="mux-dox-access">
                        <%#Eval("[Access].Value") %>
                    </span>
                    <span class="mux-dox-returns">
                        <%#Eval("[Returns].Value") %>
                    </span>
                    <span class="mux-dox-name"><%#Eval("[Name].Value") %></span>
                    <span class="mux-dox-pars"><%#GetParams(Eval("[Pars]")) %></span>
                    <div class="push-4 span-14 last mux-dox-description">
                        <%#Eval("[DescriptionHTML].Value") %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
