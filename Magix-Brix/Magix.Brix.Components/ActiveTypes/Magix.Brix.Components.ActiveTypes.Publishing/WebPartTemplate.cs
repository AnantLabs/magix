/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    /**
     * Serves as a 'recipe' for WebParts. Contains stuff such as positioning
     * of webpart, and name of module to inject. Every WebPageTemplate has a list of
     * these guys, which serves as recipe for how the WebPage's WebParts should be
     * built, and which Plugins to load up
     */
    [ActiveType]
    public class WebPartTemplate : ActiveType<WebPartTemplate>
    {
        /**
         * Name of template. Serves no logical purpose. But please 
         * make meaningful names
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Which container in our ViewPort are we supposed to get stuffed into. Normally
         * this value is being 'normalized' though, meaning the WebParts are stuffed out
         * in chronological order, according to how they appear in the collection, up until
         * we've either stuffed them all out, or ran out of Viewport Containers
         */
        [ActiveField]
        public string ViewportContainer { get; set; }

        /**
         * Contains the fully qualified name of the module to instantiate.
         * Warning, if modifying this one 'by hand', bad things can happen!
         * This shoulc contain the full name of the Module's C# Class, e.g.;
         * 'MyCompany.MyComponent.MyModule'. These modules must be attributed 
         * as PublisherPlugins
         */
        [ActiveField]
        public string ModuleName { get; set; }

        /**
         * Additional CSS class(es) to be injected into the webpart
         */
        [ActiveField]
        public string CssClass { get; set; }

        /**
         * Which page template our webpart template belongs to
         */
        [ActiveField(BelongsTo = true)]
        public WebPageTemplate PageTemplate { get; set; }

        /**
         * Width in multiples of 30/70/110 [+40] units up til 24
         */
        [ActiveField]
        public int Width { get; set; }

        /**
         * Height in multiples of 18
         */
        [ActiveField]
        public int Height { get; set; }

        /**
         * Margin-Left in multiples of 18. Can go negative to -23
         */
        [ActiveField]
        public int MarginLeft { get; set; }

        /**
         * Margin-Top in multiples of 18. Can go negative to -30
         */
        [ActiveField]
        public int MarginTop { get; set; }

        /**
         * 'Padding' in multiples of 30/70/110 [+40] units up til 24. Can go negative to -23
         */
        [ActiveField]
        public int MarginRight { get; set; }

        /**
         * Bottom-Top in multiples of 18. Can go negative to -30
         */
        [ActiveField]
        public int MarginBottom { get; set; }

        /**
         * If true, will eliminate all margins [10px spacing] to the right of the webpart
         */
        [ActiveField]
        public bool Last { get; set; }

        /**
         * If true, will allow the element to overflow in the vertical direction. Meaning
         * it will 'ignore' its height, and just 'flow as high as it is'
         */
        [ActiveField]
        public bool Overflow { get; set; }

        #region [ -- Business logic and overrides ... -- ]

        public override void Delete()
        {
            WebPageTemplate pageTempl = this.PageTemplate;
            pageTempl.Containers.Remove(this);

            pageTempl.Save();

            // Need to delete all WebParts built upon this WebPartTemplate ...
            foreach (WebPart idx in WebPart.Select(
                Criteria.ExistsIn(this.ID, false)))
            {
                idx.Delete();
            }
            base.Delete();
        }

        public override void Save()
        {
            KeepAllValuesWithinLegality();
            base.Save();
        }

        private void KeepAllValuesWithinLegality()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = "default WebPartTemplate Name";
            }
            Width = Math.Max(Math.Min(Width, 24), 2);
            Height = Math.Max(Math.Min(Height, 30), 2);
            MarginLeft = Math.Max(Math.Min(MarginLeft, 23), -23);
            MarginRight = Math.Max(Math.Min(MarginRight, 23), -23);
            MarginBottom = Math.Max(Math.Min(MarginBottom, 30), -30);
            MarginTop = Math.Max(Math.Min(MarginTop, 30), -30);
        }

        internal string GetDefaultValueForSetting(string settingName)
        {
            Type moduleType = Adapter.ActiveModules.Find(
                delegate(Type idx)
                {
                    return idx.FullName == ModuleName;
                });

            // Doing a little bit of validation, and humor here ... ;)
            if (moduleType == null)
                throw new ArgumentException(
                    string.Format(@"(2): Some genius probably have removed the DLL of the 
plugin '{0}' from your bin folder, without realizing it was in use for the 
WebPageTemplate '{1}', and didn't care to edit the Template, could it have been you ...? ;)",
                        ModuleName,
                        Name));

            PropertyInfo prop = moduleType.GetProperty(settingName);
            ModuleSettingAttribute[] atrs = 
                prop.GetCustomAttributes(
                    typeof(ModuleSettingAttribute), 
                    true) as ModuleSettingAttribute[];
            return atrs[0].DefaultValue;
        }

        #endregion
    }
}
