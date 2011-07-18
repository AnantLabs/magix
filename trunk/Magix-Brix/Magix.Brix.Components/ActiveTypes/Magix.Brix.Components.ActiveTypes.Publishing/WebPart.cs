/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    /**
     * Is once instance of a container on a WebPage. Baically one 'module' if you wish.
     */
    [ActiveType]
    public class WebPart : ActiveType<WebPart>
    {
        /**
         * Serves as a container for settings for each webpart.
         * Every webpart can define its own settings through using
         * the ModuleSettingAttribute, which will turn into one instance
         * of this call, belonging to the WebPart.
         */
        [ActiveType]
        public class WebPartSetting : ActiveType<WebPartSetting>
        {
            /**
             * This is the name of your property in your Module
             */
            [ActiveField]
            public string Name { get; set; }

            /**
             * This is the value you'd like to initialize it with
             */
            [ActiveField]
            public string Value { get; set; }

            /**
             * Parent WebPart ...
             */
            [ActiveField(BelongsTo = true)]
            public WebPart Parent { get; set; }
        }

        public WebPart()
        {
            Settings = new LazyList<WebPartSetting>();
        }

        /**
         * Which template [recipe] the WebPart is built upon
         */
        [ActiveField(IsOwner = false)]
        public WebPartTemplate Container { get; set; }

        /**
         * Which web page the WebPart belongs to
         */
        [ActiveField(BelongsTo = true)]
        public WebPage WebPage { get; set; }

        /**
         * Settings for this WebPart
         */
        [ActiveField]
        public LazyList<WebPartSetting> Settings { get; set; }
    }
}
