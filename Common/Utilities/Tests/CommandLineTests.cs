#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
    [TestFixture]
    public class CommandLineTests
    {

        public CommandLineTests()
        {

        }

        [Test]
        public void TestNamedParameters()
        {
            string[] args = new string[] {"/h:hello", "/g=whatever man","-d:xx;yy", "-k=\\program files"};

            CommandLine cm = new CommandLine(args);

            Assert.AreEqual(0, cm.Positional.Count);
            Assert.AreEqual(0, cm.Switches.Count);
            Assert.AreEqual(4, cm.Named.Count);

            Assert.AreEqual("hello", cm.Named["h"]);
            Assert.AreEqual("whatever man", cm.Named["g"]);
            Assert.AreEqual("xx;yy", cm.Named["d"]);
            Assert.AreEqual("\\program files", cm.Named["k"]);
        }

        [Test]
        public void TestPositionalParameters()
        {
            string[] args = new string[] { "c:\\program files", "\\windows\\system32", "/linux/linux", "c:\\inetpub\\" };

            CommandLine cm = new CommandLine(args);

            Assert.AreEqual(4, cm.Positional.Count);
            Assert.AreEqual(0, cm.Switches.Count);
            Assert.AreEqual(0, cm.Named.Count);

            Assert.AreEqual("c:\\program files", cm.Positional[0]);
            Assert.AreEqual("\\windows\\system32", cm.Positional[1]);
            Assert.AreEqual("/linux/linux", cm.Positional[2]);
            Assert.AreEqual("c:\\inetpub\\", cm.Positional[3]);
        }

        [Test]
        public void TestSwitches()
        {
            string[] args = new string[] { "/a", "/b+", "/c-", "/d", "/d"};

            CommandLine cm = new CommandLine(args);

            Assert.AreEqual(0, cm.Positional.Count);
            Assert.AreEqual(4, cm.Switches.Count);
            Assert.AreEqual(0, cm.Named.Count);

            Assert.AreEqual(true, cm.Switches["a"]);
            Assert.AreEqual(true, cm.Switches["b"]);
            Assert.AreEqual(false, cm.Switches["c"]);
            Assert.AreEqual(false, cm.Switches["d"]);
        }

        [Test]
        public void TestSwitchToggle()
        {
            string[] args = new string[] { "/a", "/a", "/b-", "/b+", "/c+", "/c", "/d+", "/d-" };

            CommandLine cm = new CommandLine(args);

            Assert.AreEqual(0, cm.Positional.Count);
            Assert.AreEqual(4, cm.Switches.Count);
            Assert.AreEqual(0, cm.Named.Count);

            Assert.AreEqual(false, cm.Switches["a"]);
            Assert.AreEqual(true, cm.Switches["b"]);
            Assert.AreEqual(false, cm.Switches["c"]);
            Assert.AreEqual(false, cm.Switches["d"]);
        }
        [Test]
        public void TestCombination()
        {
            string[] args = new string[] { "c:\\program files", "/a", "/h:hello", "c:\\inetpub\\" };

            CommandLine cm = new CommandLine(args);

            Assert.AreEqual(2, cm.Positional.Count);
            Assert.AreEqual(1, cm.Switches.Count);
            Assert.AreEqual(1, cm.Named.Count);

            Assert.AreEqual(true, cm.Switches["a"]);
            Assert.AreEqual("hello", cm.Named["h"]);
            Assert.AreEqual("c:\\program files", cm.Positional[0]);
            Assert.AreEqual("c:\\inetpub\\", cm.Positional[1]);
        }
    }
}

#endif
