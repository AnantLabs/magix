/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Framework;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Data.Internal;
using Magix.Brix.Data.Adapters;

namespace Magix.Brix.Tests.Data
{
    [TestFixture]
    public class BelongsToTest : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            public User()
            {
                Roles = new LazyList<Role>();
            }

            [ActiveField]
            public string Username { get; set; }

            [ActiveField]
            public string Password { get; set; }

            [ActiveField(IsOwner=false)]
            public LazyList<Role> Roles { get; set; }
        }

        [ActiveType]
        internal class Role : ActiveType<Role>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField(BelongsTo = true, RelationName = "Roles")]
            public LazyList<User> Users { get; set; }
        }

        [Test]
        public void TestEntityTypeSingleSave()
        {
            SetUp();

            Role r1 = new Role();
            r1.Name = "Admin";
            r1.Save();

            Role r2 = new Role();
            r2.Name = "Normal";
            r2.Save();

            User user = new User();
            user.Username = "username1";
            user.Roles.Add(r1);
            user.Save();

            User user1 = User.SelectByID(user.ID);
            Assert.AreEqual(1, user1.Roles.Count);

            user1.Roles[0].Name = "something new";

            Role role1 = Role.SelectByID(r1.ID);
            Assert.AreEqual("Admin", role1.Name);

            user1.Roles.RemoveAt(0);
            user1.Save();

            user1 = User.SelectByID(user.ID);
            Assert.AreEqual(0, user1.Roles.Count);
            user1.Roles.Add(r1);
            user1.Save();

            Assert.AreEqual(2, Role.Count);

            User uu1 = User.SelectFirst(Criteria.ExistsIn(r1.ID, true));
            Assert.IsNotNull(uu1);

            User uu2 = User.SelectFirst(Criteria.ExistsIn(r1.ID, false));
            Assert.IsNull(uu2);

            Role rr1 = Role.SelectFirst(Criteria.ExistsIn(uu1.ID, false));
            Assert.IsNotNull(rr1);

            Role rr2 = Role.SelectFirst(Criteria.ExistsIn(uu1.ID, true));
            Assert.IsNull(rr2);
        }
    }
}
