/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class RolesController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.EditRoles")]
        protected void Magix_Publishing_EditRoles(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Role).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "large-bottom-margin";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;

            node["NoIdColumn"].Value = true;

            node["ReuseNode"].Value = true;
            node["CreateEventName"].Value = "Magix.Publishing.CreateRole";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;

            node["Container"].Value = "content3";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.CreateRole")]
        protected void Magix_Publishing_CreateRole(object sender, ActiveEventArgs e)
        {
            Role r = new Role();
            r.Name = "Default name, change";
            r.Save();
        }
    }
}

























