<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Settings" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Settings.ViewSettings" %>

<mux:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="20"
    OnCellEdited="grid_CellEdited"
    OnRowDeleted="grid_RowDeleted"
    id="grd" />
