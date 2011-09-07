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
        ToolTip="Yes, you can move me by dragging me by my header with your mouse [only computers, sorry :(]"
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
        ToolTip="Yes, you can move me by dragging me by my header with your mouse [only computers, sorry :(]"
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
                CssClass="span-6 last mux-wysiwyg-description"
                style="display:none;margin-bottom:0;"
                Tag="p" />
            <mux:Label
                runat="server"
                id="propHeader"
                CssClass="span-6 last"
                Visible="false"
                style="margin-bottom:0;"
                Text="Properties"
                Tag="h5" />
            <mux:Panel
                runat="server"
                CssClass="span-6 last spcBottom-1"
                id="propWrp">
                <asp:Repeater
                    runat="server"
                    id="propRep">
                    <ItemTemplate>
                        <div class="span-2 mux-prop-label">
                            <%#Eval("Name") %>
                        </div>
                        <div class="span-4 last">
                            <mux:TextAreaEdit
                                runat="server"
                                OnTextChanged="PropertyValueChanged"
                                Info='<%#Eval("Name") %>'
                                ToolTip='<%#Eval("[Description].Value") %>'
                                CssClass="span-4 last mux-in-place-edit left-float prop-editor"
                                Visible='<%#Eval("Value").Equals("System.String") %>'
                                Text='<%#GetPropertyValue(Eval("Name")) %>' />
                            <mux:CheckBox
                                runat="server"
                                OnCheckedChanged="PropertyValueBoolChanged"
                                Info='<%#Eval("Name") %>'
                                ToolTip='<%#Eval("[Description].Value") %>'
                                CssClass="last left-float prop-editor"
                                Checked='<%#GetPropertyValueBool(Eval("Name")) %>'
                                Visible='<%#Eval("Value").Equals("System.Boolean") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
            <mux:Label
                runat="server"
                id="eventHeader"
                CssClass="span-6 last"
                style="margin-bottom:0;"
                Visible="false"
                Text="Events"
                Tag="h5" />
            <mux:Panel
                runat="server"
                CssClass="span-6 last"
                id="eventWrp">
                <asp:Repeater
                    runat="server"
                    id="eventRep">
                    <ItemTemplate>
                        <div class="span-2 mux-prop-label">
                            <%#Eval("Name") %>
                        </div>
                        <div class="span-4 last">
                            <mux:LinkButton
                                runat="server"
                                CssClass="span-4 last"
                                Info='<%#Eval("Name") %>'
                                ToolTip='<%#Eval("[Description].Value") %>'
                                Text="Actions..." />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
        </Content>
    </mux:Window>
</div>