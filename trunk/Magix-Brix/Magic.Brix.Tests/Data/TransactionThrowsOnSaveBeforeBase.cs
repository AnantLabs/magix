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
