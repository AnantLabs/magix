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
using Magic.Brix.Loader;
using Magic.Brix.Data.Internal;
using Magic.Brix.Data.Adapters;

namespace Magic.Brix.Tests.Modules
{
    [TestFixture]
    public class StaticEventHandlersModules
    {
        [ActiveModule]
        internal class TestModule
        {
            [ActiveEvent(Name = "TestEvent")]
            private static void TestEvent(object sender, ActiveEventArgs e)
            {
                WasTriggered = true;
            }

            public static bool WasTriggered { get; set; }
        }

        [Test]
        public void VerifyEventTriggeredInStaticModelMethod()
        {
            ActiveEvents.Instance.RaiseActiveEvent(this, "TestEvent");
            Assert.AreEqual(true, TestModule.WasTriggered);
        }
    }
}
