/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    public class CriteriaWithChildValue : BaseTest
    {
        [ActiveType]
        public class Role : ActiveType<Role>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [ActiveType]
        public class User : ActiveType<User>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public Role Role { get; set; }
        }

        [Test]
        public void GetCriteriaWithChildParameterValue()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            User u2 = User.SelectFirst(Criteria.Eq("Role.Name", "admin"));
            Assert.AreEqual("thomas", u2.Name);
        }

        [Test]
        public void GetCriteriaWithChildParameterValueMultipleValuesOneHit()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            r = new Role();
            r.Name = "admin2";

            u = new User();
            u.Name = "thomas2";
            u.Role = r;
            u.Save();

            List<User> u2 = new List<User>(User.Select(Criteria.Eq("Role.Name", "admin")));
            Assert.AreEqual(1, u2.Count);
            Assert.AreEqual("thomas", u2[0].Name);
        }

        [Test]
        public void GetCriteriaWithChildParameterValueMultipleValuesMultipleHits()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            r = new Role();
            r.Name = "admin2";

            u = new User();
            u.Name = "thomas2";
            u.Role = r;
            u.Save();

            List<User> u2 = new List<User>(User.Select(Criteria.Like("Role.Name", "adm%")));
            Assert.AreEqual(2, u2.Count);
            Assert.AreEqual("thomas", u2[0].Name);
            Assert.AreEqual("thomas2", u2[1].Name);
        }
    }
}
