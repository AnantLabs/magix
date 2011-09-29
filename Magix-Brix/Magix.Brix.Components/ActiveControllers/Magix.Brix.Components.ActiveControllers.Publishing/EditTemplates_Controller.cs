/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Controller for editing Templates. Contains all the relevant event handlers and
     * logic for editing your templates
     */
    [ActiveController]
    public class EditTemplates_Controller : ActiveController
    {
        /**
         * Level2: Shows the grid containing all your Templates such that you can edit them
         * or create new Templates and such
         */
        [ActiveEvent(Name = "Magix.Publishing.EditTemplates")]
        protected void Magix_Publishing_EditTemplates(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(WebPageTemplate).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "large-bottom-margin mux-edit-templates";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Containers"].Value = true;
            node["WhiteListColumns"]["Containers"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 4;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Publishing.EditTemplate";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["ReuseNode"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
            node["Type"]["Properties"]["Containers"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Containers"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Containers"]["Header"].Value = "WebParts";
            node["Type"]["Properties"]["Containers"]["TemplateColumnEvent"].Value = "Magix.Publisher.GetNoContainersTemplateColumn";
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.Publisher.GetCopyTemplateColumn";
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;

            node["Container"].Value = "content3";

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level3: Creates the 'Copy Template' LinkButtons and returns back to the Grid system
         * so that we can have an implementation of 'Copy Template' button. LinkButton raises 
         * 'Magix.Publishing.CopyTemplate' when clicked
         */
        [ActiveEvent(Name = "Magix.Publisher.GetCopyTemplateColumn")]
        private void Magix_Publisher_GetCopyTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Creating our LinkButton
            LinkButton ls = new LinkButton();
            ls.Info = id.ToString();
            ls.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = id;

                    RaiseEvent(
                        "Magix.Publishing.CopyTemplate",
                        node);
                };
            ls.Text = name;

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Only here to make sure that if a WebPageTemplate is being deleted
         * from the Grid while editing it [for instance], we clear from 'content4'
         * and out to make sure we're not allowig for editing of a deleted Template
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            // TODO: Kind of lame, check to see what SPECIFIC template was deleted, and if not
            // the one being edited, then do NOT fluch content4 and out ...
            if (e.Params["FullTypeName"].Get<string>() == typeof(WebPageTemplate).FullName)
            {
                // In case it's the one being edited ...
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        /**
         * Level2: Will return all possible 'CSS Template Values' from the 'web-part-templates.css'
         * file within the media/module/ folders
         */
        [ActiveEvent(Name = "Magix.Publishing.GetCssTemplatesForWebPartTemplate")]
        protected void Magix_Publishing_GetCssTemplatesForWebPartTemplate(object sender, ActiveEventArgs e)
        {
            string fullCssFileName = Page.Server.MapPath("~/media/modules/web-part-templates.css");
            using (TextReader reader = new StreamReader(File.OpenRead(fullCssFileName)))
            {
                string wholeContent = reader.ReadToEnd();
                wholeContent = wholeContent.Replace("\r\n", "\n");
                int idxNo = 0;
                foreach (string idx in wholeContent.Split(
                    new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (idx.StartsWith(".") && 
                        !idx.Contains(" ") &&
                        !idx.Contains(":"))
                    {
                        e.Params["Classes"]["i-" + idxNo]["Name"].Value = idx.Substring(1);
                        e.Params["Classes"]["i-" + idxNo]["Value"].Value = idx.Trim('.').Trim(',');
                        idxNo += 1;
                    }
                }
            }
        }

        /**
         * Level2: Changes the CSS class of the WebPartTemplate instance according to the Selected Css value
         * chosen
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangeCssForWebPartTemplateFromCssTemplate")]
        protected void Magix_Publishing_ChangeCssForWebPartTemplateFromCssTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPartTemplate template = WebPartTemplate.SelectByID(e.Params["ID"].Get<int>());
                template.CssClass = e.Params["Value"].Get<string>();
                template.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Completes a 'deep copy' of the Template and saves it and loads up the Editing of
         * it for the user to immediately start editing it
         */
        [ActiveEvent(Name = "Magix.Publishing.CopyTemplate")]
        protected void Magix_Publishing_CopyTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPageTemplate original = WebPageTemplate.SelectByID(e.Params["ID"].Get<int>());
                WebPageTemplate clone = original.Clone();
                clone.Save();

                tr.Commit();

                Node n = new Node();

                n["FullTypeName"].Value = typeof(WebPageTemplate).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    n);

                n = new Node();
                n["FullTypeName"].Value = typeof(WebPageTemplate).FullName;
                n["ID"].Value = clone.ID;

                RaiseEvent(
                    "DBAdmin.Grid.SetActiveRow",
                    n);

                n = new Node();
                n["ID"].Value = clone.ID;

                RaiseEvent(
                    "Magix.Publishing.EditTemplate",
                    n);
            }
        }

        /**
         * Level2: Edits one specific WebPageTemplate by loading up the 
         * 'Magix.Brix.Components.ActiveModules.Publishing.EditSpecificTemplate' module
         */
        [ActiveEvent(Name = "Magix.Publishing.EditTemplate")]
        protected void Magix_Publishing_EditTemplate(object sender, ActiveEventArgs e)
        {
            WebPageTemplate t = WebPageTemplate.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            // Populating with Templates ...
            RaiseEvent(
                "Magix.Publishing.GetWebPageTemplates",
                node);

            RaiseEvent(
                "Magix.Publishing.GetPublisherPlugins",
                node);

            node["ID"].Value = t.ID;
            node["Width"].Value = 24;
            node["Last"].Value = true;

            // Adding some yellowish bg which reads 'full actual page size' [normally]
            node["CssClass"].Value = "mux-wysiwyg-surface";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.EditSpecificTemplate",
                "content4",
                node);
        }

        /**
         * Level2: Deletes a specific WebPartTemplate
         */
        [ActiveEvent(Name = "Magix.Publishing.DeleteWebPartTemplate")]
        protected void Magix_Publishing_DeleteWebPartTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPartTemplate templ = WebPartTemplate.SelectByID(e.Params["ID"].Get<int>());
                templ.PageTemplate.Containers.Remove(templ);
                templ.Delete();

                tr.Commit();
            }
        }

        /**
         * Level2: Returns a node of all WebPageTemplates in the system
         */
        [ActiveEvent(Name = "Magix.Publishing.GetWebPageTemplates")]
        protected void Magix_Publishing_GetWebPageTemplates(object sender, ActiveEventArgs e)
        {
            foreach (WebPageTemplate idx in WebPageTemplate.Select())
            {
                e.Params["Templates"]["t-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Templates"]["t-" + idx.ID]["ID"].Value = idx.ID;

                Node tmp = e.Params["Templates"]["t-" + idx.ID]["Containers"];
                foreach (WebPartTemplate idxC in idx.Containers)
                {
                    tmp["i-" + idxC.ID]["ID"].Value = idxC.ID;
                    tmp["i-" + idxC.ID]["Name"].Value = idxC.Name;
                    tmp["i-" + idxC.ID]["Height"].Value = idxC.Height;
                    tmp["i-" + idxC.ID]["Last"].Value = idxC.Last;
                    tmp["i-" + idxC.ID]["Overflow"].Value = idxC.Overflow;
                    tmp["i-" + idxC.ID]["Padding"].Value = idxC.MarginRight;
                    tmp["i-" + idxC.ID]["Top"].Value = idxC.MarginTop;
                    tmp["i-" + idxC.ID]["Width"].Value = idxC.Width;
                    tmp["i-" + idxC.ID]["Push"].Value = idxC.MarginLeft;
                    tmp["i-" + idxC.ID]["BottomMargin"].Value = idxC.MarginBottom;
                    tmp["i-" + idxC.ID]["ModuleName"].Value = idxC.ModuleName;
                    tmp["i-" + idxC.ID]["CssClass"].Value = idxC.CssClass;
                }
            }
        }

        /**
         * Level2: Returns a node of all PublisherPlugins you've got available in your installation
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPublisherPlugins")]
        protected void Magix_Publishing_GetPublisherPlugins(object sender, ActiveEventArgs e)
        {
            foreach (Type idx in Adapter.ActiveModules)
            {
                PublisherPluginAttribute[] atrs =
                    idx.GetCustomAttributes(typeof(PublisherPluginAttribute), true)
                        as PublisherPluginAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    e.Params["AllModules"][idx.FullName]["ModuleName"].Value = idx.FullName;
                    e.Params["AllModules"][idx.FullName]["ShortName"].Value = idx.Name;
                }
            }
        }

        /**
         * Level2: Changes the ModuleName [plugin type] of the WebPartTemplate and saves it
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangeModuleTypeForWebPartTemplate")]
        protected void Magix_Publishing_ChangeModuleTypeForWebPartTemplate(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPartTemplate t = WebPartTemplate.SelectByID(e.Params["ID"].Get<int>());
                t.ModuleName = e.Params["ModuleName"].Get<string>();
                t.Save();

                foreach (WebPage idx in WebPage.Select(
                        Criteria.ExistsIn(t.PageTemplate.ID, true)))
                {
                    // Forcing a 'rethink' of PageObjects beloning to this template
                    // to force [among other things] Default settings and such into the
                    // objects, and have them correctly numbered and aligned and such ...
                    idx.Save();
                }
                tr.Commit();

                Node node = new Node();
                node["ID"].Value = t.ID;

                RaiseEvent(
                    "Magix.Publishing.WebPartTemplateWasModified",
                    node);
            }
        }

        /**
         * Level2: Changes properties of the Template and saves it
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangeTemplateProperty")]
        protected void Magix_Publishing_ChangeTemplateProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPartTemplate t = WebPartTemplate.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params["Action"].Get<string>() == "IncreaseWidth")
                {
                    e.Params["OldWidth"].Value = t.Width;
                    t.Width = Math.Min(24, (
                        e.Params.Contains("NewValue") ? 
                            e.Params["NewValue"].Get<int>() : 
                            t.Width + 1));
                    e.Params["NewWidth"].Value = t.Width;
                }
                else if (e.Params["Action"].Get<string>() == "DecreaseWidth")
                {
                    e.Params["OldWidth"].Value = t.Width;
                    t.Width = Math.Max(2, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.Width - 1));
                    e.Params["NewWidth"].Value = t.Width;
                }
                else if (e.Params["Action"].Get<string>() == "IncreaseHeight")
                {
                    e.Params["OldHeight"].Value = t.Height;
                    t.Height = Math.Min(30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.Height + 1));
                    e.Params["NewHeight"].Value = t.Height;
                }
                else if (e.Params["Action"].Get<string>() == "DecreaseHeight")
                {
                    e.Params["OldHeight"].Value = t.Height;
                    t.Height = Math.Max(2, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.Height - 1));
                    e.Params["NewHeight"].Value = t.Height;
                }
                else if (e.Params["Action"].Get<string>() == "IncreaseDown")
                {
                    e.Params["OldTop"].Value = t.MarginTop;
                    t.MarginTop = Math.Min(30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginTop + 1));
                    e.Params["NewTop"].Value = t.MarginTop;
                }
                else if (e.Params["Action"].Get<string>() == "DecreaseDown")
                {
                    e.Params["OldTop"].Value = t.MarginTop;
                    t.MarginTop = Math.Max(-30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginTop - 1));
                    e.Params["NewTop"].Value = t.MarginTop;
                }
                else if (e.Params["Action"].Get<string>() == "IncreaseBottom")
                {
                    e.Params["OldMarginBottom"].Value = t.MarginBottom;
                    t.MarginBottom = Math.Min(30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginBottom + 1));
                    e.Params["NewMarginBottom"].Value = t.MarginBottom;
                }
                else if (e.Params["Action"].Get<string>() == "DecreaseBottom")
                {
                    e.Params["OldMarginBottom"].Value = t.MarginBottom;
                    t.MarginBottom = Math.Max(-30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginBottom - 1));
                    e.Params["NewMarginBottom"].Value = t.MarginBottom;
                }
                else if (e.Params["Action"].Get<string>() == "IncreaseLeft")
                {
                    e.Params["OldPush"].Value = t.MarginLeft;
                    t.MarginLeft = Math.Min(18, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginLeft + 1));
                    e.Params["NewPush"].Value = t.MarginLeft;
                }
                else if (e.Params["Action"].Get<string>() == "DecreaseLeft")
                {
                    e.Params["OldPush"].Value = t.MarginLeft;
                    t.MarginLeft = Math.Max(-30, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginLeft - 1));
                    e.Params["NewPush"].Value = t.MarginLeft;
                }
                else if (e.Params["Action"].Get<string>() == "IncreasePadding")
                {
                    e.Params["OldPadding"].Value = t.MarginRight;
                    t.MarginRight = Math.Min(18, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginRight + 1));
                    e.Params["NewPadding"].Value = t.MarginRight;
                }
                else if (e.Params["Action"].Get<string>() == "DecreasePadding")
                {
                    e.Params["OldPadding"].Value = t.MarginRight;
                    t.MarginRight = Math.Max(0, (
                        e.Params.Contains("NewValue") ?
                            e.Params["NewValue"].Get<int>() :
                            t.MarginRight - 1));
                    e.Params["NewPadding"].Value = t.MarginRight;
                }
                else if (e.Params["Action"].Get<string>() == "ChangeName")
                    t.Name = e.Params["Action"]["Value"].Get<string>();
                else if (e.Params["Action"].Get<string>() == "ChangeCssClass")
                    t.CssClass = e.Params["Value"].Get<string>();
                else if (e.Params["Action"].Get<string>() == "ChangeLast")
                    t.Last = e.Params["Value"].Get<bool>();
                else if (e.Params["Action"].Get<string>() == "ChangeOverflow")
                    t.Overflow = e.Params["Value"].Get<bool>();
                t.Save();
                tr.Commit();
            }
        }

        /**
         * Level3: Creates the Container column [select list] for selecting different number of 
         * WebParts for your template
         */
        [ActiveEvent(Name = "Magix.Publisher.GetNoContainersTemplateColumn")]
        protected void Magix_Publisher_GetNoContainersTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            int id = e.Params["ID"].Get<int>();

            WebPageTemplate t = WebPageTemplate.SelectByID(id);

            SelectList ls = new SelectList();

            ls.CssClass = "span-3 mux-grid-select";

            ls.SelectedIndexChanged +=
                delegate
                {
                    int count = int.Parse(ls.SelectedItem.Value);
                    RaiseTryToChangeNumberOfWebParts(t, count);
                };

            for (int n = 0; n < 8; n++)
            {
                ListItem i = new ListItem(n.ToString() + " parts", n.ToString());
                if (n == t.Containers.Count)
                    i.Selected = true;
                ls.Items.Add(i);
            }

            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Checks to see if number of webparts are decreasing, if they are, warning
         * user about that he should be careful, blah, blah, blah ...
         */
        private void RaiseTryToChangeNumberOfWebParts(WebPageTemplate t, int count)
        {
            if (count == t.Containers.Count)
                return;

            int affectedPages = WebPage.CountWhere(Criteria.ExistsIn(t.ID, true));

            if (count > t.Containers.Count || affectedPages == 0)
            {
                TryToChangeNumberOfWebParts(t, count);
            }
            else
            {
                Node node = new Node();
                node["ForcedSize"]["width"].Value = 550;
                node["WindowCssClass"].Value =
                    "mux-shaded mux-rounded push-5 down-2";

                node["Caption"].Value = @"Are you CERTAIN ...?";
                node["Text"].Value = string.Format(@"
<p>Are you sure you wish to do this? Reducing the number of WebParts on your Template will 
irrevocably delete every single WebPart on every sigle WebPage built upon that WebPart Template ...</p>
<p>This will affect {0} different pages ...</p>",
                    affectedPages);
                node["OK"]["ID"].Value = t.ID;
                node["OK"]["Event"].Value = "Magix.Publisher.ChangeNumberOFContainersOnTemplates-Confirmed";
                node["OK"]["Count"].Value = count;
                node["Cancel"]["Event"].Value = "Magix.Publisher.ChangeNumberOFContainersOnTemplates-NotConfirmed";
                node["Width"].Value = 15;

                LoadModule(
                    "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                    "child",
                    node);
            }
        }

        /*
         * Just closes the dialog box
         */
        [ActiveEvent(Name = "Magix.Publisher.ChangeNumberOFContainersOnTemplates-NotConfirmed")]
        protected void Magix_Publisher_ChangeNumberOFContainersOnTemplates_NotConfirmed(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Level2: Will change the number of WebParts in your WebPartTemplate
         */
        [ActiveEvent(Name = "Magix.Publisher.ChangeNumberOFContainersOnTemplates-Confirmed")]
        protected void Magix_Publisher_ChangeNumberOFContainersOnTemplates_Confirmed(object sender, ActiveEventArgs e)
        {
            TryToChangeNumberOfWebParts(
                WebPageTemplate.SelectByID(e.Params["ID"].Get<int>()), 
                e.Params["Count"].Get<int>());
            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /*
         * Helper for above
         */
        private void TryToChangeNumberOfWebParts(WebPageTemplate t, int count)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (t.Containers.Count < count)
                {
                    while (t.Containers.Count < count)
                    {
                        WebPartTemplate c = new WebPartTemplate();
                        c.Name = "Default Name";
                        c.Width = 10;
                        c.Height = 9;
                        c.ViewportContainer = "content" + (t.Containers.Count + 1);
                        c.ModuleName = Adapter.ActiveModules.Find(
                            delegate(Type idx)
                            {
                                PublisherPluginAttribute[] atrs = idx.GetCustomAttributes(typeof(PublisherPluginAttribute), true) as PublisherPluginAttribute[];
                                return atrs != null && atrs.Length > 0 && atrs[0].CanBeEmpty;
                            }).FullName;
                        t.Containers.Add(c);
                    }
                }
                else if (t.Containers.Count > count)
                {
                    while (t.Containers.Count - count > 0)
                    {
                        WebPartTemplate x = t.Containers[t.Containers.Count - 1];
                        t.Containers.RemoveAt(t.Containers.Count - 1);
                        x.Delete();
                    }
                }
                t.Save();
                tr.Commit();

                Node node = new Node();
                node["ID"].Value = t.ID;

                RaiseEvent(
                    "Magix.Publishing.WebPageTemplateWasModified",
                    node);
            }
        }
    }
}
