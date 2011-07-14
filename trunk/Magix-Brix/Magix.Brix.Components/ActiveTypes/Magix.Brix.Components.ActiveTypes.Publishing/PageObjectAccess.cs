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
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    [ActiveType]
    public class PageObjectAccess : ActiveType<PageObjectAccess>
    {
        [ActiveField(IsOwner = false)]
        public Role Role { get; set; }

        [ActiveField(IsOwner = false)]
        public PageObject Page { get; set; }
    }
}
