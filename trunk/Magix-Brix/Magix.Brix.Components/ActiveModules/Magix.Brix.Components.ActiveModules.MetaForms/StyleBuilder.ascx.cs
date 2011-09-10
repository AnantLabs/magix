/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;
using System.Reflection;
using System.Drawing;
using Magix.UX.Effects;
using Magix.UX.Aspects;

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     */
    [ActiveModule]
    public class StyleBuilder : ActiveModule
    {
        protected MultiPanel mp;
        protected TabStrip mub;
        protected TabButton mb1;
        protected TabButton mb2;
        protected TabButton mb3;
        protected TabButton mb4;
        protected Panel fgText;
        protected Panel bgText;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    SetActiveMultiViewIndex(0);
                };
        }

        private void SetActiveMultiViewIndex(int index)
        {
            switch (index)
            {
                case 0:
                    mb1.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view";
                    break;
                case 1:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view";
                    break;
                case 2:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    mb4.CssClass = "mux-multi-button-view";
                    break;
                case 3:
                    mb1.CssClass = "mux-multi-button-view";
                    mb2.CssClass = "mux-multi-button-view";
                    mb3.CssClass = "mux-multi-button-view";
                    mb4.CssClass = "mux-multi-button-view mux-multi-view-button-selected";
                    break;
            }
        }

        protected void mp_MultiButtonClicked(object sender, EventArgs e)
        {
            if (mub.ActiveMultiButtonViewIndex == 3)
            {
                SetActiveMultiViewIndex(3);
            }
            else if (mub.ActiveMultiButtonViewIndex == 2)
            {
                SetActiveMultiViewIndex(2);
            }
            else if (mub.ActiveMultiButtonViewIndex == 1)
            {
                SetActiveMultiViewIndex(1);
            }
            else if (mub.ActiveMultiButtonViewIndex == 0)
            {
                SetActiveMultiViewIndex(0);
            }
        }

        protected void fgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;

            node["SelectEvent"].Value = "Magix.MetaForms.FGColorColorWasPicked";
            node["Caption"].Value = "Pick Foreground Color for Widget";
            node["NoImage"].Value = true;

            if (DataSource["Style"].Contains("color"))
                node["Color"].Value = DataSource["Style"]["color"].Value;
            else
                node["Color"].Value = "#000000";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        protected void bgText_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Top"].Value = 20;

            node["SelectEvent"].Value = "Magix.MetaForms.BGColorColorWasPicked";
            node["Caption"].Value = "Pick Background Color for Widget";

            if (DataSource["Style"].Contains("color"))
                node["Color"].Value = DataSource["Style"]["color"].Value;
            else
                node["Color"].Value = "#FFFFFF";

            RaiseSafeEvent(
                "Magix.Core.PickColorOrImage",
                node);
        }

        /**
         * Level2: Will change the Foreground Color setting of the builder to the given 
         * 'Color' parameter
         */
        [ActiveEvent(Name = "Magix.MetaForms.FGColorColorWasPicked")]
        protected void Magix_MetaForms_FGColorColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
            string color = e.Params["Color"].Get<string>();
            fgText.Style[Styles.backgroundColor] = color;
        }

        /**
         * Level2: Will change the Background Color setting of the builder to the given 
         * 'Color' parameter
         */
        [ActiveEvent(Name = "Magix.MetaForms.BGColorColorWasPicked")]
        protected void Magix_MetaForms_BGColorColorWasPicked(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");

            if (e.Params.Contains("Color"))
            {
                string color = e.Params["Color"].Get<string>();
                bgText.Style[Styles.backgroundColor] = color;

                bgText.Style[Styles.backgroundImage] = "";
            }
            else
            {
                string img = e.Params["FileName"].Get<string>();

                bgText.Style[Styles.backgroundImage] = "url(" + img + ")";
                bgText.Style[Styles.backgroundAttachment] = "scroll";
                bgText.Style[Styles.backgroundPosition] = "0 0";
                bgText.Style[Styles.backgroundRepeat] = "no-repeat";

                bgText.Style[Styles.backgroundColor] = "";

                // There are TWO popups here now ...
                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }
    }
}
