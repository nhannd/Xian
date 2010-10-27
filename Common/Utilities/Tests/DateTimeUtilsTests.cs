#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class DateTimeUtilsTests
    {
        public DateTimeUtilsTests()
        {
        }

        [Test]
        public void TestFormatISO()
        {
            DateTime date1 = new DateTime(2008, 4, 10, 6, 30, 0);
            Assert.AreEqual("2008-04-10T06:30:00", DateTimeUtils.FormatISO(date1));
        }

        [Test]
        public void TestParseISO()
        {
            string s = "2008-04-10T06:30:00";
            DateTime date1 = new DateTime(2008, 4, 10, 6, 30, 0);
            Assert.AreEqual(date1, DateTimeUtils.ParseISO(s));
        }
    }
}

#endif