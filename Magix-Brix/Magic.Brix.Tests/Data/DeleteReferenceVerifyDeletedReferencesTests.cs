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
using Magic.Brix.Types;

namespace Magic.Brix.Tests.Data
{
    [TestFixture]
    public class DeleteReferenceVerifyDeletedReferencesTests : BaseTest
    {
        [ActiveType]
        public class Wine : ActiveType<Wine>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [ActiveType]
        public class Menu : ActiveType<Menu>
        {
            public Menu()
            {
                Wines = new LazyList<Wine>();
            }

            [ActiveField]
            public string Client { get; set; }

            [ActiveField(IsOwner = false)]
            public LazyList<Wine> Wines { get; set; }
        }

        [ActiveType]
        public class Client : ActiveType<Client>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [ActiveType]
        public class Menu2 : ActiveType<Menu2>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField(IsOwner = false)]
            public Client Client { get; set; }
        }

        [Test]
        public void CreateReferenceDeleteOneObjectVerifyOnlyOneReference()
        {
            SetUp();

            Wine w1 = new Wine();
            w1.Name = "Cabernet";
            w1.Save();

            Wine w2 = new Wine();
            w2.Name = "Chardonnay";
            w2.Save();

            Menu m = new Menu();
            m.Client = "Winery";
            m.Wines.Add(w1);
            m.Wines.Add(w2);
            m.Save();

            w2.Delete();

            Menu mAfter = Menu.SelectByID(m.ID);
            Assert.AreEqual(1, mAfter.Wines.Count);
        }

        [Test]
        public void CreateReferenceDeleteOneObjectVerifyOnlyOneReferenceNoList()
        {
            SetUp();

            Client c = new Client();
            c.Save();

            Menu2 m = new Menu2();
            m.Client = c;
            m.Client = c;
            m.Save();

            c.Delete();

            Menu2 m2 = Menu2.SelectByID(m.ID);
            Assert.IsNull(m2.Client);
        }
    }
}



























