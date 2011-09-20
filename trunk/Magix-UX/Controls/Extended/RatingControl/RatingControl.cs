/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;
using Magix.UX.Effects;

namespace Magix.UX.Widgets
{
    /**
     * Encapsulates a Rating Control, for allowing the end user to choose number of stars, or 1-x 
     * value of something
     */
    public class RatingControl : Panel
    {
        /**
         * Raised when the user chose to click one of the stars [or other types of objects you are using to signify the value]
         * Use the Value property to retrieve which value the end user gave the object
         */
        public event EventHandler Rated;

        public RatingControl()
        {
            CssClass = "mux-rating";
        }

        /**
         * Maximum number of stars [rating objects]
         */
        [DefaultValue(5)]
        public int MaxValue
        {
            get { return ViewState["MaxValue"] == null ? 5 : (int)ViewState["MaxValue"]; }
            set { ViewState["MaxValue"] = value; }
        }

        /**
         * Its value, [0 - MaxValue], 0 equals no-value
         */
        [DefaultValue(0)]
        public int Value
        {
            get { return ViewState["Value"] == null ? 0 : (int)ViewState["Value"]; }
            set { ViewState["Value"] = value; }
        }

        /**
         * Will show the average score using different colors if you wish
         */
        [DefaultValue(0)]
        public int Average
        {
            get { return ViewState["Average"] == null ? 0 : (int)ViewState["Average"]; }
            set { ViewState["Average"] = value; }
        }

        /**
         * If true [default] the control is enabled, else disabled
         */
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return ViewState["Enabled"] == null ? true : (bool)ViewState["Enabled"]; }
            set { ViewState["Enabled"] = value; }
        }

        /**
         * If true, will show alternative icons for every second star
         */
        public bool ShowAlternativeIcons
        {
            get { return ViewState["ShowAlternativeIcons"] == null ? true : (bool)ViewState["ShowAlternativeIcons"]; }
            set { ViewState["ShowAlternativeIcons"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            EnsureID();

            for (int idx = 0; idx <= MaxValue; idx++)
            {
                Label l = new Label();
                l.ID = ID + "x" + idx;
                l.Info = idx.ToString();

                if (Enabled)
                    l.Click += l_Click;

                Controls.Add(l);
            }
        }

        void l_Click(object sender, EventArgs e)
        {
            Label l = sender as Label;
            int score = int.Parse(l.Info);
            Value = score + 1;
            if (Rated != null)
            {
                Rated(this, new EventArgs());
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetCssClass();
            base.OnPreRender(e);
        }

        private void SetCssClass()
        {
            for (int idx = 0; idx <= (MaxValue - 1); idx++)
            {
                Label l = Selector.FindControl<Label>(this, ID + "x" + idx);
                string tmp = "mux-rating-star";
                if ((Value - 1) >= idx)
                {
                    tmp += " mux-rating-star-checked";
                }
                else
                {
                    if ((Average - 1) >= idx)
                    {
                        tmp += " mux-rating-star-average";
                    }
                    else
                    {
                        tmp += " mux-rating-star-unchecked";
                    }
                }
                if (ShowAlternativeIcons)
                {
                    if (idx == 0)
                    {
                        tmp += " mux-rating-star-min";
                    }
                    else if (idx == (MaxValue - 1))
                    {
                        tmp += " mux-rating-star-max";
                    }
                    else if (((MaxValue - 1) - ((MaxValue - 1) % 2)) / 2 == idx)
                    {
                        tmp += " mux-rating-star-mid";
                    }
                }
                l.CssClass = tmp;
            }
        }
    }
}
