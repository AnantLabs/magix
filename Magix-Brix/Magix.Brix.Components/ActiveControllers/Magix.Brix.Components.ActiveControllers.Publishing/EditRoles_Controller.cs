/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: Here mostly to serve up grids and such for editing of Roles in the system
     */
    [ActiveController]
    public class EditRoles_Controller : ActiveController
    {
        /**
         * Level2: Will allow the end-user to edit [add, change, delete] roles within the system
         */
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

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;

            node["Container"].Value = "content3";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }
    }
}
