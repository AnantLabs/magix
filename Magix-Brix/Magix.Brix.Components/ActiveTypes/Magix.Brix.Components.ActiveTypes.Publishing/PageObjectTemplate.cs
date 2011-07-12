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
    [ActiveType]
    public class PageObjectTemplate : ActiveType<PageObjectTemplate>
    {
        [ActiveType]
        public class PageObjectTemplateSetting : ActiveType<PageObjectTemplateSetting>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Value { get; set; }

            [ActiveField(BelongsTo = true)]
            public PageObjectTemplate Parent { get; set; }
        }

        public PageObjectTemplate()
        {
            Settings = new LazyList<PageObjectTemplateSetting>();
        }

        [ActiveField(IsOwner = false)]
        public PageTemplateContainer Container { get; set; }

        [ActiveField]
        public LazyList<PageObjectTemplateSetting> Settings { get; set; }
    }
}
