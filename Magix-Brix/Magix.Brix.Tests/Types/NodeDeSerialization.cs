/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using NUnit.Framework;
using Magix.Brix.Types;

namespace Magix.Brix.Tests.Types
{
    [TestFixture]
    public class NodeDeSerializationTest
    {
        [Test]
        public void SimpleDeSerializationTest()
        {
            Node orig = new Node("SomeNode");
            orig["One"].Value = "1";
            string str = orig.ToJSONString();

            Node deSer = Node.FromJSONString(str);
            Assert.AreEqual(orig.Name, deSer.Name);
            Assert.AreEqual(orig.Value, deSer.Value);
            Assert.AreEqual(1, deSer.Count);
            Assert.AreEqual(orig.Count, deSer.Count);
            Assert.AreEqual(orig["One"].Value, deSer["One"].Value);
        }

        [Test]
        public void SimpleDeSerializationTestNoChildren()
        {
            Node orig = new Node("SomeNode");
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestOnlyChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestMultipleChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            orig["Two"].Value = "2";
            orig["Three"].Value = "3";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestRecursiveChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            orig["One"]["Two"].Value = "2";
            orig["One"]["Two"]["Three"].Value = "3";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }
    }
}



















