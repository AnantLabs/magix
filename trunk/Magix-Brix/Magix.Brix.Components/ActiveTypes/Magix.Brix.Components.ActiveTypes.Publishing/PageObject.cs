/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Reflection;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    [ActiveType]
    public class PageObject : ActiveType<PageObject>
    {
        public PageObject ()
	    {
            ObjectTemplates = new LazyList<PageObjectTemplate>();
            Children = new LazyList<PageObject>();
	    }

        [ActiveField(IsOwner = false)]
        public PageTemplate Template { get; set; }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string URL { get; private set; }

        [ActiveField]
        public LazyList<PageObjectTemplate> ObjectTemplates { get; set; }

        [ActiveField]
        public LazyList<PageObject> Children { get; set; }

        [ActiveField(BelongsTo = true)]
        public PageObject Parent { get; set; }

        public override void Save()
        {
            if (string.IsNullOrEmpty(Name))
                Name = "Default name, please change";
            FixLocalURL();
            if (Template == null)
                Template = PageTemplate.SelectFirst();
            if (Template == null)
                throw new ArgumentException("You cannot create PageObjects before you've created at least ONE PageTemplate ...");

            ObjectTemplates.RemoveAll(
                delegate(PageObjectTemplate idx)
                {
                    return !Template.Containers.Exists(
                        delegate(PageTemplateContainer idxI)
                        {
                            return idxI == idx.Container;
                        });
                });
            foreach (PageTemplateContainer idx in Template.Containers)
            {
                if (!ObjectTemplates.Exists(
                    delegate(PageObjectTemplate idxI)
                    {
                        return idxI.Container == idx;
                    }))
                {
                    PageObjectTemplate nTpl = new PageObjectTemplate();
                    ObjectTemplates.Add(nTpl);
                    nTpl.Container = idx;
                    Type moduleType = Adapter.ActiveModules.Find(
                        delegate(Type idxType)
                        {
                            return idxType.FullName == idx.ModuleName;
                        });
                    foreach (PropertyInfo idxProp in
                        moduleType.GetProperties(
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic))
                    {
                        ModuleSettingAttribute[] atrs =
                            idxProp.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                                as ModuleSettingAttribute[];
                        if (atrs != null && atrs.Length > 0)
                        {
                            ModuleSettingAttribute atr = atrs[0];
                            PageObjectTemplate.PageObjectTemplateSetting set = nTpl.Settings.Find(
                                delegate(PageObjectTemplate.PageObjectTemplateSetting idxSet)
                                {
                                    return idxSet.Name == moduleType.FullName + idxProp.Name;
                                });
                            if (set == null)
                            {
                                set = new PageObjectTemplate.PageObjectTemplateSetting();
                                set.Name = moduleType.FullName + idxProp.Name;
                                nTpl.Settings.Add(set);
                                set.Parent = nTpl;
                            }
                            set.Value = "default value, please change ...";
                            idx.Save();
                        }
                    }
                }
                else
                {
                    PageObjectTemplate tpl = ObjectTemplates.Find(
                        delegate(PageObjectTemplate idxI)
                        {
                            return idxI.Container == idx;
                        });
                    Type moduleType = Adapter.ActiveModules.Find(
                        delegate(Type idxType)
                        {
                            return idxType.FullName == idx.ModuleName;
                        });
                    foreach (PropertyInfo idxProp in
                        moduleType.GetProperties(
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic))
                    {
                        ModuleSettingAttribute[] atrs =
                            idxProp.GetCustomAttributes(typeof(ModuleSettingAttribute), true)
                                as ModuleSettingAttribute[];
                        if (atrs != null && atrs.Length > 0)
                        {
                            ModuleSettingAttribute atr = atrs[0];
                            PageObjectTemplate.PageObjectTemplateSetting set = tpl.Settings.Find(
                                delegate(PageObjectTemplate.PageObjectTemplateSetting idxSet)
                                {
                                    return idxSet.Name == moduleType.FullName + idxProp.Name;
                                });
                            if (set == null)
                            {
                                set = new PageObjectTemplate.PageObjectTemplateSetting();
                                set.Name = moduleType.FullName + idxProp.Name;
                                tpl.Settings.Add(set);
                                set.Parent = tpl;
                                set.Value = "default value, please change ...";
                                tpl.Save();
                            }
                        }
                    }
                    tpl.Save();
                }
            }
            base.Save();
        }

        private void FixLocalURL()
        {
            string url = URL;

            if (string.IsNullOrEmpty(url))
            {
                if (Parent != null)
                    url = Parent.URL + "/" + Name.ToLowerInvariant();
                else
                    url = "";
            }
            while (true)
            {
                bool found = false;
                foreach (char idx in url)
                {
                    if (("abcdefghijklmnopqrstuvwxyz1234567890-_/").IndexOf(idx) == -1)
                    {
                        found = true;
                        url = url.Replace(idx, '-');
                        break;
                    }
                }
                if (!found)
                    break;
            }
            while (true)
            {
                if (!url.Contains("--"))
                    break;
                url = url.Replace("--", "-");
            }
            url = url.Trim('-');
            foreach (PageObject idx in PageObject.Select(Criteria.Eq("URL", url)))
            {
                if (idx != this)
                {
                    if (string.IsNullOrEmpty(url))
                        throw new ArgumentException("Cannot have multiple top-level Pages ... ");
                    for (int idxI = 2; idxI < 1000000; idxI++)
                    {
                        if (!idx.URL.Contains("-" + idxI) ||
                            idx.URL.IndexOf("-" + idxI) != idx.URL.Length - ("-" + idxI).Length)
                        {
                            string tmpUrl = url + "-" + idxI;

                            if (PageObject.CountWhere(Criteria.Eq("URL", tmpUrl)) == 0)
                            {
                                url = tmpUrl;
                                break;
                            }
                        }
                    }
                }
            }
            URL = url;
        }

        public void SetURL(string p)
        {
            if (Parent != null)
            {
                if (p.Contains("/"))
                    throw new ArgumentException("You cannot have '/' characters in your URL. PageObject was NOT saved ...! ");
                URL = Parent.URL + "/" + (string.IsNullOrEmpty(p) ? Name : p).ToLowerInvariant();
                FixLocalURL();
                foreach (PageObject idx in Children)
                {
                    idx.FixURL();
                }
            }
            else
                throw new ArgumentException("Cannot change URL of Top Level Page object ... ");
        }

        private void FixURL()
        {
            if (Parent != null)
            {
                string tmpUrl = Parent.URL + "/";
                if (!string.IsNullOrEmpty(URL))
                {
                    string lastParts = URL.Substring(URL.LastIndexOf("/") + 1);
                    tmpUrl += lastParts;
                }
                else
                {
                    tmpUrl += Name;
                }
                URL = tmpUrl;
                FixLocalURL();
                foreach (PageObject idx in Children)
                {
                    idx.FixURL();
                }
            }
        }
    }
}
