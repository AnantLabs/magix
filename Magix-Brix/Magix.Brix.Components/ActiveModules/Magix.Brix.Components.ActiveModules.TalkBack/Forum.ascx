<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.TalkBack" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.TalkBack.Forum" %>

<link href="media/modules/talkback.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="talkback span-18 last"
    id="wrp">
    <asp:Repeater
        runat="server"
        id="rep">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <mux:Panel
                runat="server"
                CssClass="one-item">
                <mux:Label
                    runat="server"
                    Text='<%#GetShorter(Eval("[Header].Value")) %>'
                    Tag="h3"
                    ToolTip='<%#Eval("[Header].Value") %>'
                    CssClass="header" />
                <label class="user">
                    <%#Eval("[User].Value")%>
                </label>
                <label class="date">
                    <%#((DateTime)Eval("[Date].Value")).ToString("dddd - d.MMM yyyy - HH:mm", System.Globalization.CultureInfo.InvariantCulture)%>
                </label>
                <mux:Panel
                    runat="server"
                    style="display:none;"
                    CssClass="one-item-content">
                    <div class="content">
                        <%#Eval("[Content].Value")%>
                    </div>
                    <div class="replies span-16 last">
                        <asp:Repeater
                            runat="server" 
                            DataSource='<%# Eval("[Children]") %>'>
                            <ItemTemplate>
                                <div class="reply span-16 last">
                                    <hr />
                                    <mux:Label
                                        runat="server"
                                        Text='<%#Eval("[Header].Value") %>'
                                        Tag="h3"
                                        CssClass="headerInner" />
                                    <label class="user">
                                        <%#Eval("[User].Value")%>
                                    </label>
                                    <label class="date">
                                        <%#((DateTime)Eval("[Date].Value")).ToString("dddd - d.MMM yyyy - HH:mm", System.Globalization.CultureInfo.InvariantCulture)%>
                                    </label>
                                    <div
                                        class="one-item-content">
                                        <div class="content">
                                            <%#Eval("[Content].Value")%>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="span-11 padding-1 controls">
                        <mux:TextBox
                            runat="server"
                            style="display:block;"
                            CssClass="span-10 half"
                            Text='<%#"Re: " + Eval("[Header].Value") %>'
                            PlaceHolder="Header ..." />
                        <mux:TextArea
                            runat="server"
                            style="display:block;"
                            CssClass="span-10"
                            PlaceHolder="Description ..." />
                        <mux:Button
                            runat="server"
                            style="display:block;"
                            CssClass="span-3 push-7"
                            Info='<%#Eval("[ID].Value") %>'
                            OnClick="reply_Click"
                            Text="Reply" />
                    </div>
                </mux:Panel>
            </mux:Panel>
        </ItemTemplate>
    </asp:Repeater>
</mux:Panel>

<div class="span-18 last">
    <mux:TextBox
        runat="server"
        CssClass="span-10"
        PlaceHolder="Header ..."
        id="header" />
    <mux:TextArea
        runat="server"
        CssClass="clear span-10"
        PlaceHolder="Description ..."
        id="body" />
    <mux:Button
        runat="server"
        id="submit"
        CssClass="clear span-3 push-7"
        OnClick="submit_Click"
        Text="Submit" />
</div>










