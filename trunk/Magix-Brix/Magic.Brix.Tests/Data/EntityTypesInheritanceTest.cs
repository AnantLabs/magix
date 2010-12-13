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
    public class EntityTypesInheritanceTest : BaseTest
    {
        [ActiveType]
        internal class Mammal<T> : ActiveType<T>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public bool IsPrimate { get; set; }
        }

        internal class Dog : Mammal<Dog>
        {
            [ActiveField]
            public bool Barks { get; set; }
        }

        [Test]
        public void TestInheritedTemplateClass()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Dog>();
            Dog d = new Dog();
            d.Name = "Goldie";
            d.IsPrimate = false;
            d.Barks = true;
            d.Save();

            Dog d2 = Dog.SelectByID(d.ID);
            Assert.IsNotNull(d2);
            Assert.AreEqual(d2.Name, "Goldie");
            Assert.AreEqual(d2.IsPrimate, false);
            Assert.AreEqual(d2.Barks, true);
        }
    }
}
