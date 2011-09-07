﻿/*
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
using Magix.Brix.Components.ActiveTypes.Users;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Contains logic for Editing WebPage objects and such for administrator
     */
    [ActiveController]
    public class EditPages_Controller : ActiveController
    {
        #region [ -- ApplicationStartup, creation of some default objects -- ]

        /**
         * Level2: Overridden to make sure we've got some default pages during startup if none 
         * exists from before
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (WebPageTemplate.Count == 0)
                {
                    // Creating a default template ...
                    WebPageTemplate t1 = new WebPageTemplate();
                    t1.Name = "M+H+C";
                    
                    WebPartTemplate c1 = new WebPartTemplate();
                    c1.Name = "Menu";
                    c1.CssClass = "menu";
                    c1.ViewportContainer = "content1";
                    c1.Width = 6;
                    c1.Height = 20;
                    c1.MarginTop = 13;
                    c1.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.SliderMenu";
                    t1.Containers.Add(c1);

                    WebPartTemplate c2 = new WebPartTemplate();
                    c2.Name = "Header";
                    c2.CssClass = "header";
                    c2.ViewportContainer = "content2";
                    c2.Width = 18;
                    c2.MarginTop = 6;
                    c2.Height = 5;
                    c2.Last = true;
                    c2.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Header";
                    t1.Containers.Add(c2);

                    WebPartTemplate c3 = new WebPartTemplate();
                    c3.Name = "Content";
                    c3.CssClass = "content";
                    c3.ViewportContainer = "content3";
                    c3.Width = 18;
                    c3.Height = 30;
                    c3.MarginBottom = 10;
                    c3.Overflow = true;
                    c3.MarginTop = 2;
                    c3.Last = true;
                    c3.ModuleName = "Magix.Brix.Components.ActiveModules.Publishing.Content";
                    t1.Containers.Add(c3);

                    t1.Save();
                }
                if (WebPage.Count == 0)
                {
                    // Creating a default page ...
                    WebPage o = new WebPage();
                    o.Name = "Welcome to Magix!";
                    o.Template = WebPageTemplate.SelectFirst(Criteria.Eq("Name", "Menu Left"));

                    WebPart t1 = new WebPart();
                    t1.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Menu"));
                    o.WebParts.Add(t1);

                    WebPart t2 = new WebPart();
                    t2.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Header"));
                    WebPart.WebPartSetting s1 = new WebPart.WebPartSetting();
                    s1.Name = "Magix.Brix.Components.ActiveModules.Publishing.HeaderCaption";
                    s1.Value = "Welcome to Magix";
                    t2.Settings.Add(s1);
                    o.WebParts.Add(t2);

                    WebPart t3 = new WebPart();
                    t3.Container = WebPartTemplate.SelectFirst(Criteria.Eq("Name", "Content"));
                    WebPart.WebPartSetting s2 = new WebPart.WebPartSetting();
                    s2.Name = "Magix.Brix.Components.ActiveModules.Publishing.ContentText";
                    s2.Value = @"
<img src=""media/images/magix-logo.png"" class=""horus-ra-image"" style=""margin-left:20px;float:right;display:block;width:200px;"" alt=""Magix, where Dreams come Through ...""/>
<p>By default a user has been created with the username/password of admin/admin</p>
<p>You can login with this user to the Back-Web Dashboard <a href=""?dashboard=true"">here</a> ...</p>
<h3>What is Magix ...?</h3>
<p>Magix is difficult to describe. Some will say it's a 'better CMS' or a 'Publishing System', 
others might say that it's a System Development Platform, some might call it a Web Operating System, 
others again might say it's a Social Media platform, for collaboration across Organizations. Some developers 
will claim it's nothing but a RAD implementation of Lisp on top of ASP.NET and WebControls. And so on ...</p>
<p>Regardless, they're probably all right! Every person who looks at Magix will probably categorize it 
differently than the next person out there! </p>
<p>That's because <strong>Magix is Everything</strong>! Regardless of what types of IT problems you've got, Magix is probably the best solution for you.</p>
<p>With Magix you can create complex web applications, that runs on everything, within hours after you've started using it</p>
<p>Need your own iTunes, create one within a couple of hours. Need a new CRM application? Spend 2-4 hours reading the docs for Magix, and you'd be surprised at how much you'd be able to implement, even with zero coding experience from before</p>
<p>Oh yeah! Almost forgot the most important thing; <strong>Magis is Free Software!</strong> ...</p>
<p>However, regardless of what you think about Magix, we consider it our gift, from us, to You, and the Rest of The World!</p>
<p>To sum it all up, in a couple of meaningful words, which I think accurately describes Magix; </p>
<p><strong>Yup!<br/>It's Game Over!<br/>Equilibrium to the Force, And YOU Won ... ! ;)</strong></p>
<p>Have a nice life!</p>
<p>.t</p>
";
                    t3.Settings.Add(s2);
                    o.WebParts.Add(t3);

                    o.Save();
                }
                tr.Commit();
            }
        }

        #endregion

        /**
         * Level2: Will load a Tree of the Pages from which you can browse around within to edit
         * and view the relationships between all your WebPage objects
         */
        [ActiveEvent(Name = "Magix.Publishing.EditPages")]
        protected void Magix_Publishing_EditPages(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            RaiseEvent(
                "Magix.Publishing.GetEditPagesDataSource",
                node);

            node["CssClass"].Value = "edit-pages-tree-view";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Header"].Value = "Select a WebPage ...";
            node["FullTypeName"].Value = typeof(WebPage).FullName;
            node["ItemSelectedEvent"].Value = "Magix.Publishing.EditSpecificPage";
            node["GetItemsEvent"].Value = "Magix.Publishing.GetEditPagesDataSource";
            node["NoClose"].Value = true;

            RaiseEvent(
                "Magix.Publishing.GetEditPagesDataSource",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Tree",
                "content3",
                node);

        }

        /**
         * Level2: Will return the WebPages in your system back to caller in the return value as 'Items'
         * collections
         */
        [ActiveEvent(Name = "Magix.Publishing.GetEditPagesDataSource")]
        protected void Magix_Publishing_GetEditPagesDataSource(object sender, ActiveEventArgs e)
        {
            foreach (WebPage idx in WebPage.Select())
            {
                if (idx.Parent != null)
                    continue; // Only 'Root' objects ...

                e.Params["Items"]["P" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Items"]["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, e.Params["Items"]["P" + idx.ID]);
            }
        }

        /*
         * Helper for above ...
         */
        private void DoChildren(WebPage parent, Node node)
        {
            foreach (WebPage idx in parent.Children)
            {
                node["Items"]["P" + idx.ID]["Name"].Value = idx.Name;
                node["Items"]["P" + idx.ID]["ID"].Value = idx.ID;

                if (idx.Children.Count > 0)
                    DoChildren(idx, node["Items"]["P" + idx.ID]);
            }
        }

        /**
         * Level2: Will edit one Specific page according to either SelectedItemID [Tree] or ID parameter
         */
        [ActiveEvent(Name = "Magix.Publishing.EditSpecificPage")]
        protected void Magix_Publishing_EditSpecificPage(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(
                e.Params.Contains("SelectedItemID") ?
                int.Parse(e.Params["SelectedItemID"].Value.ToString()) :
                e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["Top"].Value = 1;

            node["ID"].Value = p.ID;
            node["Header"].Value = p.Name;
            node["URL"].Value = p.URL;
            node["TemplateID"].Value = p.Template.ID;

            RaiseEvent(
                "Magix.Publishing.GetWebPageTemplates",
                node);

            GetWebParts(p, node);

            foreach (Role idx in Role.Select())
            {
                node["Roles"]["r-" + idx.ID]["Name"].Value = idx.Name;
                node["Roles"]["r-" + idx.ID]["ID"].Value = idx.ID;
            }

            RaiseEvent(
                "Magix.Publishing.GetRolesListForPage",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Publishing.EditSpecificPage",
                "content4",
                node);
        }

        /**
         * Level2: Will make sure we update our Tree control
         */
        [ActiveEvent(Name = "Magix.Publishing.PageWasUpdated")]
        protected void Magix_Publishing_PageWasUpdated(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = typeof(WebPage).FullName + "-META";

            RaiseEvent(
                "Magix.Core.UpdateTree",
                node);
        }

        /*
         * Helper method for above to stuff 'WebPageTemplate' data into Node structure 
         * before loading EditSpecificPage
         */
        private static void GetWebParts(WebPage p, Node node)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                foreach (WebPart idx in p.WebParts)
                {
                    if (idx.Container == null)
                    {
                        // Cleaning up .. ! [... darn it! Needs refactoring, probably down to Model layer ... :( ]
                        idx.Delete();
                        continue;
                    }
                    foreach (WebPart.WebPartSetting idxI in idx.Settings)
                    {
                        Type moduleType = Adapter.ActiveModules.Find(
                            delegate(Type idxT)
                            {
                                return idxT.FullName == idx.Container.ModuleName;
                            });
                        if (moduleType.GetProperty(
                            idxI.Name.Replace(moduleType.FullName, ""),
                            BindingFlags.NonPublic |
                            BindingFlags.Public |
                            BindingFlags.Instance) != null)
                        {
                            node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name].Value = idxI.Value;
                            node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name]["ID"].Value = idxI.ID;
                            PropertyInfo prop = moduleType.GetProperty(
                                idxI.Name.Replace(moduleType.FullName, ""),
                                BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance);
                            if (prop != null)
                            {
                                ModuleSettingAttribute[] atrs =
                                    prop.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                                    as ModuleSettingAttribute[];
                                if (atrs != null && atrs.Length > 0)
                                {
                                    ModuleSettingAttribute atr = atrs[0];
                                    if (!string.IsNullOrEmpty(atr.ModuleEditorName))
                                    {
                                        node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name]["Editor"].Value = atr.ModuleEditorName;
                                    }
                                    else if (!string.IsNullOrEmpty(atr.ModuleEditorEventName))
                                    {
                                        node["ObjectTemplates"]["i-" + idx.ID]["i-" + idxI.Parent.Container.ID][idxI.Name]["EditorEvent"].Value = atr.ModuleEditorEventName;
                                    }
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }

        /**
         * Level2: Changes the given properties of the WebPage object. Legal values are Text, URL, PageTemplateID
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangePageProperty")]
        protected void Magix_Publishing_ChangePageProperty(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                if (e.Params.Contains("Text"))
                    o.Name = e.Params["Text"].Get<string>();

                if (e.Params.Contains("URL"))
                    o.ChangeURL(e.Params["URL"].Get<string>());

                if (e.Params.Contains("PageTemplateID"))
                {
                    o.Template = WebPageTemplate.SelectByID(e.Params["PageTemplateID"].Get<int>());
                }

                o.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");
            }
        }

        /**
         * Level2: Will create a new WebPage being the child of the given 'ID' WebPage
         */
        [ActiveEvent(Name = "Magix.Publishing.CreateChild")]
        protected void Magix_Publishing_CreateChild(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                WebPage n = new WebPage();
                n.Parent = o;
                o.Children.Add(n);

                o.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");

                RaiseEvent("Magix.Core.ExpandTreeSelectedID");
            }
        }

        /**
         * Level2: Will ask the end user if he wish to delete specific Page object and all of 
         * its children
         */
        [ActiveEvent(Name = "Magix.Publishing.DeletePageObject")]
        protected void Magix_Publishing_DeletePageObject(object sender, ActiveEventArgs e)
        {
            WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

            if (o.Parent == null)
            {
                throw new ArgumentException("You cannot delete the top most Page, only edit it ...");
            }

            int affectedPages = 1;
            affectedPages += CountChildPages(o);

            // Showing message box to user asking him if he's really sure he wish to delete
            // this page ...
            Node node = new Node();
            node["ForcedSize"]["width"].Value = 550;
            node["WindowCssClass"].Value =
                "mux-shaded mux-rounded push-5 down-2";

            node["Caption"].Value = @"Are you CERTAIN ...?";
            node["Text"].Value = string.Format(@"
<p>Are you sure you wish to delete this Page? This operation is ireversible, and will affect {0} pages</p>",
                affectedPages);
            node["OK"]["ID"].Value = o.ID;
            node["OK"]["Event"].Value = "Magix.Publishing.DeletePageObject-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /*
         * Helper for above ...
         */
        private int CountChildPages(WebPage o)
        {
            int retVal = o.Children.Count;

            foreach (WebPage idx in o.Children)
            {
                retVal += CountChildPages(idx);
            }

            return retVal;
        }

        /**
         * Level2: Will delete specific Page object ['ID'] and all of its children
         */
        [ActiveEvent(Name = "Magix.Publishing.DeletePageObject-Confirmed")]
        protected void Magix_Publishing_DeletePageObject_Confirmed(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage o = WebPage.SelectByID(e.Params["ID"].Get<int>());

                o.Delete();

                tr.Commit();

                ActiveEvents.Instance.RaiseClearControls("content4");

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                Node node = new Node();
                node["ID"].Value = o.ID;

                node = new Node();
                node["FullTypeName"].Value = typeof(WebPage).FullName + "-META";

                RaiseEvent(
                    "Magix.Core.UpdateTree",
                    node);

                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }

        /**
         * Level2: Will update and save the incoming WebPartID [WebPartSetting] and set its Value 
         * property to 'Value'
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangeWebPartSetting")]
        protected void Magix_Publishing_ChangeWebPartSetting(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPart.WebPartSetting set = WebPart.WebPartSetting.SelectByID(e.Params["WebPartID"].Get<int>());
                set.Value = e.Params["Value"].Get<string>();
                set.Save();

                tr.Commit();

                // Signalizing that Pages has been changed, in case other modules are
                // dependent upon knowing ...
                RaiseEvent("Magix.Publishing.PageWasUpdated");

                // In case ...
                ActiveEvents.Instance.RaiseClearControls("content5");
            }
        }

        /**
         * Level3: Will run through all WebParts which are 'touched' by this WebPartTemplate
         * and update their settings according to whatever new module was chosen.
         * Warning; This will set any 'value properties' in these WebParts to their
         * default value. It might also retrieve values it had before if this WebPart
         * has earlier been this type of WebPartTemplate
         */
        [ActiveEvent(Name = "Magix.Publishing.WebPartTemplateWasModified")]
        protected void Magix_Publishing_WebPartTemplateWasModified(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // We must 're-touch' all Pages effected by the change of our template
                // This to create default values for newly inserted type of
                // modules and such ...
                List<WebPage> pages = new List<WebPage>();
                foreach (WebPart idx in
                    WebPart.Select(
                        Criteria.ExistsIn(e.Params["ID"].Get<int>(), false)))
                {
                    if (!pages.Exists(
                        delegate(WebPage idx2)
                        {
                            return idx.WebPage == idx2;
                        }))
                        pages.Add(idx.WebPage);
                }
                foreach (WebPage idx in pages)
                {
                    idx.Save();
                }

                tr.Commit();
            }
        }

        /**
         * Level2: Will update all WebPages that was modified by changing the WebPageTemplate. This might
         * include creating new WebParts with default values, or removing existing WebParts on Pages
         */
        [ActiveEvent(Name = "Magix.Publishing.WebPageTemplateWasModified")]
        protected void Magix_Publishing_WebPageTemplateWasModified(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // We must 're-touch' all Pages effected by the change of our template
                // This to create default values for newly inserted type of
                // modules and such ...
                foreach (WebPage idx in 
                    WebPage.Select(
                        Criteria.ExistsIn(e.Params["ID"].Get<int>(), true)))
                {
                    idx.Save();
                }

                tr.Commit();
            }
        }
    }
}