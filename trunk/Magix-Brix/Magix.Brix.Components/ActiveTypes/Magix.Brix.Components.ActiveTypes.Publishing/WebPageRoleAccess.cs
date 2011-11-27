/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
     * Gives access to a page according to a specific role. This is the core class in Magix in regards
     * to authorizing access to specific pages. Access in Magix is associated with a User, his Role
     * and the WebPage the user tries to access. You cannot authorize on a more fine-grained level out
     * than WebPages, out of the box. This class encapsulates the authorization logic in Magix. By default
     * everyone have access to everything. If one Role is explicitly granted access, then all other 
     * roles will be denied access, unless they too are granted access. In addition access is 'inherited'
     * from the 'Mother Page'. Meaning a child page will be default have the same access rights as its 
     * Mother page, unless the child page itself starts granting and denying access to specific pages
     */
    [ActiveType]
    public class WebPageRoleAccess : ActiveTypeCached<WebPageRoleAccess>
    {
        /**
         * Role to grant users belonging to that role access to the Page in this object. All other roles, 
         * which are not having their own access objects towards the page, will now no longer have 
         * access to this page
         */
        [ActiveField(IsOwner = false)]
        public Role Role { get; set; }

        /**
         * Page to grant users belonging to role access to. All other roles, which are not having
         * their own access objects towards the page, will no longer have access to this page
         */
        [ActiveField(IsOwner = false)]
        public WebPage Page { get; set; }
    }
}
