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
    [ActiveType]
    public class PageTemplate : ActiveType<PageTemplate>
    {
        public PageTemplate()
        {
            Containers = new LazyList<PageTemplateContainer>();
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public LazyList<PageTemplateContainer> Containers { get; set; }
    }
}
