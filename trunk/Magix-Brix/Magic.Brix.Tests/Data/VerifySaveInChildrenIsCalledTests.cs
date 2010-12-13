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
    public class VerifySaveInChildrenIsCalledTests : BaseTest
    {
        [ActiveType]
        public class Wine : ActiveType<Wine>
        {
            public bool WasCalled;

            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                WasCalled = true;
                base.Save();
            }
        }

        [ActiveType]
        public class Menu : ActiveType<Menu>
        {
            public Menu()
            {
                Wines = new LazyList<Wine>();
            }

            [ActiveField]
            public LazyList<Wine> Wines { get; set; }

            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                base.Save();
            }
        }

        [ActiveType]
        public class Wine2 : ActiveType<Wine2>
        {
            public bool WasCalled;

            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                WasCalled = true;
                base.Save();
            }
        }

        [ActiveType]
        public class Menu2 : ActiveType<Menu2>
        {
            public Menu2()
            {
                Wines = new List<Wine2>();
            }

            [ActiveField]
            public List<Wine2> Wines { get; set; }

            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                base.Save();
            }
        }

        [Test]
        public void VerifySaveInLazyList()
        {
            SetUp();

            Wine w = new Wine();
            w.Name = "Cab";

            Menu m = new Menu();
            m.Name = "Menu1";
            m.Wines.Add(w);
            m.Save();

            Assert.AreEqual(true, w.WasCalled);
        }

        [Test]
        public void VerifySaveInList()
        {
            SetUp();

            Wine2 w = new Wine2();
            w.Name = "Cab";

            Menu2 m = new Menu2();
            m.Name = "Menu1";
            m.Wines.Add(w);
            m.Save();

            Assert.AreEqual(true, w.WasCalled);
        }
    }
}
