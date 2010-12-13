/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Framework;
using Magic.Brix.Data;
using Magic.Brix.Types;
using Magic.Brix.Data.Internal;
using Magic.Brix.Data.Adapters;

namespace Magic.Brix.Tests.Data
{
    [TestFixture]
    public class ParentChild : BaseTest
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

            [ActiveField]
            public LazyList<Role> Roles { get; set; }
        }

        [ActiveType]
        internal class Role : ActiveType<Role>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [Test]
        public void CreateParentChildEditChildSaveParent()
        {
            SetUp();
            User u = new User();
            u.Username = "uname";
            u.Password = "pwd";
            Role r1 = new Role();
            r1.Name = "admin";
            u.Roles.Add(r1);
            u.Save();

            User u2 = User.SelectByID(u.ID);
            Assert.IsNotNull(u2);
            Assert.IsNotNull(u2.Roles);
            Assert.AreEqual(1, u2.Roles.Count);
            Assert.AreEqual("admin", u2.Roles[0].Name);

            u2.Roles[0].Name = "admin2";
            u2.Roles[0].Save();

            User u3 = User.SelectByID(u.ID);
            Assert.IsNotNull(u3);
            Assert.IsNotNull(u3.Roles);
            Assert.AreEqual(1, u3.Roles.Count);
            Assert.AreEqual("admin2", u3.Roles[0].Name);
        }
    }
}
