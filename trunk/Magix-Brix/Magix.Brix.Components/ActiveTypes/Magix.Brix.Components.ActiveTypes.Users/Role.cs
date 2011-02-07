/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web;
using Magix.Brix.Data;
using Magix.Brix.Types;

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
            if (other != this)
            {
                throw new ApplicationException(
                    @"The name of that Role is already taken by another object in the system 
and would create very hard to track-down bugs if allowed to be created ...");
            }
            base.Save();
        }
    }
}
