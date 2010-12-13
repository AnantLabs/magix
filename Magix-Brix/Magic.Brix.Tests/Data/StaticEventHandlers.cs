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
using Magic.Brix.Types;
using Magic.Brix.Loader;
using Magic.Brix.Data.Internal;
using Magic.Brix.Data.Adapters;

namespace Magic.Brix.Tests.Data
{
    [TestFixture]
    public class StaticEventHandlers : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            [ActiveField]
            public string Username { get; set; }

            public static bool WasTriggered;

            public override void Save()
            {
                base.Save();

                // Stupid de-reference to make sure we've initialized all our types and such...!
                // In an actual application, this will never be neseccary since all those things
                // triggering events will be running something within the PluginLoader BEFORE
                // they run anything in the Events class...
                object tmp = PluginLoader.Instance;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "SaveTriggered", 
                    new Node("Value", 5));
            }

            [ActiveEvent(Name = "SaveTriggered")]
            private static void SaveTriggered(object sender, ActiveEventArgs e)
            {
                if (sender is User && (int)e.Params.Value == 5)
                    WasTriggered = true;
            }
        }

        [Test]
        public void VerifyEventTriggeredInStaticModelMethod()
        {
            SetUp();
            User u = new User();
            u.Username = "thomas";
            u.Save();
            Assert.AreEqual(true, User.WasTriggered);
        }
    }
}
