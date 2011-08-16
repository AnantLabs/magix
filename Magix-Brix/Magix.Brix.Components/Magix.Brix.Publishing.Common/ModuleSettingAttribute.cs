/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.Brix.Publishing.Common
{
    /**
     * Level3: Wraps a setting property for a PublisherPlugin
     */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ModuleSettingAttribute : Attribute
    {
        /**
         * Level3: What is the name of the Module Editor which we'll use
         * to edit this specifi value? PS! It must conform to the RichEditor
         * in the way it sends around its nodes and such if you intend to create your
         * own. Meaning, if you're embarking on creating your own, UNDERSTAND the RichEditor
         * way of handling nodes back and forth FIRST and avoid a lot of PAIN ...!
         */
        public string ModuleEditorName = "";

        /**
         * Level3: If you want to go 'completely x86 CISC mode' and take control over the 
         * actual creation yourself entirely
         */
        public string ModuleEditorEventName = "";

        /**
         * Level3: The default value of your module setting
         */
        public string DefaultValue = "Default";
    }
}
