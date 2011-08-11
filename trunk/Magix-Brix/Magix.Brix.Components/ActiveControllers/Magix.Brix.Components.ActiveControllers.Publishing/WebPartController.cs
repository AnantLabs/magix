/*
 * Magix - A Modular-based Framework for building Web Applications 
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
    [ActiveController]
    public class WebPartController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.InitializePublishingPlugin")]
        protected void Magix_Publishing_InitializePublishingPlugin(object sender, ActiveEventArgs e)
        {
            WebPart t = WebPart.SelectByID(e.Params["ID"].Get<int>());
            Type moduleType = Adapter.ActiveModules.Find(
                delegate(Type idx)
                {
                    return idx.FullName == t.Container.ModuleName;
                });
            foreach (PropertyInfo idx in 
                moduleType.GetProperties(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Instance))
            {
                ModuleSettingAttribute[] atrs =
                    idx.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                    as ModuleSettingAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    string propName = idx.Name;
                    foreach (WebPart.WebPartSetting idxSet in t.Settings)
                    {
                        if (idxSet.Name == moduleType.FullName + idx.Name)
                        {
                            object nValue = Convert.ChangeType(idxSet.Value, idx.PropertyType, CultureInfo.InvariantCulture);
                            idx.GetSetMethod(true).Invoke(e.Params["_ctrl"].Value, new object[] { nValue });
                            break;
                        }
                    }
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.FindFirstChildPageUserCanAccess")]
        protected void Magix_Publishing_FindFirstChildPageUserCanAccess(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

            // Assuming already checked for access against this bugger ...
            foreach (WebPage idx in p.Children)
            {
                if (CheckAccess(p, e.Params))
                {
                    return;
                }
            }
        }

        private bool CheckAccess(WebPage p, Node node)
        {
            Node ch1 = new Node();
            ch1["ID"].Value = p.ID;
            RaiseEvent(
                "Magix.Publishing.VerifyUserHasAccessToPage",
                ch1);

            if (!ch1.Contains("STOP") ||
                !ch1["STOP"].Get<bool>())
            {
                node["AccessToID"].Value = p.ID;
                return true;
            }
            foreach (WebPage idx in p.Children)
            {
                if (CheckAccess(idx, node))
                {
                    return true;
                }
            }
            return false;
        }

        [ActiveEvent(Name = "Magix.Publishing.GetWebPartValue")]
        private void Magix_Publishing_GetWebPartValue(object sender, ActiveEventArgs e)
        {
            WebPart part = WebPart.SelectByID(e.Params["WebPartID"].Get<int>());

            foreach (WebPart.WebPartSetting idx in part.Settings)
            {
                if (idx.Name == part.Container.ModuleName + e.Params["Name"].Get<string>())
                {
                    e.Params["Value"].Value = idx.Value;
                    break;
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.GetChildExcerptAdditionalControls")]
        private void Magix_Publishing_GetChildExcerptAdditionalControls(object sender, ActiveEventArgs e)
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
                            lbl.CssClass = "excerpt-excerpt";
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
                pnl.CssClass = "excerpt-item";
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
                    "Magix.Publishing.GetChildExcerptAdditionalControls",
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

                    tx["Params"]["ID"].Value = e.Params["ID"].Value;
                    tx["Params"]["PropertyName"].Value = "Magix.Brix.Components.ActiveModules.Publishing.ChildExcerptPagesCount";
                    tx["Params"]["PotID"].Value = e.Params["PotID"].Value;
                    tx["Text"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.SavePageObjectIDSetting",
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

        [ActiveEvent(Name = "Magix.Publishing.InjectPlugin")]
        private void Magix_Publishing_InjectPlugin(object sender, ActiveEventArgs e)
        {
            WebPart page = WebPart.SelectByID(e.Params["ID"].Get<int>());

            Node ch = new Node();

            ch["ModuleName"].Value = page.Container.ModuleName;
            ch["Container"].Value = page.Container.ViewportContainer;
            ch["WebPartID"].Value = page.ID;

            RaiseEvent(
                "Magix.Publishing.ShouldReloadWebPart",
                ch);

            if (!ch.Contains("Stop") || !ch["Stop"].Get<bool>())
            {
                Node node = new Node();

                node["BottomMargin"].Value = page.Container.MarginBottom;
                node["CssClass"].Value = page.Container.CssClass;
                node["Height"].Value = page.Container.Height;
                if (page.Container.Last)
                    node["Last"].Value = true;
                node["PushRight"].Value = page.Container.MarginRight;
                node["PushLeft"].Value = page.Container.MarginLeft;
                node["SpcBottom"].Value = page.Container.MarginBottom;
                node["Top"].Value = page.Container.MarginTop;
                node["Width"].Value = page.Container.Width;
                node["ID"].Value = page.ID;
                node["ModuleInitializationEvent"].Value = "Magix.Publishing.InitializePublishingPlugin";
                node["PageObjectTemplateID"].Value = page.ID;

                node["CssClass"].Value = "web-part" + " " + page.Container.CssClass;

                if (page.Container.Overflow)
                    node["OverflowWebPart"].Value = true;

                LoadModule(
                    page.Container.ModuleName,
                    page.Container.ViewportContainer,
                    node);
            }
            else
            {
                // Don't need to Inject Module for some reasons. It might be a Sliding Menu for instance ...
                // Though we DO need to UPDATE SETTINGS for module, since it might still be a different template ...
                Node node = new Node();

                node["MarginBottom"].Value = page.Container.MarginBottom;
                node["CssClass"].Value = page.Container.CssClass;
                node["Height"].Value = page.Container.Height;
                node["Last"].Value = page.Container.Last;
                node["PushRight"].Value = page.Container.MarginRight;
                node["PushLeft"].Value = page.Container.MarginLeft;
                node["SpcBottom"].Value = page.Container.MarginBottom;
                node["Top"].Value = page.Container.MarginTop;
                node["Width"].Value = page.Container.Width;

                string cssClass = "web-part";
                if (page.Container.Overflow)
                    cssClass += " web-part-overflow";
                cssClass += " " + page.Container.CssClass;
                node["CssClass"].Value = cssClass;

                node["Container"].Value = page.Container.ViewportContainer;

                RaiseEvent(
                    "Magix.Core.SetViewPortContainerSettings",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ReloadWebPart")]
        protected void Magix_Publishing_ReloadWebPart(object sender, ActiveEventArgs e)
        {
            WebPart t = WebPart.SelectByID(e.Params["PageObjectTemplateID"].Get<int>());

            Node node = new Node();

            node["Container"].Value = e.Params["Parameters"]["Container"].Value;
            node["FreezeContainer"].Value = true;
            node["ID"].Value = t.ID;

            RaiseEvent(
                "Magix.Publishing.InjectPlugin",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetStateForLoginControl")]
        protected void Magix_Publishing_GetStateForLoginControl(object sender, ActiveEventArgs e)
        {
            if (User.Current != null)
            {
                e.Params["ShouldLoadLogout"].Value = true;
            }
            else
            {
                e.Params["ShouldLoadLogin"].Value = true;
            }
        }

        [ActiveEvent(Name = "Magix.Core.GetContainerForControl")]
        protected void Magix_Core_GetContainerForControl(object sender, ActiveEventArgs e)
        {
            if (e.Params.Contains("PageObjectTemplateID"))
            {
                e.Params["Container"].Value =
                    WebPart.SelectByID(e.Params["PageObjectTemplateID"].Get<int>()).Container.ViewportContainer;
                e.Params["FreezeContainer"].Value = true;
            }
        }
    }
}

























