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
using Magix.Brix.Data.Internal;
using Magix.Brix.Data.Adapters;

namespace Magix.Brix.Tests.Data
{
    [TestFixture]
    public class MultipleSingleChildrenOfSameTypeNoOwner : BaseTest
    {
        [ActiveType]
        public class Role : ActiveType<Role>
        {
            [ActiveField]
            public int Value { get; set; }
        }

        [ActiveType]
        public class User : ActiveType<User>
        {
            [ActiveField]
            public string Name{ get; set; }

            [ActiveField(IsOwner = false)]
            public Role Role1 { get; set; }

            [ActiveField(IsOwner = false)]
            public Role Role2 { get; set; }
        }

        [Test]
        public void SaveWithTwoDifferentValue()
        {
            Role r1 = new Role();
            r1.Value = 1;
            r1.Save();

            Role r2 = new Role();
            r2.Value = 2;
            r2.Save();

            User u1 = new User();
            u1.Name = "Thomas";
            u1.Role1 = r1;
            u1.Role2 = r2;
            u1.Save();

            User u2 = new User();
            u2.Name = "Kariem";
            u2.Role1 = r2;
            u2.Role2 = r1;
            u2.Save();

            User u1_after = User.SelectByID(u1.ID);
            Assert.AreEqual(1, u1_after.Role1.Value);
            Assert.AreEqual(2, u1_after.Role2.Value);

            User u2_after = User.SelectByID(u2.ID);
            Assert.AreEqual(2, u2_after.Role1.Value);
            Assert.AreEqual(1, u2_after.Role2.Value);
        }
    }
}
