#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Tests
{
    [TestFixture]
    public class DriveInfoTests
    {
        const long _kiloByte = 1024;
        const long _megaByte = _kiloByte * _kiloByte;
        const long _gigaByte = _megaByte * _kiloByte;
        const long _terabyte = _gigaByte * _kiloByte;
        const long _petaByte = _terabyte * _kiloByte;
        const long _halfGig = 500 * _megaByte;

        [Test]
        public void TestUsedSpacePercent()
        {
            var info = new DriveInfo { TotalSize = _petaByte, AvailableFreeSpace = _halfGig, TotalFreeSpace = _halfGig };
            Assert.AreEqual(99.999953433871269, info.TotalUsedSpacePercent);
            Assert.AreEqual(100 - 99.999953433871269, info.TotalFreeSpacePercent);
        }

        [Test]
        public void TestUsedSpace()
        {
            var info = new DriveInfo { TotalSize = _petaByte, AvailableFreeSpace = _halfGig, TotalFreeSpace = _halfGig };
            Assert.AreEqual(_petaByte - _halfGig, info.TotalUsedSpace);
        }
    }
}

#endif