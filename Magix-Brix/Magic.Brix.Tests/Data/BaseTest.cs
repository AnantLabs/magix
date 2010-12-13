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
    public class BaseTest
    {
        [TestFixtureSetUp]
        public void SetUpTestFixture()
        {
            // Stupid reference to make sure assembly gets into AppDomain...!
            Brix.Data.Adapters.MSSQL.MSSQL MSSQL = null;
        }

        [TestFixtureTearDown]
        public void TearDownTestFixture()
        {
        }

        // This method will *delete all* documents in database...!
        protected void SetUp()
        {
            List<int> idsOfAllDocumentsToDelete = new List<int>();
            foreach (object idx in Adapter.Instance.Select())
            {
                Type type = idx.GetType();
                PropertyInfo method = type.GetProperty(
                    "ID", 
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic);
                int id = (int)method.GetGetMethod(true).Invoke(idx, null);
                idsOfAllDocumentsToDelete.Add(id);
            }
            foreach (int idx in idsOfAllDocumentsToDelete)
            {
                Adapter.Instance.Delete(idx);
            }
        }
    }
}
