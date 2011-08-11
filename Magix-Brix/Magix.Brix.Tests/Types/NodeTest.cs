/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using NUnit.Framework;
using Magix.Brix.Types;

namespace Magix.Brix.Tests.Types
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void SimpleContruction()
        {
            Node n = new Node("Customers");
            n["Customer1"].Value = "id1";
            n["Customer1"]["Name"].Value = "Thomas Hansen";
            n["Customer1"]["Name"]["First"].Value = "Thomas";
            n["Customer1"]["Name"]["Last"].Value = "Hansen";
            n["Customer2"].Value = "id2";
            n["Customer2"]["Name"].Value = "Kariem Ali";
            n["Customer2"]["Name"]["First"].Value = "Kariem";
            n["Customer2"]["Name"]["Last"].Value = "Ali";

            Assert.AreEqual("Customers", n.Name);
            Assert.AreEqual(null, n.Value);

            Assert.AreEqual("id1", n["Customer1"].Value);
            Assert.AreEqual("id2", n["Customer2"].Value);

            Assert.AreEqual("Thomas Hansen", n["Customer1"]["Name"].Value);
            Assert.AreEqual("Kariem Ali", n["Customer2"]["Name"].Value);

            Assert.AreEqual("Thomas", n["Customer1"]["Name"]["First"].Value);
            Assert.AreEqual("Kariem", n["Customer2"]["Name"]["First"].Value);

            Assert.AreEqual("Hansen", n["Customer1"]["Name"]["Last"].Value);
            Assert.AreEqual("Ali", n["Customer2"]["Name"]["Last"].Value);
        }
    }
}



















