<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.DBAdmin" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.DBAdmin.Main" %>

<mux:TreeView
    runat="server"
    id="tree"
    OnSelectedItemChanged="tree_SelectedItemChanged">
</mux:TreeView>

<mux:Window 
    runat="server" 
    CssClass="mux-shaded mux-rounded"
    style="left:150px;top:100px;position:fixed;z-index:1000;width:450px;height:250px;"
    Caption="Details..."
    Visible="false"
    OnClosed="wnd_Closed"
    id="wnd">
    <Content>
        <mux:DynamicPanel 
            runat="server" 
            CssClass="dynamic"
            style="overflow:auto;width:400px;height:200px;margin-left:auto;margin-right:auto;"
            OnReload="dynamic_LoadControls"
            id="popup" />
    </Content>
</mux:Window>


