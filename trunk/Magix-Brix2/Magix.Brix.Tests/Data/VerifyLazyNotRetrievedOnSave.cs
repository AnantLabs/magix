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
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Data.Internal;
using Magix.Brix.Data.Adapters;

namespace Magix.Brix.Tests.Data
{
    [TestFixture]
    public class VerifyLazyNotRetrievedOnSave : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            public User()
            {
                Roles = new LazyList<Role>();
            }

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
            Role r1 = new Role();
            r1.Name = "admin";
            u.Roles.Add(r1);
            u.Save();

            User u2 = User.SelectByID(u.ID);
            u2.Save();
            Assert.AreEqual(false, u2.Roles.ListRetrieved);
        }
    }
}






















