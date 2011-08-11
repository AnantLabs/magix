/*
 * Magix - A Modular-based Framework for building Web Applications 
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
    public class TransactionThrowsOnSaveBeforeBase : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                throw new ApplicationException("Intentional");
            }
        }

        [Test]
        public void CreateParentChildEditChildSaveParent()
        {
            SetUp();
            User u = new User();
            bool failure = true;
            try
            {
                u.Save();
            }
            catch
            {
                failure = false;
            }
            Assert.IsFalse(failure);
            Assert.AreEqual(0, User.Count);
        }
    }
}
