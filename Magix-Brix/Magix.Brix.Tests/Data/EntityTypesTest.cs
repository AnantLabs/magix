﻿/*
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
    public class EntityTypesTest : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            [ActiveField]
            public string Username { get; set; }

            [ActiveField]
            public string Password { get; set; }
        }

        [ActiveType]
        internal class User2 : ActiveType<User2>
        {
            [ActiveField]
            public string Username { get; set; }

            [ActiveField]
            public string Password { get; set; }
        }

        [Test]
        public void TestEntityTypeSingleSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u = new User();
            u.Save();
        }

        [Test]
        public void TestEntityTypeSingleSaveAndRetrieve()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u = new User();
            u.Save();

            u = User.SelectFirst();
            Assert.IsNotNull(u);
        }

        [Test]
        public void TestEntityTypeSingleSaveAndRetrieveVerifyValues()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u = new User();
            u.Username = "howdy";
            u.Password = "hello";
            u.Save();
            int id = u.ID;

            u = User.SelectByID(id);
            Assert.IsNotNull(u);
            Assert.AreEqual(id, u.ID);
            Assert.AreEqual("howdy", u.Username);
            Assert.AreEqual("hello", u.Password);
        }

        [Test]
        public void ChangeEntityTypeAndVerifySameIDAndUpdatedValues()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u = new User();
            u.Username = "howdy";
            u.Password = "hello";
            u.Save();
            int id = u.ID;

            u = User.SelectByID(id);
            u.Username = "howdy2";
            u.Save();

            u = User.SelectByID(id);
            Assert.AreEqual(id, u.ID);
            Assert.AreEqual("howdy2", u.Username);
            Assert.AreEqual("hello", u.Password);
        }

        [Test]
        public void CreateManyEntityTypes()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username-u1";
            u1.Password = "password-p1";
            u1.Save();

            User u2 = new User();
            u2.Username = "username-u2";
            u2.Password = "password-p2";
            u2.Save();

            User u3 = new User();
            u3.Username = "username-u3";
            u3.Password = "password-p3";
            u3.Save();

            List<User> users = new List<User>(User.Select());
            Assert.AreEqual(3, users.Count);

            Assert.AreEqual("username-u1", users[0].Username);
            Assert.AreEqual("password-p1", users[0].Password);
            Assert.AreEqual(u1.ID, users[0].ID);

            Assert.AreEqual("username-u2", users[1].Username);
            Assert.AreEqual("password-p2", users[1].Password);
            Assert.AreEqual(u2.ID, users[1].ID);

            Assert.AreEqual("username-u3", users[2].Username);
            Assert.AreEqual("password-p3", users[2].Password);
            Assert.AreEqual(u3.ID, users[2].ID);
        }

        [Test]
        public void CreateAndChangeManyEntityTypes()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username-u1";
            u1.Password = "password-p1";
            u1.Save();

            User u2 = new User();
            u2.Username = "username-u2";
            u2.Password = "password-p2";
            u2.Save();

            User u3 = new User();
            u3.Username = "username-u3";
            u3.Password = "password-p3";
            u3.Save();

            List<User> users = new List<User>(User.Select());
            Assert.AreEqual(3, users.Count);

            users[0].Username = "user1";
            users[0].Password = "pwd1";
            users[1].Username = "user2";
            users[1].Password = "pwd2";
            users[2].Username = "user3";
            users[2].Password = "pwd3";
            users[0].Save();
            users[1].Save();
            users[2].Save();

            users = new List<User>(User.Select());
            Assert.AreEqual(3, users.Count);

            Assert.AreEqual("user1", users[0].Username);
            Assert.AreEqual("pwd1", users[0].Password);
            Assert.AreEqual(u1.ID, users[0].ID);

            Assert.AreEqual("user2", users[1].Username);
            Assert.AreEqual("pwd2", users[1].Password);
            Assert.AreEqual(u2.ID, users[1].ID);

            Assert.AreEqual("user3", users[2].Username);
            Assert.AreEqual("pwd3", users[2].Password);
            Assert.AreEqual(u3.ID, users[2].ID);
        }

        [Test]
        public void VerifySelectLike()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username-u1";
            u1.Password = "password-p1";
            u1.Save();

            User u2 = new User();
            u2.Username = "username-u2";
            u2.Password = "password-p2";
            u2.Save();

            User u3 = new User();
            u3.Username = "username-u3";
            u3.Password = "password-p3";
            u3.Save();

            User u4 = new User();
            u4.Username = "xxx_username-u4";
            u4.Password = "xxx_password-p4";
            u4.Save();

            List<User> users = new List<User>(User.Select(Criteria.Like("Username", "username-u%")));
            Assert.AreEqual(3, users.Count);
            Assert.AreEqual("username-u1", users[0].Username);
            Assert.AreEqual("password-p1", users[0].Password);
            Assert.AreEqual("username-u2", users[1].Username);
            Assert.AreEqual("password-p2", users[1].Password);
            Assert.AreEqual("username-u3", users[2].Username);
            Assert.AreEqual("password-p3", users[2].Password);
        }

        [Test]
        public void VerifySelectEqual()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username-u1";
            u1.Password = "password-p1";
            u1.Save();

            User u2 = new User();
            u2.Username = "username-u2";
            u2.Password = "password-p2";
            u2.Save();

            List<User> users = new List<User>(
                User.Select(
                    Criteria.Eq("Username", "username-u1")));
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual("username-u1", users[0].Username);
            Assert.AreEqual("password-p1", users[0].Password);
        }

        [Test]
        public void VerifySelectEqualMany()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username";
            u1.Password = "password-p1";
            u1.Save();

            User u2 = new User();
            u2.Username = "username";
            u2.Password = "password-p2";
            u2.Save();

            User u3 = new User();
            u3.Username = "username-u3";
            u3.Password = "password-p3";
            u3.Save();

            List<User> users = new List<User>(
                User.Select(
                    Criteria.Eq("Username", "username")));
            Assert.AreEqual(2, users.Count);
            Assert.AreEqual("username", users[0].Username);
            Assert.AreEqual("password-p1", users[0].Password);
            Assert.AreEqual("username", users[1].Username);
            Assert.AreEqual("password-p2", users[1].Password);
        }

        [Test]
        public void VerifySelectEqualManyCriterias()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username";
            u1.Password = "password";
            u1.Save();

            User u2 = new User();
            u2.Username = "username";
            u2.Password = "password";
            u2.Save();

            User u3 = new User();
            u3.Username = "username";
            u3.Password = "password-XX";
            u3.Save();

            List<User> users = new List<User>(
                User.Select(
                    Criteria.Eq("Username", "username"),
                    Criteria.Eq("Password", "password")
                        ));
            Assert.AreEqual(2, users.Count);

            Assert.AreEqual("username", users[0].Username);
            Assert.AreEqual("password", users[0].Password);
            Assert.AreEqual(u1.ID, users[0].ID);

            Assert.AreEqual("username", users[1].Username);
            Assert.AreEqual("password", users[1].Password);
            Assert.AreEqual(u2.ID, users[1].ID);
        }

        [Test]
        public void VerifySelectEqualManyDifferentCriterias()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Username = "username";
            u1.Password = "password";
            u1.Save();

            User u2 = new User();
            u2.Username = "username";
            u2.Password = "password";
            u2.Save();

            User u3 = new User();
            u3.Username = "username";
            u3.Password = "password-XX";
            u3.Save();

            List<User> users = new List<User>(
                User.Select(
                    Criteria.Like("Username", "%rna%"),
                    Criteria.Eq("Password", "password")
                        ));
            Assert.AreEqual(2, users.Count);

            Assert.AreEqual("username", users[0].Username);
            Assert.AreEqual("password", users[0].Password);
            Assert.AreEqual(u1.ID, users[0].ID);

            Assert.AreEqual("username", users[1].Username);
            Assert.AreEqual("password", users[1].Password);
            Assert.AreEqual(u2.ID, users[1].ID);
        }

        [Test]
        public void VerifyCountWorks()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            User u1 = new User();
            u1.Save();

            User u2 = new User();
            u2.Save();

            User u3 = new User();
            u3.Save();

            Assert.AreEqual(3, User.Count);

            User u4 = new User();
            u4.Save();

            Assert.AreEqual(4, User.Count);
        }

        [Test]
        public void VerifyCountCriteriaWorks()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<User2>();
            User u1 = new User();
            u1.Username = "howdy1";
            u1.Save();

            User u2 = new User();
            u2.Username = "howdy2";
            u2.Save();

            User u3 = new User();
            u3.Username = "howdy3";
            u3.Save();

            Assert.AreEqual(3, User.Count);

            User u4 = new User();
            u4.Username = "not-counted...!";
            u4.Save();

            User2 userType2 = new User2();
            userType2.Username = "howdy";
            userType2.Save();

            Assert.AreEqual(3,
                User.CountWhere(
                    Criteria.Like(
                        "Username",
                        "howdy%")));

            Assert.AreEqual(4, User.Count);
            Assert.AreEqual(1, User2.Count);
        }
    }
}
