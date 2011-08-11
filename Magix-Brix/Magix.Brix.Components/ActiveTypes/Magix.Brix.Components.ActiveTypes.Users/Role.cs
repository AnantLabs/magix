/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
            if (ID == 0)
            {
                Node node = new Node();

                node["LogItemType"].Value = "Magix.Core.RoleCreated";
                node["Header"].Value = "Name: " + Name;
                node["ObjectID"].Value = -1;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.Log",
                    node);
            }
            if (string.IsNullOrEmpty(Name))
                Name = "Default Name";

            string name = Name;

            int idxNo = 2;
            while (CountWhere(
                Criteria.Eq("Name", name),
                Criteria.NotId(ID)) > 0)
            {
                name = Name + "-" + idxNo.ToString();
                idxNo += 1;
            }
            Name = name;
            base.Save();
        }
    }
}
