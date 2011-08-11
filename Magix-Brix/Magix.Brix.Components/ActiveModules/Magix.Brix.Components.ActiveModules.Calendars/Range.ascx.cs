/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
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

namespace Magix.Brix.Components.ActiveModules.Calendars
{
    [ActiveModule]
    public class Range : UserControl, IModule
    {
        protected Window wnd;
        protected Calendar calStart;
        protected Calendar calEnd;
        protected Button changeDate;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    calStart.Value = node["Start"].Get<DateTime>();
                    calEnd.Value = node["End"].Get<DateTime>();
                    AutoUpdate = node["AutoUpdate"].Get<bool>();
                    if (AutoUpdate)
                    {
                        changeDate.Visible = false;
                        wnd.CssClass += " select-date-range-no-button";
                    }
                    FormatCaption();
                };
        }

        private bool AutoUpdate
        {
            get { return (bool)ViewState["AutoUpdate"]; }
            set { ViewState["AutoUpdate"] = value; }
        }

        protected void DateSelected(object sender, EventArgs e)
        {
            FormatCaption();
            if (AutoUpdate)
            {
                RaiseChangedEvent();
            }
            if (changeDate.Visible)
            {
                changeDate.Focus();
            }
        }

        private void FormatCaption()
        {
            wnd.Caption = string.Format("{0} - {1}",
                calStart.Value.ToString("dddd MMM d "),
                calEnd.Value.ToString("dddd MMM d "));
        }

        protected void changeDate_Click(object sender, EventArgs e)
        {
            RaiseChangedEvent();
        }

        private void RaiseChangedEvent()
        {
            Node node = new Node();
            node["Start"].Value = calStart.Value;
            node["End"].Value = calEnd.Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "NewDateRangeSelected",
                node);
        }
    }
}



