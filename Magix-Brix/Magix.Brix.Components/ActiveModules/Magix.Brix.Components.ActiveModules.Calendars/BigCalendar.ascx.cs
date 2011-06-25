/*
 * MagixBRIX - A Web Application Framework for ASP.NET
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
        protected TextBox filter;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;

                    // Figuring out our start/end dates here ...
                    DateTime month = DataSource["Month"].Get<DateTime>();
                    FetchCalendarItems(month);
                };
        }

        private void FetchCalendarItems(DateTime month)
        {
            DateTime start = new DateTime(month.Year, month.Month, 1);
            DateTime end = start.AddMonths(1);

            // Finding first Sunday before our start ...
            while (start.DayOfWeek != DayOfWeek.Sunday)
                start = start.AddDays(-1);

            while (end.DayOfWeek != DayOfWeek.Sunday)
                end = end.AddDays(1);

            // Putting start/end dates into our DataSource
            DataSource["Start"].Value = start;
            DataSource["End"].Value = end;

            // Fetching items ...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["GetItemsEvent"].Get<string>(),
                DataSource);
        }

        protected void Previous(object sender, EventArgs e)
        {
            DateTime month = DataSource["Month"].Get<DateTime>();
            month = month.AddMonths(-1);
            DataSource["Month"].Value = new DateTime(month.Year, month.Month, 1);
            pnl.Controls.Clear();
            DataSource["Objects"].UnTie();
            FetchCalendarItems(month);
            CreateCalendar();
            pnl.ReRender();
        }

        protected void Next(object sender, EventArgs e)
        {
            DateTime month = DataSource["Month"].Get<DateTime>();
            month = month.AddMonths(1);
            DataSource["Month"].Value = new DateTime(month.Year, month.Month, 1);
            pnl.Controls.Clear();
            DataSource["Objects"].UnTie();
            FetchCalendarItems(month);
            CreateCalendar();
            pnl.ReRender();
        }

        protected void Filter(object sender, EventArgs e)
        {
            DataSource["Filter"].Value = filter.Text;
            pnl.Controls.Clear();
            DataSource["Objects"].UnTie();
            FetchCalendarItems(DataSource["Month"].Get<DateTime>());
            CreateCalendar();
            pnl.ReRender();
            filter.Focus();
            filter.Select();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateCalendar();
        }

        [ActiveEvent(Name = "Magix.Core.UpdateCalendar")]
        protected void Magix_Core_UpdateCalendar(object sender, ActiveEventArgs e)
        {
            // Clearing previous data ...
            pnl.Controls.Clear();

            // Removing previously fetched items out of DataSource ...
            DataSource["Objects"].UnTie();

            // Fetching items ...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["GetItemsEvent"].Get<string>(),
                DataSource);

            // Re-creating Calendar controls ...
            CreateCalendar();

            pnl.ReRender();

            // Flashing our screen ... ;)
            new EffectHighlight(pnl, 250)
                .Render();
        }

        private void CreateCalendar()
        {
            DateTime idx = DataSource["Start"].Get<DateTime>();
            int idxNoTotal = 0;
            Rows = (int)(DataSource["End"].Get<DateTime>() - DataSource["Start"].Get<DateTime>())
                .TotalDays / 7;
            while (idx < DataSource["End"].Get<DateTime>())
            {
                for (int idxNo = 0; idxNo < 7; idxNo++)
                {
                    Panel cell = new Panel();
                    cell.ID = "x" + idx.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    cell.CssClass = "mux-big-calendar-item mux-rounded mux-shaded";
                    if (DataSource["Objects"][idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)]
                        .Contains("CssClass"))
                    {
                        cell.CssClass += " " + DataSource["Objects"][idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)]["CssClass"].Get<string>();
                    }
                    string position = " pos-" + idxNoTotal ++;
                    cell.CssClass += position;
                    cell.Info = idx.Date.ToString("yyyy.MM.dd");
                    if (!string.IsNullOrEmpty(PreviousSelected))
                    {
                        string[] ids = PreviousSelected.Split('_');
                        string dateStr = ids[ids.Length - 1].Substring(1);
                        if (dateStr == idx.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture))
                            SetCssClass(cell);
                    }
                    cell.Click += cell_Click;
                    if (idxNo == 0)
                        cell.CssClass += " cal-clear-both";
                    if (idx.Date == DateTime.Now.Date)
                    {
                        cell.CssClass += " cal-today";
                        cell.ToolTip += " Today!";
                    }
                    if (idx.Date.Month != DataSource["Month"].Get<DateTime>().Month)
                    {
                        cell.CssClass += " cal-off-month";
                        cell.ToolTip += idx.Date.ToString("MMMM", CultureInfo.InvariantCulture);
                    }

                    // 'View Details' part ...
                    if (DataSource.Contains("DateDetailsClickedEvent"))
                    {
                        LinkButton det = new LinkButton();
                        if (DataSource.Contains("DateDetailsClickedEventDescription"))
                        {
                            det.Text = DataSource["DateDetailsClickedEventDescription"].Get<string>();
                        }
                        else
                        {
                            det.Text = "View Details ...";
                        }
                        if (DataSource.Contains("DateDetailsClickedEventTooltip"))
                        {
                            det.ToolTip = DataSource["DateDetailsClickedEventTooltip"].Get<string>();
                        }
                        det.CssClass = "cal-view-details";
                        det.Info = idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                        det.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton b = sender as LinkButton;
                                Node node = new Node();
                                node["Date"].Value = 
                                    DateTime.ParseExact(b.Info, "yyyy.MM.dd", CultureInfo.InvariantCulture);
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    DataSource["DateDetailsClickedEvent"].Get<string>(),
                                    node);
                            };
                        cell.Controls.Add(det);
                    }

                    // Day of month part
                    Label lbl = new Label();
                    lbl.CssClass = "cal-date";
                    lbl.Tag = "div";
                    lbl.Text = idx.Day.ToString();
                    cell.Controls.Add(lbl);

                    // Month part, which by default won't show before 'blown up' ...
                    lbl = new Label();
                    lbl.CssClass = "cal-month";
                    lbl.Tag = "div";
                    lbl.Text = idx.ToString("MMM", CultureInfo.InvariantCulture);
                    cell.Controls.Add(lbl);

                    foreach (Node idxObj in 
                        DataSource["Objects"][idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)])
                    {
                        if (idxObj.Contains("ClickEvent"))
                        {
                            Node tmpNode = idxObj;
                            LinkButton item = new LinkButton();
                            if (idxObj.Contains("CssClass"))
                                item.CssClass = "cal-activity " + idxObj["CssClass"].Get<string>();
                            else
                                item.CssClass = "cal-activity";
                            item.Info = idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                            item.Click +=
                                delegate(object sender2, EventArgs e2)
                                {
                                    Node node = null;
                                    if (tmpNode.Contains("ClickedEventParams"))
                                    {
                                        node = tmpNode["ClickedEventParams"];
                                    }
                                    else
                                        node = new Node();
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        tmpNode["ClickEvent"].Get<string>(),
                                        node);
                                };
                            item.Text = idxObj["Header"].Get<string>();
                            item.ToolTip = idxObj["Body"].Get<string>();
                            cell.Controls.Add(item);
                        }
                        else if (idxObj.Contains("EditEvent"))
                        {
                            Node tmpNode = idxObj;

                            TextAreaEdit item = new TextAreaEdit();
                            if (idxObj.Contains("CssClass"))
                                item.CssClass = "cal-activity " + idxObj["CssClass"].Get<string>();
                            else
                                item.CssClass = "cal-activity";
                            item.Info = idx.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                            item.TextChanged +=
                                delegate(object sender2, EventArgs e2)
                                {
                                    Node node = null;
                                    if (tmpNode.Contains("EditEventParams"))
                                    {
                                        node = tmpNode["EditEventParams"];
                                    }
                                    else
                                        node = new Node();
                                    node["Text"].Value = (sender2 as TextAreaEdit).Text;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        tmpNode["EditEvent"].Get<string>(),
                                        node);
                                };
                            item.Text = idxObj["Header"].Get<string>();
                            item.ToolTip = idxObj["Body"].Get<string>();
                            cell.Controls.Add(item);
                        }
                        else
                        {
                            Label item = new Label();
                            item.Tag = "div";
                            if (idxObj.Contains("CssClass"))
                                item.CssClass = "cal-activity " + idxObj["CssClass"].Get<string>();
                            else
                                item.CssClass = "cal-activity";
                            item.Text = idxObj["Header"].Get<string>();
                            item.ToolTip = idxObj["Body"].Get<string>();
                            cell.Controls.Add(item);
                        }
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

        private int Rows
        {
            get { return ViewState["Rows"] == null ? 0 : (int)ViewState["Rows"]; }
            set { ViewState["Rows"] = value; }
        }

        private void cell_Click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;

            bool shouldRaiseDeSelect = false;

            // Checking to see if this is 'un-select' ...
            if (p.CssClass.Contains(" cal-selected-top-left"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-top-left", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-top-right"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-top-right", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-bottom-left"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-bottom-left", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-bottom-right"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-bottom-right", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-top"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-top", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-bottom"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-bottom", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-left"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-left", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected-right"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected-right", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else if (p.CssClass.Contains(" cal-selected"))
            {
                p.CssClass = p.CssClass.Replace(" cal-selected", "");
                PreviousSelected = null;
                shouldRaiseDeSelect = true;
            }
            else
            {
                DateTime date = 
                    DateTime.ParseExact(
                        p.Info.Split('|')[0], 
                        "yyyy.MM.dd", 
                        CultureInfo.InvariantCulture);

                SetCssClass(p);

                if (!string.IsNullOrEmpty(PreviousSelected))
                {
                    Panel old = Selector.FindControlClientID<Panel>(pnl, PreviousSelected);
                    if (old != null)
                    {
                        if (old.CssClass.Contains(" cal-selected-top-left"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-top-left", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-top-right"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-top-right", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-bottom-left"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-bottom-left", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-bottom-right"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-bottom-right", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-top"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-top", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-bottom"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-bottom", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-left"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-left", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected-right"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected-right", "");
                        }
                        else if (old.CssClass.Contains(" cal-selected"))
                        {
                            old.CssClass = old.CssClass.Replace(" cal-selected", "");
                        }
                    }
                }

                PreviousSelected = p.ClientID;

                Node node = new Node();
                node["Date"].Value = date;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    DataSource["DateClickedEvent"].Get<string>(),
                    node);
            }

            if (shouldRaiseDeSelect)
            {
                if (DataSource.Contains("DateDeSelectedClickedEvent"))
                {
                    Node node = new Node();
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        DataSource["DateDeSelectedClickedEvent"].Get<string>(),
                        node);
                }
            }
        }

        private void SetCssClass(Panel p)
        {
            string position =
                p.CssClass.Substring(
                    p.CssClass.IndexOf("pos"))
                .Split(' ')[0];

            switch (position)
            {
                case "pos-0":
                    p.CssClass += " cal-selected-top-left";
                    break;
                case "pos-1":
                case "pos-2":
                case "pos-3":
                case "pos-4":
                case "pos-5":
                    p.CssClass += " cal-selected-top";
                    break;
                case "pos-6":
                    p.CssClass += " cal-selected-top-right";
                    break;
                case "pos-7":
                case "pos-14":
                    p.CssClass += " cal-selected-left";
                    break;
                case "pos-13":
                case "pos-20":
                    p.CssClass += " cal-selected-right";
                    break;
                case "pos-21":
                    if (Rows == 4)
                        p.CssClass += " cal-selected-bottom-left";
                    else
                        p.CssClass += " cal-selected-left";
                    break;
                case "pos-27":
                    if (Rows == 4)
                        p.CssClass += " cal-selected-bottom-right";
                    else
                        p.CssClass += " cal-selected-right";
                    break;
                case "pos-28":
                    if (Rows == 5)
                        p.CssClass += " cal-selected-bottom-left";
                    else
                        p.CssClass += " cal-selected-left";
                    break;
                case "pos-34":
                    if (Rows == 5)
                        p.CssClass += " cal-selected-bottom-right";
                    else
                        p.CssClass += " cal-selected-right";
                    break;
                case "pos-35":
                    p.CssClass += " cal-selected-bottom-left";
                    break;
                case "pos-41":
                    p.CssClass += " cal-selected-bottom-right";
                    break;
                case "pos-22":
                case "pos-23":
                case "pos-24":
                case "pos-25":
                case "pos-26":
                    if (Rows == 4)
                        p.CssClass += " cal-selected-bottom";
                    else
                        p.CssClass += " cal-selected";
                    break;
                case "pos-29":
                case "pos-30":
                case "pos-31":
                case "pos-32":
                case "pos-33":
                    if (Rows == 5)
                        p.CssClass += " cal-selected-bottom";
                    else
                        p.CssClass += " cal-selected";
                    break;
                case "pos-36":
                case "pos-37":
                case "pos-38":
                case "pos-39":
                case "pos-40":
                    p.CssClass += " cal-selected-bottom";
                    break;
                default:
                    p.CssClass += " cal-selected";
                    break;
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}



