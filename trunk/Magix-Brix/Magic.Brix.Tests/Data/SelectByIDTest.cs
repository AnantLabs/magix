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

namespace Magic.Brix.Tests.Data
{
    [TestFixture]
    public class SelectByIDTest : BaseTest
    {
        [ActiveType]
        public class Dummy : ActiveType<Dummy>
        {
            [ActiveField]
            public int Value { get; set; }
        }

        [Test]
        public void SaveAndSelectByID()
        {
            SetUp();

            Dummy d = new Dummy();
            d.Value = 5;
            d.Save();

            Assert.AreNotEqual(0, d.ID);

            Dummy d2 = Dummy.SelectByID(d.ID);
            Assert.AreEqual(5, d2.Value);
        }

        [Test]
        public void SaveMultipleAndSelectByIDs()
        {
            SetUp();

            Dummy d1 = new Dummy();
            d1.Value = 1;
            d1.Save();

            Dummy d2 = new Dummy();
            d2.Value = 2;
            d2.Save();

            Dummy d3 = new Dummy();
            d3.Value = 3;
            d3.Save();

            List<Dummy> res = new List<Dummy>(
                Dummy.SelectByIDs(d1.ID, d2.ID, d3.ID));
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(1, res[0].Value);
            Assert.AreEqual(2, res[1].Value);
            Assert.AreEqual(3, res[2].Value);
        }
    }
}
