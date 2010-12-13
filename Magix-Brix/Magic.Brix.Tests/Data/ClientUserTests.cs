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
using Magic.Brix.Data.Internal;
using Magic.Brix.Data.Adapters;
using WineTasting.CoreTypes;

namespace Magic.Brix.Tests.Data
{
    [TestFixture]
    public class ClientUser : BaseTest
    {
        [Test]
        public void CreateClient()
        {
            SetUp();

            Client client = new Client();
            client.Name = "Howdy";
            client.Save();

            Assert.AreEqual(1, Client.Count);
            Assert.AreEqual(Client.SelectFirst().Name, "Howdy");
        }

        [Test]
        public void CreateUser()
        {
            SetUp();

            User u = new User();
            u.Username = "thomas";
            u.Save();

            Assert.AreEqual(1, User.Count);
            Assert.AreEqual(User.SelectFirst().Username, "thomas");
        }

        [Test]
        public void CreateUserAndClient()
        {
            SetUp();

            User u = new User();
            u.Username = "thomas";
            u.Save();

            Client c = new Client();
            c.Name = "client";
            c.Users.Add(u);
            c.Save();

            c = Client.SelectFirst();
            Assert.AreEqual("thomas", c.Users[0].Username);
        }

        [Test]
        public void CreateUserAndClientReversed()
        {
            SetUp();

            User u = new User();
            u.Username = "thomas";
            u.Save();

            Client c = new Client();
            c.Name = "client";
            c.Users.Add(u);
            c.Save();

            u = User.SelectFirst();
            Assert.AreEqual("client", u.Client.Name);
        }

        [Test]
        public void CreateTwoClientsAndUserInSecond()
        {
            SetUp();

            Client c1 = new Client();
            c1.Name = "client1";
            c1.Save();

            Client c2 = new Client();
            c2.Name = "client2";
            c2.Save();

            User u1 = new User();
            u1.Username = "user1";
            u1.Client = c1;
            u1.Save();

            User u2 = new User();
            u2.Username = "user2";
            u2.Client = c2;
            u2.Save();

            u1 = User.SelectByID(u1.ID);
            Assert.AreEqual("client1", u1.Client.Name);

            c1 = Client.SelectByID(c1.ID);
            Assert.AreEqual("user1", c1.Users[0].Username);

            u2 = User.SelectByID(u2.ID);
            Assert.AreEqual("client2", u2.Client.Name);

            c2 = Client.SelectByID(c2.ID);
            Assert.AreEqual("user2", c2.Users[0].Username);
        }

        [Test]
        public void CreateUserAndClientSaveClientAfterUser()
        {
            SetUp();

            User u = new User();
            u.Username = "thomas";
            u.Save();

            Client c = new Client();
            c.Name = "client";
            c.Users.Add(u);
            c.Save();

            u = User.SelectFirst();
            u.Save();

            c = Client.SelectFirst();
            c.Save();

            u = User.SelectFirst();
            c = Client.SelectFirst();
            Assert.AreEqual("client", u.Client.Name);
            Assert.AreEqual("thomas", c.Users[0].Username);
        }

        [Test]
        public void CreateWineAndMenu()
        {
            Menu m = new Menu();
            m.Name = "whites";
            m.Save();

            Wine w = new Wine();
            w.Name = "cab";
            w.Save();

            w = Wine.SelectFirst();
            m = Menu.SelectFirst();
            m.Wines.Add(w);
            m.Save();

            w = Wine.SelectFirst();
            Assert.AreEqual("whites", w.Menu.Name);
        }

        [Test]
        public void CreateWineAndMenuReversed()
        {
            Menu m = new Menu();
            m.Name = "whites";
            m.Save();

            Wine w = new Wine();
            w.Name = "cab";
            w.Save();

            w = Wine.SelectFirst();
            m = Menu.SelectFirst();
            w.Menu = m;
            w.Save();

            m = Menu.SelectFirst();
            Assert.AreEqual("cab", m.Wines[0].Name);
        }
    }
}
