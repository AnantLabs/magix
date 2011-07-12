/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    [ActiveType]
    public class PageTemplateContainer : ActiveType<PageTemplateContainer>
    {
        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string ViewportContainer { get; set; }

        [ActiveField]
        public int Width { get; set; }

        [ActiveField]
        public int Padding { get; set; }

        [ActiveField]
        public int Push { get; set; }

        [ActiveField]
        public int Top { get; set; }

        [ActiveField]
        public int BottomMargin { get; set; }

        [ActiveField]
        public int Height { get; set; }

        [ActiveField]
        public bool Last { get; set; }

        [ActiveField]
        public string ModuleName { get; set; }

        [ActiveField]
        public string CssClass { get; set; }
    }
}
