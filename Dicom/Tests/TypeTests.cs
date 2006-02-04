#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using NUnit.Framework;
    using ClearCanvas.Dicom.Exceptions;
    using ClearCanvas.Dicom;

    [TestFixture]
    public class TypeTests
    {

        [TestFixtureSetUp]
        public void Init()
        {
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
        }

        [Test]
        public void DicomTagTest()
        {
            DicomTag tag = new DicomTag(0x0000, 0x0000);
            Assert.IsTrue(tag.Group == 0x0000);
            Assert.IsTrue(tag.Element == 0x0000);

            DicomTag tag2 = new DicomTag(0x0001, 0x0002);
            Assert.IsTrue(tag.Group == 0x0001);
            Assert.IsTrue(tag.Element == 0x0002);

            DicomTag tag3 = new DicomTag(0x7fe0, 0010);
            Assert.IsTrue(tag.Group == 0x7fe0);
            Assert.IsTrue(tag.Element == 0x0010);
        }
    }
}

#endif