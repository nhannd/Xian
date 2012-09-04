#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    [TestFixture]
    public class StorageConfigurationTests
    {
        const long _kiloByte = 1024;
        const long _megaByte = _kiloByte * _kiloByte;
        const long _gigaByte = _megaByte * _kiloByte;
        const long _terabyte = _gigaByte * _kiloByte;
        const long _petaByte = _terabyte * _kiloByte;
        const long _halfGig = 500 * _megaByte;

        [Test]
        public void TestMinimumFreeSpaceBytes()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
                                                     {
                                                         FileStoreDirectory = @"c:\filestore",
                                                         FileStoreDiskSpace = diskSpace
                                                     };

            configuration.MinimumFreeSpacePercent = 90;
            Assert.AreEqual(1013309916158361, configuration.MinimumFreeSpaceBytes);
        }

        [Test]
        public void TestMaximumUsedSpace()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = 90;
            Assert.AreEqual(10, configuration.MaximumUsedSpacePercent);
            Assert.AreEqual(_petaByte - 1013309916158361, configuration.MaximumUsedSpaceBytes);
        }

        [Test]
        public void TestSetMinUsedSpace_Auto()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = -10;
            Assert.AreEqual(configuration.MinimumFreeSpacePercent, StorageConfiguration.AutoMinimumFreeSpace);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetMinUsedSpace_InvalidPercent()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = 110;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetMaxUsedSpace_InvalidPercent()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MaximumUsedSpacePercent = -1;
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDriveNotSet()
        {
            var configuration = new StorageConfiguration {FileStoreDirectory = @"c:\filestore"};
            var maxUsedSpace = configuration.MaximumUsedSpaceBytes;
        }

        [Test]
        public void TestFileStoreDirectoryValid()
        {
            var configuration = new StorageConfiguration { FileStoreDirectory = @"c:\filestore" };
            Assert.IsTrue(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"A:\\";
            Assert.IsTrue(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\\";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\\test\testing";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);
        }
    }
}

#endif