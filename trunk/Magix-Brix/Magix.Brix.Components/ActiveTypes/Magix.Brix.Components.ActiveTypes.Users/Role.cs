/*
 * Magix - A Web Application Framework for Humans
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
    // TODO: Why does this have that namespace ...
    /**
     * Level3: Contains the roles i the system. Every user belongs to a 'role' which gives him
     * rights in regards to some aspect of functionality or something. All autorization,
     * by default in Magix, will go through this Role class, and which specific roles
     * the currently logged in user belongs to
     */
    [ActiveType(TableName = "docWineTasting.CoreTypes.Role")]
    public class Role : ActiveType<Role>
    {
        /**
         * Level3: The name of the role, typically 'Administrator' or 'Guest' or something
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Level3: Overridden to make sure Name is unique among other things
         */
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
