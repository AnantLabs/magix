<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.MetaForms" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.MetaForms.EditMetaForm" %>

<link href="media/modules/edit-meta-forms.css" rel="stylesheet" type="text/css" />

<div class="wysiwyg-meta-forms">
    <mux:Panel
        runat="server"
        OnClick="ctrls_Click"
        CssClass="yellow-background mux-meta-form-wysiwyg-wrapper span-16 push-8 last"
        id="ctrls" />

    <mux:Window
        runat="server"
        id="tools"
        Draggable="true"
        CssClass="mux-window mux-rounded mux-shaded mux-meta-forms-toolbox span-8"
        Closable="false"
        Caption="Widgets ...">
        <Content>
            <asp:Repeater
                runat="server"
                id="tlsRep">
                <ItemTemplate>
                    <mux:LinkButton
                        runat="server"
                        CssClass="mux-toolbox-item"
                        ToolTip='<%#Eval("[ToolTip].Value") %>'
                        OnClick="AddControlToPage"
                        Info='<%#Eval("[TypeName].Value") %>'
                        Text='<%#Eval("[Name].Value") %>' />
                </ItemTemplate>
            </asp:Repeater>
        </Content>
    </mux:Window>

    <mux:Window
        runat="server"
        id="props"
        Draggable="true"
        CssClass="mux-window mux-rounded mux-shaded mux-meta-forms-props span-8"
        Closable="false"
        Caption="Properties ...">
        <Content>
            <mux:Label
                runat="server"
                id="type"
                style="cursor:pointer;"
                ToolTip="Click me for a more thorough Description of the Widget"
                Tag="h4" />
            <mux:Label
                runat="server"
                id="desc"
                CssClass="span-6 last spcBottom-2"
                style="display:none;margin-bottom:0;"
                Tag="p" />
            <mux:Label
                runat="server"
                id="propHeader"
                CssClass="span-6 last"
                Visible="false"
                style="margin-top:18px;"
                Text="Properties"
                Tag="h5" />
            <mux:Panel
                runat="server"
                CssClass="span-6 last spcBottom-4"
                id="propWrp">
                <asp:Repeater
                    runat="server"
                    id="propRep">
                    <ItemTemplate>
                        <div class="span-2">
                            <%#Eval("Name") %>
                        </div>
                        <div class="span-4 last">
                            <mux:TextAreaEdit
                                runat="server"
                                OnTextChanged="PropertyValueChanged"
                                Info='<%#Eval("Name") %>'
                                CssClass="span-4 last mux-in-place-edit left-float prop-editor"
                                Text='<%#GetPropertyValue(Eval("Name")) %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
        </Content>
    </mux:Window>
</div>