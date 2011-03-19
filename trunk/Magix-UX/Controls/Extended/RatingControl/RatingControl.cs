/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
    public class RatingControl : Panel
    {
        public event EventHandler Rated;

        public RatingControl()
        {
            CssClass = "mux-rating";
        }

        [DefaultValue(5)]
        public int MaxValue
        {
            get { return ViewState["MaxValue"] == null ? 5 : (int)ViewState["MaxValue"]; }
            set { ViewState["MaxValue"] = value; }
        }

        [DefaultValue(0)]
        public int Value
        {
            get { return ViewState["Value"] == null ? 0 : (int)ViewState["Value"]; }
            set { ViewState["Value"] = value; }
        }

        [DefaultValue(0)]
        public int Average
        {
            get { return ViewState["Average"] == null ? 0 : (int)ViewState["Average"]; }
            set { ViewState["Average"] = value; }
        }

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return ViewState["Enabled"] == null ? true : (bool)ViewState["Enabled"]; }
            set { ViewState["Enabled"] = value; }
        }

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
            for (int idx = 0; idx < MaxValue; idx++)
            {
                Label l = new Label();
                l.ID = "rat" + idx;
                l.Info = idx.ToString();
                if (Enabled)
                    l.Click += l_Click;
                Controls.Add(l);
                if (Enabled)
                {
                    l.ClickEffect = new EffectGeneric(
                        string.Format(@"
function() {{
  for(var idx = 0; idx <= {1}; idx++) {{
    MUX.$('{0}_rat' + idx).addClassName('mux-rating-star-checked');
    MUX.$('{0}_rat' + idx).removeClassName('mux-rating-star-unchecked');
  }}
  for(var idx = {1} + 1; idx < {2}; idx++) {{
    MUX.$('{0}_rat' + idx).addClassName('mux-rating-star-unchecked');
    MUX.$('{0}_rat' + idx).removeClassName('mux-rating-star-checked');
  }}
}}",
                        this.ClientID,
                        idx,
                        MaxValue));
                }
            }
        }

        void l_Click(object sender, EventArgs e)
        {
            Label l = sender as Label;
            int score = int.Parse(l.Info);
            Value = score;
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
            if (!this.HasRendered || !AjaxManager.Instance.IsCallback)
            {
                for (int idx = 0; idx < MaxValue; idx++)
                {
                    Label l = Selector.FindControl<Label>(this, "rat" + idx);
                    l.CssClass = "mux-rating-star";
                    if (Value >= idx)
                    {
                        l.CssClass += " mux-rating-star-checked";
                    }
                    else
                    {
                        if (Average >= idx)
                        {
                            l.CssClass += " mux-rating-star-average";
                        }
                        else
                        {
                            l.CssClass += " mux-rating-star-unchecked";
                        }
                    }
                    if (ShowAlternativeIcons)
                    {
                        if (idx == 0)
                        {
                            l.CssClass += " mux-rating-star-min";
                        }
                        else if (idx == MaxValue - 1)
                        {
                            l.CssClass += " mux-rating-star-max";
                        }
                        else if ((MaxValue - (MaxValue % 2)) / 2 == idx)
                        {
                            l.CssClass += " mux-rating-star-mid";
                        }
                    }
                }
            }
        }
    }
}
