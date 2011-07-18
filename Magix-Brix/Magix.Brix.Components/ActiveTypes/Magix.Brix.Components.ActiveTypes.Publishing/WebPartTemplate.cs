/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
     * of webpart, and name of module to inject.
     */
    [ActiveType]
    public class WebPartTemplate : ActiveType<WebPartTemplate>
    {
        /**
         * Name of template. Serves no logical purpose.
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Which container in our ViewPort are we supposed to get stuffed into ...
         */
        [ActiveField]
        public string ViewportContainer { get; set; }

        /**
         * Contains the fully qualified name of the module to instantiate.
         * Warning, if modifying this one 'by hand', bad things can happen ...!!!
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
         * Margin-Left in multiples of 18
         */
        [ActiveField]
        public int MarginLeft { get; set; }

        /**
         * Margin-Top in multiples of 18
         */
        [ActiveField]
        public int MarginTop { get; set; }

        /**
         * 'Padding' in multiples of 30/70/110 [+40] units up til 24
         */
        [ActiveField]
        public int MarginRight { get; set; }

        /**
         * Bottom-Top in multiples of 18
         */
        [ActiveField]
        public int MarginBottom { get; set; }

        /**
         * If true, will eliminate al margins to the right of the webpart
         */
        [ActiveField]
        public bool Last { get; set; }

        #region [ -- Business logic and overrides ... -- ]

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
            Width = Math.Max(Math.Min(Width, 24), 6);
            Height = Math.Max(Math.Min(Height, 30), 6);
            MarginLeft = Math.Max(Math.Min(MarginLeft, 23), 0);
            MarginRight = Math.Max(Math.Min(MarginRight, 23), 0);
            MarginBottom = Math.Max(Math.Min(MarginBottom, 30), 0);
            MarginTop = Math.Max(Math.Min(MarginTop, 30), 0);
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
