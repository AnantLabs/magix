/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.Users
{
    [ActiveType(TableName = "docWineTasting.CoreTypes.Role")]
    public class Role : ActiveType<Role>
    {
        [ActiveField]
        public string Name { get; set; }

        public override void Save()
        {
            Role other = Role.SelectFirst(Criteria.Eq("Name", Name));
            if (other != null && other != this)
            {
                throw new ApplicationException(
                    @"The name of that Role is already taken by another object in the system 
and would create very hard to track-down bugs if allowed to be created ...");
            }
            if (ID == 0)
            {
                Node node = new Node();

                node["LogItemType"].Value = "Magix.Core.RoleCreated";
                node["Header"].Value = "Name: " + Name;
                node["ObjectID"].Value = "-1";

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.Log",
                    node);
            }
            base.Save();
        }
    }
}
