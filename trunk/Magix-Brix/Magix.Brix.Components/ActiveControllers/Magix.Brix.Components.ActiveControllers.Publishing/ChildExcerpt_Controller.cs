/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Helps out with some of the editing and using of the ChildExcerpt Module Type
     */
    [ActiveController]
    public class ChildExcerpt_Controller : ActiveController
    {
        /**
         * Level3: Creates a SelectList from which the no of articles in the ChildExcerpt Object can
         * be chosen
         */
        [ActiveEvent(Name = "Magix.Publishing.GetEditChildExcerptNoArticlesDropDown")]
        private void Magix_Publishing_GetEditChildExcerptNoArticlesDropDown(object sender, ActiveEventArgs e)
        {
            WebPage wp = WebPage.SelectByID(e.Params["ID"].Get<int>());

            foreach (WebPart idx in wp.WebParts)
            {
                string moduleName = idx.Container.ModuleName;
                switch (moduleName)
                {
                    case "Magix.Brix.Components.ActiveModules.Publishing.Content":
                        {
                            Label lbl = new Label();
                            lbl.Tag = "p";
                            lbl.CssClass = "mux-excerpt-text";
                            string text = idx.Settings.Find(
                                delegate(WebPart.WebPartSetting idxI)
                                {
                                    return idxI.Name == moduleName + "Text";
                                }).Value;
                            text = Regex.Replace(text, "<(.|\n)*?>", string.Empty);
                            if (text.Length > 150)
                                text = text.Substring(0, 147) + "...";
                            lbl.Text = text;
                            e.Params["Controls"]["Magix.Brix.Components.ActiveModules.Publishing.Content"].Value = lbl;
                        } break;
                }
            }
        }

        /**
         * Level3: The actual construction of our Child Excerpt panels are being done here
         */
        [ActiveEvent(Name = "Magix.Publishing.BuildOneChildExcerptControl")]
        private void Magix_Publishing_BuildOneChildExcerptControl(object sender, ActiveEventArgs e)
        {
            WebPage wp = WebPage.SelectByID(e.Params["ID"].Get<int>());

            Node ch = new Node();
            ch["ID"].Value = wp.ID;

            RaiseEvent(
                "Magix.Publishing.VerifyUserHasAccessToPage",
                ch);

            if (!ch.Contains("STOP") ||
                !ch["STOP"].Get<bool>())
            {
                Panel pnl = new Panel();
                pnl.CssClass = "mux-excerpt-item";
                pnl.Click +=
                    delegate
                    {
                        Node x = new Node();
                        x["ID"].Value = wp.ID;

                        RaiseEvent(
                            "Magix.Publishing.OpenPage",
                            x);
                    };

                Label lb = new Label();
                lb.Tag = "h3";
                lb.Text = wp.Name;
                pnl.Controls.Add(lb);

                Node node = new Node();
                node["ID"].Value = wp.ID;

                RaiseEvent(
                    "Magix.Publishing.GetEditChildExcerptNoArticlesDropDown",
                    node);
                if (node.Contains("Controls"))
                {
                    foreach (Node idx in node["Controls"])
                    {
                        pnl.Controls.Add(idx.Value as System.Web.UI.Control);
                    }
                }
                e.Params["Control"].Value = pnl;
            }
        }

        /**
         * Level2: Will return the last 'Count' number of pages, sorted according to newest first
         */
        [ActiveEvent(Name = "Magix.Publishing.GetLastChildrenPages")]
        private void Magix_Publishing_GetLastChildrenPages(object sender, ActiveEventArgs e)
        {
            WebPart part = WebPart.SelectByID(e.Params["ID"].Get<int>());

            WebPage page = part.WebPage;

            // Doing it the hard way, in case there are 'millions' of pages .......
            foreach (WebPage idx in WebPage.Select(
                Criteria.ParentId(page.ID),
                Criteria.Range(0, e.Params["Count"].Get<int>(), "Created", false)))
            {
                e.Params["Items"]["i" + idx.ID]["ID"].Value = idx.ID;
            }
        }

        /**
         * Level3: Will create a SelectList for editing of the number of ChildExcerpts in the WebPart
         */
        [ActiveEvent(Name = "Magix.Publishing.GetTemplateColumnSelectChildExcerptNo")]
        protected void Magix_Publishing_GetTemplateColumnSelectChildExcerptNo(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ls.Items.Add(new ListItem("5", "5"));
            ls.Items.Add(new ListItem("10", "10"));
            ls.Items.Add(new ListItem("15", "15"));
            ls.Items.Add(new ListItem("20", "20"));
            ls.Items.Add(new ListItem("25", "25"));
            switch (e.Params["Value"].Value.ToString())
            {
                case "5":
                    ls.SelectedIndex = 0;
                    break;
                case "10":
                    ls.SelectedIndex = 1;
                    break;
                case "15":
                    ls.SelectedIndex = 2;
                    break;
                case "20":
                    ls.SelectedIndex = 3;
                    break;
                case "25":
                    ls.SelectedIndex = 4;
                    break;
                default:
                    ls.Enabled = false;
                    break;
            }
        }
    }
}
