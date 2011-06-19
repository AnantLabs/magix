/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components;
using System.Globalization;
using Magix.UX.Effects;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.Calendars
{
    [ActiveModule]
    public class BigCalendar : UserControl, IModule
    {
        protected Panel pnl;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;

                    // Figuring out our start/end dates here ...
                    DateTime start = DataSource["Month"].Get<DateTime>();
                    start = new DateTime(start.Year, start.Month, 1);
                    DateTime end = start.AddMonths(1);

                    // Finding first Sunday before our start ...
                    while (start.DayOfWeek != DayOfWeek.Sunday)
                        start = start.AddDays(-1);

                    // Putting start/end dates into our DataSource
                    DataSource["Start"].Value = start;
                    DataSource["End"].Value = end;

                    // Fetching items ...
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        DataSource["GetItemsEvent"].Get<string>(),
                        DataSource);
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateCalendar();
        }

        private void CreateCalendar()
        {
            DateTime idx = DataSource["Start"].Get<DateTime>();
            while (idx < DataSource["End"].Get<DateTime>())
            {
                for (int idxNo = 0; idxNo < 7; idxNo++)
                {
                    Panel cell = new Panel();
                    cell.CssClass = "mux-big-calendar-item mux-rounded mux-shaded";
                    cell.Info = idx.Date.ToString("yyyy.MM.dd");
                    cell.Click += cell_Click;
                    if (idxNo == 0)
                        cell.CssClass += " cal-clear-both";
                    if (idx.Date == DateTime.Now.Date)
                    {
                        cell.CssClass += " cal-today";
                        cell.ToolTip += " Today!";
                    }
                    if (idx.Date.Month != DateTime.Now.Date.Month)
                    {
                        cell.CssClass += " cal-off-month";
                        cell.ToolTip += idx.Date.ToString("MMMM", CultureInfo.InvariantCulture);
                    }

                    Label lbl = new Label();
                    lbl.CssClass = "cal-date";
                    lbl.Tag = "div";
                    lbl.Text = idx.Day.ToString();
                    cell.Controls.Add(lbl);

                    lbl = new Label();
                    lbl.CssClass = "cal-month";
                    lbl.Tag = "div";
                    lbl.Text = idx.ToString("MMM", CultureInfo.InvariantCulture);
                    cell.Controls.Add(lbl);

                    foreach (Node idxObj in 
                        DataSource["Objects"][idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)])
                    {
                        Label item = new Label();
                        item.Tag = "div";
                        if(idxObj.Contains("CssClass"))
                            item.CssClass = "cal-activity " + idxObj["CssClass"].Get<string>();
                        else
                            item.CssClass = "cal-activity";
                        item.Text = idxObj["Header"].Get<string>();
                        item.ToolTip = idxObj["Body"].Get<string>();
                        cell.Controls.Add(item);
                    }
                    pnl.Controls.Add(cell);
                    idx = idx.AddDays(1);
                }
            }
        }

        private string PreviousSelected
        {
            get { return ViewState["PreviousSelected"] as string; }
            set { ViewState["PreviousSelected"] = value; }
        }

        private void cell_Click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;

            // Checking to see if this is 'un-select' ...
            if (p.CssClass.Contains(" cal-selected"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected", "");
                PreviousSelected = null;
            }
            else
            {
                DateTime date = DateTime.ParseExact(p.Info, "yyyy.MM.dd", CultureInfo.InvariantCulture);

                p.CssClass += " cal-selected";

                if (!string.IsNullOrEmpty(PreviousSelected))
                {
                    Panel old = Selector.FindControlClientID<Panel>(pnl, PreviousSelected);
                    old.CssClass = old.CssClass.Replace(" cal-selected", "");
                }

                PreviousSelected = p.ClientID;

                Node node = new Node();
                node["Date"].Value = date;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    DataSource["DateClickedEvent"].Get<string>(),
                    node);
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}



