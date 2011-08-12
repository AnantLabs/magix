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
    <div class="dox-methods">
        <asp:Repeater
            runat="server"
            id="rep">
            <ItemTemplate>
                <div class="one-dox-method">
                    <span class="dox-access">
                        <%#Eval("[Access].Value") %>
                    </span>
                    <span class="dox-returns">
                        <%#Eval("[Returns].Value") %>
                    </span>
                    <span class="dox-name"><%#Eval("[Name].Value") %></span>
                    <span class="dox-pars"><%#GetParams(Eval("[Pars]")) %></span>
                    <div class="push-4 span-14 last dox-description">
                        <%#Eval("[DescriptionHTML].Value") %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
