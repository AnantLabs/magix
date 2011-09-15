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
        ToolTip="Deselect Widget, if any are selected ..."
        CssClass="yellow-background mux-meta-form-wysiwyg-wrapper span-24 prepend-top last"
        id="ctrls" />

    <mux:Window
        runat="server"
        id="tools"
        Draggable="true"
        OnDragged="tools_Dragged"
        OnClick="tools_Click"
        CssClass="mux-window mux-rounded mux-shaded mux-meta-forms-toolbox span-8"
        Closable="false"
        ToolTip="The Widgets you have to your disposal within this installation of Magix. Click any particular widget you wish to append into the control collection, on either the form, or the currently selected composite widget. If you have a mouse somewhere in your proximity, you can 'drag and drop' this window around by its Header if it obscures something you need to reach ..."
        Caption="Widgets & Controls ...">
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
        OnDragged="props_Dragged"
        OnClick="props_Click"
        CssClass="mux-window mux-rounded mux-shaded mux-meta-forms-props span-8 mux-hide-events mux-hide-props"
        ToolTip="Properties and Actions for your selected Widget. If you have a mouse somewhere in your proximity, you can 'drag and drop' this window around by its Header if it obscures something you need to reach ..."
        Closable="false"
        Caption="Properties & Actions ...">
        <Content>
            <mux:SelectList
                runat="server"
                id="selWidg"
                OnSelectedIndexChanged="selWidg_SelectedIndexChanged"
                style="margin-bottom:18px;"
                CssClass="span-6 last">
                <mux:ListItem Text="Meta Form" Value="" />
            </mux:SelectList>
            <div 
                title="These actions will be raised as your Form is being initially loaded. Meaning, only when a Meta Form is being initially loaded, these Actions will run"
                class="span-6 last mux-init-actions-row">
                <div class="span-3">
                    Init Actions:
                </div>
                <mux:LinkButton
                    runat="server"
                    id="formInitActions"
                    OnClick="formInitActions_Click"
                    Text="Actions ..." />
            </div>
            <mux:Label
                runat="server"
                id="type"
                style="cursor:pointer;clear:both;margin-bottom:0;"
                CssClass="mux-section-header"
                ToolTip="Click me for a more thorough Description of the Widget"
                Tag="h5" />
            <mux:Label
                runat="server"
                id="desc"
                CssClass="span-6 last mux-wysiwyg-description"
                style="display:none;margin-bottom:0;"
                Tag="p" />
            <mux:Label
                runat="server"
                id="propHeader"
                CssClass="span-6 last mux-section-header"
                Visible="false"
                style="margin-bottom:0;cursor:pointer;margin-top:18px;"
                ToolTip="Click me to toggle visibility of Properties for selected Widget"
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
                        <div 
                            title='<%#Eval("[Description].Value") %>'
                            class="span-6 last mux-property-row mux-property-type-row">
                            <div class="span-3 mux-prop-label">
                                <%#Eval("Name") %>
                            </div>
                            <div class="span-3 last">
                                <mux:TextAreaEdit
                                    runat="server"
                                    OnTextChanged="PropertyValueChanged"
                                    TextLength="30"
                                    Info='<%#Eval("Name") %>'
                                    CssClass="span-4 last mux-in-place-edit left-float prop-editor"
                                    Visible='<%#Eval("Value").Equals("System.String") %>'
                                    Text='<%#GetPropertyValue(Eval("Name")) %>' />
                                <mux:InPlaceEdit
                                    runat="server"
                                    OnTextChanged="PropertyValueIntChanged"
                                    TextLength="5"
                                    Info='<%#Eval("Name") %>'
                                    CssClass="span-2 last mux-in-place-edit left-float prop-editor"
                                    Visible='<%#Eval("Value").Equals("System.Int32") %>'
                                    Text='<%#GetPropertyValue(Eval("Name")) %>' />
                                <mux:CheckBox
                                    runat="server"
                                    OnCheckedChanged="PropertyValueBoolChanged"
                                    Info='<%#Eval("Name") %>'
                                    CssClass="last left-float prop-editor"
                                    Checked='<%#GetPropertyValueBool(Eval("Name")) %>'
                                    Visible='<%#Eval("Value").Equals("System.Boolean") %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
            <mux:Label
                runat="server"
                id="eventHeader"
                CssClass="span-6 last mux-section-header"
                style="margin-bottom:0;cursor:pointer;"
                Visible="false"
                ToolTip="Click me to toggle visibility of Actions for selected Widget"
                Text="Actions"
                Tag="h5" />
            <mux:Panel
                runat="server"
                CssClass="span-6 last"
                id="eventWrp">
                <asp:Repeater
                    runat="server"
                    id="eventRep">
                    <ItemTemplate>
                        <div 
                            title='<%#Eval("[Description].Value") %>'
                            class="span-6 last mux-property-row mux-event-type-row">
                            <div class="span-3 mux-prop-label">
                                <%#Eval("Name") %>
                            </div>
                            <div class="span-3 last">
                                <mux:LinkButton
                                    runat="server"
                                    Info='<%#Eval("Name") %>'
                                    OnClick="ActionsClicked"
                                    CssClass='<%#"span-2 " + GetCssClass(Eval("[Description]")) %>'
                                    Text="Actions ..." />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
            <mux:Panel
                runat="server"
                id="shortCutWrp">
                <asp:Repeater
                    runat="server"
                    id="shortCutRep">
                    <ItemTemplate>
                        <mux:LinkButton
                            runat="server"
                            CssClass='<%# Eval("[CssClass].Value") + " mux-shortcut-button" %>'
                            ToolTip='<%#Eval("[ToolTip].Value") %>'
                            Info='<%#Eval("[Event].Value") %>'
                            OnClick="ShortCutButtonClicked"
                            Text='<%#Eval("[Text].Value") %>' />
                    </ItemTemplate>
                </asp:Repeater>
            </mux:Panel>
        </Content>
    </mux:Window>
</div>