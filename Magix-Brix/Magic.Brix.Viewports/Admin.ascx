<%@ Assembly 
    Name="Magic.Brix.Viewports" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magic.Brix.Viewports.Admin" %>

<link href="media/admin.css" rel="stylesheet" type="text/css" />

<mux:AjaxWait 
    runat="server" 
    CssClass="ajax-wait"
    MaxOpacity="0.8"
    id="waiter">
    <h2>Please wait while Marvin is thinking ...</h2>
    <img src="media/images/animated_brain.gif" alt="Marvin's brain ..." />
</mux:AjaxWait>

<div class="mainAdminWhileContent">
    <a runat="server" href="~/" class="adminHeader">&nbsp;</a>
    <div class="menu">
        <mux:SlidingMenu 
            runat="server" 
            OnLeafMenuItemClicked="sliding_LeafMenuItemClicked"
            ID="sliding">
            <mux:SlidingMenuLevel 
                runat="server" 
                ID="root">
                <mux:SlidingMenuItem 
                    runat="server" 
                    Text="Dashboard"
                    ID="LoadDashboard" />
                <mux:SlidingMenuItem 
                    runat="server" 
                    Text="Menus"
                    ID="item1">
                    <mux:SlidingMenuLevel 
                        runat="server" 
                        ID="lev4">
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="View All Menus..."
                            ID="ViewAllMenus" />
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="Create New..."
                            ID="CreateNewMenu" />
                    </mux:SlidingMenuLevel>
                </mux:SlidingMenuItem>
                <mux:SlidingMenuItem 
                    runat="server" 
                    Text="Reports/Exports"
                    ID="item2">
                    <mux:SlidingMenuLevel 
                        runat="server" 
                        ID="lev5">
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="View ratings..."
                            ID="ViewRatings" />
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="View exports..."
                            ID="ViewExports" />
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="View Club Members..."
                            ID="ViewClubMembers" />
                    </mux:SlidingMenuLevel>
                </mux:SlidingMenuItem>
                <mux:SlidingMenuItem 
                    runat="server" 
                    Text="Advanced"
                    ID="item3">
                    <mux:SlidingMenuLevel 
                        runat="server" 
                        ID="lev6">
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="Edit Email Template..."
                            ID="EditEmailTemplate" />
                        <mux:SlidingMenuItem 
                            runat="server" 
                            Text="Settings..."
                            ID="Settings" />
                    </mux:SlidingMenuLevel>
                </mux:SlidingMenuItem>
                <mux:SlidingMenuItem 
                    runat="server" 
                    Text="Log Out!"
                    ID="LogOut" />
            </mux:SlidingMenuLevel>
        </mux:SlidingMenu>
    </div>
    <div class="content">
        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            OnReload="dynamic_LoadControls"
            id="dynAdmin" />

        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            OnReload="dynamic_LoadControls"
            id="dynAdmin2" />
    </div>
</div>

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded mux-window wine-window"
    style="display:none;"
    Caption="Message from system"
    Closable="false"
    id="message">
    <Content>
        <mux:Label 
            runat="server" 
            id="msgLbl" />
    </Content>
</mux:Window>

