/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.Brix.Publishing.Common
{
    /**
     * Level3: I'd be highly surprised if this is not your first entry to Magix in C#.
     * This is the PublisherPlugin attribute, which you can use to create your own
     * plugins for the Publishing system within. Implement this attribute on your
     * ActiveModules and VOILA! They'll surface up as selections in your 
     * WebPageTemplate editing operations and be usable as plugins in your system.
     * Probably the easiest way on the planet to create a plugin for any kind of system
     * out there. Especially in combination with the logic behind the ModuleSettingAttribute
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PublisherPluginAttribute : Attribute
    {
        /**
         * Level3: Whether or not the Module will work unless it's had its settings changed
         * If this one is false, then unless some of the module's settings have been set,
         * the module is to be considered in an 'indetermined state'. Meaning, might blow up.
         * And hence will not this module be chosen if something just needs a 'random module'.
         * Like for instance when a new WebPartTemplate is created for a WebPageTemplate and
         * it needs a 'default module type'
         */
        public bool CanBeEmpty = false;
    }
}
