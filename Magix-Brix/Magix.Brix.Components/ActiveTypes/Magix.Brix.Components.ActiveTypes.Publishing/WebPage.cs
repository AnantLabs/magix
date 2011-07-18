﻿/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using Magix.Brix.Loader;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    /**
     * Represents one web page in our system, or one unique URL if you wish.
     * Contains a list of WebParts which again are the areas of the screen within your
     * web page.
     */
    [ActiveType]
    public class WebPage : ActiveType<WebPage>
    {
        public WebPage ()
	    {
            WebParts = new LazyList<WebPart>();
            Children = new LazyList<WebPage>();
	    }

        /**
         * Name of your Web Page. Serves nothing but as a friendly name
         * and have no real meaning in the system. Doesn't need to be in any
         * ways unique ...
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Needs to be unique system wide, but will do its best
         * at staying unique. Also serves as a Materialized Path
         * for our Page Hierarchy ...
         */
        [ActiveField]
        public string URL { get; private set; }

        /**
         * The Template the Page is built upon
         */
        [ActiveField(IsOwner = false)]
        public WebPageTemplate Template { get; set; }

        /**
         * The parts, or containers of the Web Page ...
         */
        [ActiveField]
        public LazyList<WebPart> WebParts { get; set; }

        /**
         * Children Page objects ...
         */
        [ActiveField]
        public LazyList<WebPage> Children { get; set; }

        /**
         * Parent web page ...
         * Is null if this is the top most page ...
         */
        [ActiveField(BelongsTo = true)]
        public WebPage Parent { get; set; }

        #region [ -- Busiess logic and overrides ... -- ]

        public void ChangeURL(string newUrl)
        {
            if (CountWhere(Criteria.Like("URL", URL + "/%")) > 20)
                throw new ArgumentException("You cannot change the URL of your pages once they've acquired more than 20 descendants in total. Note, if you want to force this through anyway, you can edit the Page through the Database Administrator ...");
            if (Parent != null)
            {
                if (newUrl.Contains("/"))
                {
                    // Ignoring everything before the last '/' ...
                    newUrl = newUrl.Substring(newUrl.LastIndexOf('/') + 1);
                }

                URL = Parent.URL + "/" + (string.IsNullOrEmpty(newUrl) ? Name : newUrl).ToLowerInvariant();

                FixLocalURL();

                // Cleaning up children ...
                // PS!
                // This makes changing the URL of a top level document, with lots of children, VERY 
                // timeconsuming ...
                // TODO: Optimize ...?
                foreach (WebPage idx in Children)
                {
                    idx.RetouchURLParentWasChanged();
                }
            }
            else
                throw new ArgumentException("Cannot change URL of Top Level Page object ... ");
        }

        public override void Save()
        {
            // Defaulting Name of Page to something ...
            if (string.IsNullOrEmpty(Name))
                Name = "Newly Created";

            // Can't have more than one top level page ...
            CheckToSeIfMoreThanOneTopLevelPageIsCreated();

            // Makes sure URL is Unique within the level
            FixLocalURL();

            // Choosing first random template if none are given
            // NEVER allowing a Page to exist without it having a template
            if (Template == null)
                Template = WebPageTemplate.SelectFirst();

            // Well, the text is explaining it pretty OK I think ... ... ;)
            if (Template == null)
                throw new ArgumentException("You cannot create WebPages before you've created at least one WebPageTemplate ...");

            // Making sure that our current WebParts are correct 
            // according to our current WebPageTemplate and WebPartTemplates ...
            MakeSureWebPartsAreCorrectAccordingToTemplates();

            base.Save();
        }

        private void CheckToSeIfMoreThanOneTopLevelPageIsCreated()
        {
            if (Parent == null)
            {
                // Making sure this is our Only top-level WebPage
                foreach (WebPage idx in WebPage.Select())
                {
                    if (idx.Parent == null && idx != this)
                        throw new ArgumentException("You cannot have more than one WebPage being your root page");
                }
            }
        }

        private void FixLocalURL()
        {
            // ORDER COUNT ...!
            SetDefaultURLIfEmpty();
            ReplaceAllNonURLCharactersWithHyphens();
            MakeSureURLIsUnique();
        }

        private void MakeSureURLIsUnique()
        {
            string url = URL;

            int idxNo = 2;
            while (CountWhere(
                Criteria.Eq("URL", url),
                Criteria.NotId(ID)) > 0)
            {
                url = URL + "-" + idxNo.ToString();
                idxNo += 1;
            }
            URL = url;
        }

        private void ReplaceAllNonURLCharactersWithHyphens()
        {
            while (true)
            {
                bool found = false;
                foreach (char idx in URL)
                {
                    if (("abcdefghijklmnopqrstuvwxyz1234567890-_/").IndexOf(idx) == -1)
                    {
                        found = true;
                        URL = URL.Replace(idx, '-');
                        break; // We cannot keep on enumerating over URL here ...
                    }
                }
                if (!found)
                    break;
            }

            // Making sure there are no occurencies of "--" in our URL ...
            while (true)
            {
                if (!URL.Contains("--"))
                    break; // No more things to replace ...

                URL = URL.Replace("--", "-");
            }

            // Making sure no "-" exists at the end or beginning of URL
            URL = URL.Trim('-');
        }

        private void SetDefaultURLIfEmpty()
        {
            // Defaulting URL to Name if URL is null or empty
            if (string.IsNullOrEmpty(URL))
            {
                if (Parent != null)
                    URL = Parent.URL + "/" + Name.ToLowerInvariant();
                else
                    URL = "";
            }
        }

        private void MakeSureWebPartsAreCorrectAccordingToTemplates()
        {
            RemoveAllWebPartsNotExistingInTemplates();
            CreateDefaultEmptyTemplatesForNewTemplates();
        }

        private IEnumerable<WebPartTemplate> InTemplateButNotInWebParts
        {
            get
            {
                foreach (WebPartTemplate idx in Template.Containers)
                {
                    if (!WebParts.Exists(
                        delegate(WebPart wp)
                        {
                            return wp.Container == idx;
                        }))
                    {
                        yield return idx;
                    }
                }
            }
        }

        private void CreateDefaultEmptyTemplatesForNewTemplates()
        {
            foreach (WebPartTemplate idx in InTemplateButNotInWebParts)
            {
                CreateDefaultEmptyWebPartForTemplate(idx);
            }
        }

        private void CreateDefaultEmptyWebPartForTemplate(WebPartTemplate template)
        {
            WebPart webPart = new WebPart();
            webPart.Container = template;
            InitializeWebPart(webPart);
            WebParts.Add(webPart);
        }

        private Type FindModuleName(WebPartTemplate template)
        {
            Type moduleType = Adapter.ActiveModules.Find(
                delegate(Type idxType)
                {
                    return idxType.FullName == template.ModuleName;
                });

            // Doing a little bit of validation, and humor here ... ;)
            if (moduleType == null)
                throw new ArgumentException(
                    string.Format(@"Some genius probably have removed the DLL of the 
plugin '{0}' from your bin folder, without realizing it was in use for the 
WebPageTemplate '{1}', and didn't care to edit the Template, could it have been you ...? ;)",
                        template.ModuleName,
                        template.Name));

            return moduleType;
        }

        private IEnumerable<Tuple<PropertyInfo, ModuleSettingAttribute>> 
            FindSettingsForModuleType(Type moduleType)
        {
            foreach (PropertyInfo idx in
                moduleType.GetProperties(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic))
            {
                ModuleSettingAttribute[] atrs =
                    idx.GetCustomAttributes(
                        typeof(ModuleSettingAttribute), true)
                        as ModuleSettingAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    yield return 
                        new Tuple<PropertyInfo, ModuleSettingAttribute>(
                            idx, 
                            atrs[0]);
                }
            }
        }

        private void InitializeWebPart(WebPart webPart)
        {
            Type moduleType = FindModuleName(webPart.Container);

            foreach (var idx in FindSettingsForModuleType(moduleType))
            {
                WebPart.WebPartSetting set = webPart.Settings.Find(
                    delegate(WebPart.WebPartSetting idxSet)
                    {
                        return idxSet.Name == moduleType.FullName + idx.Left.Name;
                    });
                if (set == null)
                {
                    set = new WebPart.WebPartSetting();
                    set.Name = moduleType.FullName + idx.Left.Name;
                    webPart.Settings.Add(set);
                    set.Parent = webPart; // To not mess up our Cache ...
                }
                set.Value = webPart.Container.GetDefaultValueForSetting(idx.Left.Name);
            }
        }

        private void RemoveAllWebPartsNotExistingInTemplates()
        {
            WebParts.RemoveAll(
                delegate(WebPart idx)
                {
                    return !Template.Containers.Exists(
                        delegate(WebPartTemplate idxI)
                        {
                            return idxI == idx.Container;
                        });
                });
        }

        public override void Delete()
        {
            Node node = new Node();
            node["ID"].Value = ID;

            // Someone might be interested in knowing about this ...?
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Publishing.PageObjectDeleted",
                node);

            base.Delete();
        }

        private void RetouchURLParentWasChanged()
        {
            string tmpUrl = Parent.URL + "/";
            if (!string.IsNullOrEmpty(URL))
            {
                string lastParts = URL;
                if (lastParts.IndexOf('/') != null)
                    lastParts = URL.Substring(URL.LastIndexOf("/") + 1);
                tmpUrl += lastParts;
            }
            else
            {
                tmpUrl += Name;
            }
            URL = tmpUrl;

            // Making sure it's unique and such ...
            FixLocalURL();

            foreach (WebPage idx in Children)
            {
                idx.RetouchURLParentWasChanged();
            }
        }

        #endregion
    }
}
