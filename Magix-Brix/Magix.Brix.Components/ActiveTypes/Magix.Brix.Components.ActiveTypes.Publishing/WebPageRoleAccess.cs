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
    /**
     * Gives access to a page according to a specific role. 
     */
    [ActiveType]
    public class WebPageRoleAccess : ActiveType<WebPageRoleAccess>
    {
        /**
         * Role to grant access to
         */
        [ActiveField(IsOwner = false)]
        public Role Role { get; set; }

        /**
         * Page to grant role access to ...
         */
        [ActiveField(IsOwner = false)]
        public WebPage Page { get; set; }
    }
}
