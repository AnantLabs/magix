<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.TalkBack" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.TalkBack.Forum" %>

<link href="media/modules/talkback.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    CssClass="talkback span-18 last prepend-top"
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
                <label class="counter">
                    <%#Eval("[Children].Count")%>
                </label>
                <mux:Panel
                    runat="server"
                    style='<%#GetVisiblePanel(Eval("[ID].Value")) %>'
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
                                        Tag="h4"
                                        Visible='<%#!string.IsNullOrEmpty((string)Eval("[Header].Value")) %>'
                                        CssClass="headerInner" />
                                    <label class="userInner">
                                        <%#Eval("[User].Value")%>
                                    </label>
                                    <label class="dateInner">
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
                    <div class="span-16 controls">
                        <mux:TextBox
                            runat="server"
                            style="display:none;"
                            CssClass="span-16 last"
                            PlaceHolder='<%#"Re: " + Eval("[Header].Value") %>' />
                        <mux:TextArea
                            runat="server"
                            style="display:block;"
                            CssClass="span-16 half last"
                            PlaceHolder="Description ..." />
                        <mux:Button
                            runat="server"
                            style="display:block;"
                            CssClass="span-3 push-13 last"
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
        CssClass="span-18"
        PlaceHolder="Short, but descriptive header of new post ..."
        id="header" />
    <mux:TextArea
        runat="server"
        CssClass="clear span-18 height-7"
        PlaceHolder="Detailed description of new post ..."
        id="body" />
    <mux:Button
        runat="server"
        id="submit"
        CssClass="clear span-3 push-15"
        OnClick="submit_Click"
        Text="Submit" />
</div>










