/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Globalization;
using Magix.UX.Builder;

namespace Magix.UX.Widgets
{
    public class Calendar : Panel
    {
        private LinkButton _previous = new LinkButton();
        private LinkButton _next = new LinkButton();
        private InPlaceEdit _month_year = new InPlaceEdit();

        public event EventHandler DateChanged;
        public event EventHandler DateSelected;

        public Calendar()
        {
            CssClass = "mux-calendar";
        }

        public DateTime Value
        {
            get { return ViewState["Value"] == null ? DateTime.MinValue : (DateTime)ViewState["Value"]; }
            set
            {
                if (value != Value)
                {
                    // Huuh...!??
                }
                ViewState["Value"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateCalendarControls();
        }

        private void CreateCalendarControls()
        {
            _previous.ID = "prev";
            _previous.Text = "<<";
            _previous.CssClass = "mux-calendar-previous";
            _previous.Click += _previous_Click;
            Controls.Add(_previous);

            _next.ID = "next";
            _next.Text = ">>";
            _next.CssClass = "mux-calendar-next";
            _next.Click += _next_Click;
            Controls.Add(_next);

            _month_year.ID = "monthYear";
            _month_year.CssClass = "mux-calendar-month-year";
            _month_year.TextChanged += _month_year_TextChanged;
            Controls.Add(_month_year);
        }

        void _month_year_TextChanged(object sender, EventArgs e)
        {
            TryParseText();
        }

        private void TryParseText()
        {
            string text = _month_year.Text.ToLowerInvariant();
            string[] entities = text.Split(' ');
            bool hasChanges = false;
            foreach (string idx in entities)
            {
                switch (idx)
                {
                    case "jan":
                    case "january":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 1, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "feb":
                    case "february":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 2, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "mar":
                    case "march":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 3, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "apr":
                    case "april":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 4, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "may":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 5, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "jun":
                    case "june":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 6, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "jul":
                    case "july":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 7, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "aug":
                    case "august":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 8, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "sep":
                    case "september":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 9, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "oct":
                    case "october":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 10, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "nov":
                    case "november":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 11, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    case "dec":
                    case "december":
                        hasChanges = true;
                        Value = new DateTime(Value.Year, 12, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        break;
                    default:
                        int year = -1;
                        if (int.TryParse(idx, out year))
                        {
                            hasChanges = true;
                            Value = new DateTime(year, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second);
                        }
                        break;
                }
            }
            if (hasChanges)
            {
                ReRender();
                if (DateChanged != null)
                    DateChanged(this, new EventArgs());
            }
        }

        void _previous_Click(object sender, EventArgs e)
        {
            Value = Value.AddMonths(-1);
            if (DateChanged != null)
                DateChanged(this, new EventArgs());
            ReRender();
        }

        void _next_Click(object sender, EventArgs e)
        {
            Value = Value.AddMonths(1);
            if (DateChanged != null)
                DateChanged(this, new EventArgs());
            ReRender();
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                case "changeDate":
                    string[] dateStr = Page.Request.Params["__date"].Split('_');
                    DateTime date = new DateTime(
                        int.Parse(dateStr[0]), 
                        int.Parse(dateStr[1]), 
                        int.Parse(dateStr[2]),
                        Value.Hour,
                        Value.Minute,
                        Value.Second);
                    Value = date;
                    ReRender();
                    if (DateChanged != null)
                        DateChanged(this, new EventArgs());
                    if (DateSelected != null)
                        DateSelected(this, new EventArgs());
                    break;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Value == DateTime.MinValue)
                Value = DateTime.Now.Date;
            _month_year.Text = Value.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            AjaxManager.Instance.IncludeScriptFromResource(
                typeof(Timer),
                "Magic_UX.Js.Calendar.js");
            base.OnPreRender(e);
        }

        private DateTime GetWeekOneDate(int year)
        {
            DateTime date = new DateTime(year, 1, 4);
            int dayNum = (int)date.DayOfWeek; // 0==Sunday, 6==Saturday
            if (dayNum == 0)
                dayNum = 7;
            return date.AddDays(1 - dayNum);
        }

        private int GetWeekNumber(DateTime dt)
        {
            int year = dt.Year;
            DateTime week1;
            if (dt >= new DateTime(year, 12, 29))
            {
                week1 = GetWeekOneDate(year + 1);
                if (dt < week1)
                {
                    week1 = GetWeekOneDate(year);
                }
            }
            else
            {
                week1 = GetWeekOneDate(year);
                if (dt < week1)
                {
                    week1 = GetWeekOneDate(--year);
                }
            }
            return ((dt - week1).Days / 7 + 1);
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement(Tag))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
                using (Element tbl = builder.CreateElement("table"))
                {
                    tbl.AddAttribute("class", "mux-calendar-table");
                    tbl.AddAttribute("id", ID + "tbl");
                    using (Element topRow = builder.CreateElement("tr"))
                    {
                        foreach (string idx in new string[] { "Wk.", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"})
                        {
                            using (Element cell = builder.CreateElement("td"))
                            {
                                cell.AddAttribute("class", "mux-calendar-week");
                                cell.Write(idx);
                            }
                        }
                    }
                    DateTime cur = new DateTime(Value.Year, Value.Month, 1);
                    while (cur.DayOfWeek != DayOfWeek.Sunday)
                        cur = cur.AddDays(-1);
                    while (cur.Month == Value.Month
                        || (Value.Month - cur.Month == 1)
                        || (cur.Month - Value.Month == 11))
                    {
                        using (Element row = builder.CreateElement("tr"))
                        {
                            using (Element leftCell = builder.CreateElement("td"))
                            {
                                leftCell.AddAttribute("class", "mux-calendar-week");
                                leftCell.Write(GetWeekNumber(cur).ToString());
                            }
                            for (int idxDay = 0; idxDay < 7; idxDay++)
                            {
                                using (Element cell = builder.CreateElement("td"))
                                {
                                    string cssClass = "";
                                    if (cur.Date == DateTime.Now.Date)
                                        cssClass += "mux-calendar-today ";
                                    if (cur.Date == Value.Date)
                                        cssClass += "mux-calendar-selected ";
                                    if (cur.Month == Value.Month)
                                    {
                                        cell.AddAttribute("class", cssClass + "mux-calendar-current-month");
                                    }
                                    else
                                    {
                                        cell.AddAttribute("class", cssClass + "mux-calendar-wrong-month");
                                    }
                                    cell.AddAttribute("id", ID + cur.ToString("yyyy_MM_dd"));
                                    cell.Write(cur.Day.ToString());
                                }
                                cur = cur.AddDays(1);
                            }
                        }
                    }
                }
            }
        }

        protected override string GetClientSideScriptType()
        {
            return "new MUX.Calendar";
        }

        protected override string GetClientSideScriptOptions()
        {
            string retVal = base.GetClientSideScriptOptions();
            retVal += "prefix:'" + ID + "'";
            return retVal;
        }
    }
}
