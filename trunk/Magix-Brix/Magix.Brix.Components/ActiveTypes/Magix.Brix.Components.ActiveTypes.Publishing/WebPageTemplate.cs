/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Data;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    /**
     * Serves as a 'recipe' for a WebPage. Every WebPage is built from one of these.
     * Contains the definition of which module to instantiate for instance, but not which
     * parameters to send into it. Etc ...
     */
    [ActiveType]
    public class WebPageTemplate : ActiveType<WebPageTemplate>
    {
        public WebPageTemplate()
        {
            Containers = new LazyList<WebPartTemplate>();
        }

        /**
         * Friendly name of template. Serves no logical meaning ...
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Our 'WebPart-recipes'. Contains the recipe for every webpart that
         * should be added to our page.
         */
        [ActiveField]
        public LazyList<WebPartTemplate> Containers { get; set; }
    }
}
