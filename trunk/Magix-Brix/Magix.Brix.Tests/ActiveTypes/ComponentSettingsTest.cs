/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Framework;
using Magix.Brix.Data;
using Magix.Brix.Data.Internal;
using Magix.Brix.Data.Adapters;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Tests.Data;

namespace Magix.Brix.Tests.ActiveTypes
{
    [TestFixture]
    public class ComponentSettingsTest : BaseTest
    {
        [Test]
        public void TestClear()
        {
            Settings.Instance.Clear();
            Assert.AreEqual(0, Settings.Instance.Count);
        }

        [Test]
        public void TestSetValue()
        {
            Settings.Instance.Clear();
            Settings.Instance["Test"] = "howdy";
            Assert.AreEqual(1, Settings.Instance.Count);
        }

        [Test]
        public void TestSetTwoValues()
        {
            Settings.Instance.Clear();
            Settings.Instance["Test1"] = "howdy";
            Settings.Instance["Test2"] = "howdy";
            Assert.AreEqual(2, Settings.Instance.Count);
        }

        [Test]
        public void TestSetAndRetrieve()
        {
            Settings.Instance.Clear();
            Settings.Instance["Test"] = "howdy";
            Assert.AreEqual("howdy", Settings.Instance["Test"]);
        }

        [Test]
        public void TestGetNonExistingNullValue()
        {
            Settings.Instance.Clear();
            Assert.AreEqual(null, Settings.Instance["Test"]);
        }

        [Test]
        public void TestGetConfigValue()
        {
            Settings.Instance.Clear();
            ConfigurationManager.AppSettings.Set("ConfigValue", "ConfigValueTjobing");
            Assert.AreEqual("ConfigValueTjobing", Settings.Instance["ConfigValue"]);
        }

        [Test]
        public void TestGetTypedConfigValue()
        {
            Settings.Instance.Clear();
            ConfigurationManager.AppSettings.Set("ConfigValue", "5");
            Assert.AreEqual(5, Settings.Instance.Get<int>("ConfigValue"));
        }

        [Test]
        public void TestGetTypedNonExistingDefaultValue()
        {
            Settings.Instance.Clear();
            Assert.AreEqual(5, Settings.Instance.Get<int>("Test", 5));
        }

        [Test]
        public void TestGetTypedNonExistingValueNoDefault()
        {
            Settings.Instance.Clear();
            Assert.AreEqual(0, Settings.Instance.Get<int>("Test"));
        }

        [Test]
        public void TestSetStringGetTypedExistingValueDefaultWrong()
        {
            Settings.Instance.Clear();
            Settings.Instance["Test"] = "5";
            Assert.AreEqual(5, Settings.Instance.Get<int>("Test", 4));
        }

        [Test]
        public void TestSetTypedValueIntThenRetrieve()
        {
            Settings.Instance.Clear();
            Settings.Instance.Set("Test", 5);
            Assert.AreEqual(5, Settings.Instance.Get<int>("Test"));
        }

        [Test]
        public void TestSetTypedValueBoolThenRetrieve()
        {
            Settings.Instance.Clear();
            Settings.Instance.Set("Test", true);
            Assert.AreEqual(true, Settings.Instance.Get<bool>("Test"));
        }

        [Test]
        public void TestSetTypedValueDateTimeThenRetrieve()
        {
            Settings.Instance.Clear();
            DateTime date = new DateTime(2010, 10, 25, 11, 10, 9);
            Settings.Instance.Set("Test", date);
            Assert.AreEqual(date, Settings.Instance.Get<DateTime>("Test"));
        }

        [Test]
        public void TestSetValueMultipleTimesThenRetrieve()
        {
            Settings.Instance.Clear();
            DateTime date = new DateTime(2010, 10, 25, 11, 10, 9);
            for (int idx = 0; idx < 50; idx++)
            {
                Settings.Instance.Set("Test", date);
            }
            Assert.AreEqual(1, Settings.Instance.Count);
            Assert.AreEqual(date, Settings.Instance.Get<DateTime>("Test"));
        }

        [Test]
        public void TestSetValueMultipleTimesIndexerThenRetrieve()
        {
            Settings.Instance.Clear();
            for (int idx = 0; idx < 50; idx++)
            {
                Settings.Instance["Test"] = "howdy";
            }
            Assert.AreEqual(1, Settings.Instance.Count);
            Assert.AreEqual("howdy", Settings.Instance["Test"]);
        }

        [Test]
        public void TestEnumerating()
        {
            Settings.Instance.Clear();
            Settings.Instance["Test1"] = "howdy1";
            Settings.Instance["Test2"] = "howdy2";
            Settings.Instance["Test3"] = "howdy3";
            Settings.Instance["Test4"] = "howdy4";
            Settings.Instance["Test5"] = "howdy5";
            Assert.AreEqual(5, Settings.Instance.Count);

            int idxNo = 1;
            foreach (string idx in Settings.Instance.Keys)
            {
                Assert.AreEqual("Test" + idxNo, idx);
                Assert.AreEqual("howdy" + idxNo, Settings.Instance[idx]);
                idxNo += 1;
            }
        }
    }
}
