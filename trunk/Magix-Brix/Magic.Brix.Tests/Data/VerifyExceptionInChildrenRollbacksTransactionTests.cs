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
    public class VerifyExceptionInChildrenRollbacksTransactionTests : BaseTest
    {
        [ActiveType]
        public class Wine : ActiveType<Wine>
        {
            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                throw new Exception();
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

        [Test]
        public void ThrowExceptionVerifyNoDanglingDocuments()
        {
            SetUp();

            Wine w = new Wine();
            w.Name = "Cab";

            Menu m = new Menu();
            m.Name = "Menu1";
            m.Wines.Add(w);

            try
            {
                m.Save();
            }
            catch
            { }

            Assert.AreEqual(0, Menu.Count);
        }
    }
}
