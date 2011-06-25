<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Calendars" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Calendars.BigCalendar" %>

<link href="media/modules/Calendar.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    DefaultWidget="search"
    id="wrp">
    <mux:Button
        runat="server"
        id="previous"
        CssClass="span-4 previous-button"
        OnClick="Previous"
        Text="<<" />
    <mux:Button
        runat="server"
        id="search"
        style="margin-left:-4000px;"
        OnClick="Filter"
        Text="&nbsp;" />
    <mux:TextBox
        runat="server"
        id="filter"
        PlaceHolder="Filter ..."
        CssClass="span-6 filter-text" />
    <mux:Button
        runat="server"
        id="next"
        CssClass="span-4 next-button"
        OnClick="Next"
        Text=">>" />
</mux:Panel>

<mux:Panel
    runat="server"
    CssClass="span-15 last"
    id="pnl" />