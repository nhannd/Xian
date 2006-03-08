#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using NUnit.Framework;
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
            Assert.IsTrue(tag2.Group == 0x0001);
            Assert.IsTrue(tag2.Element == 0x0002);

            DicomTag tag3 = new DicomTag(0x7fe0, 0x0010);
            Assert.IsTrue(tag3.Group == 0x7fe0);
            Assert.IsTrue(tag3.Element == 0x0010);

            DicomTag tag4 = DicomTag.StudyInstanceUID;
            Assert.IsTrue(tag4.Group == 0x0020);
            Assert.IsTrue(tag4.Element == 0x000d);

            DicomTag tag5 = DicomTag.StudyInstanceUID;
            DicomTag tag6 = DicomTag.StudyInstanceUID;


            //*  x.Equals(x) returns true, except in cases that involve floating-point types. See IEC 60559:1989, Binary Floating-point Arithmetic for Microprocessor Systems.
            //* x.Equals(y) returns the same value as y.Equals(x).
            //* x.Equals(y) returns true if both x and y are NaN.
            //* (x.Equals(y) && y.Equals(z)) returns true if and only if x.Equals(z) returns true.
            //* Successive calls to x.Equals(y) return the same value as long as the objects referenced by x and y are not modified.
            //* x.Equals(a null reference (Nothing)) returns false.

            Assert.IsTrue(tag4.Equals(tag4));
            Assert.IsTrue(tag4.Equals(tag3) == tag3.Equals(tag4));
            Assert.IsTrue(tag4.Equals(tag5) == tag5.Equals(tag4));
            Assert.IsTrue(tag4.Equals(tag6) && tag4.Equals(tag5) && tag5.Equals(tag6));
            Assert.IsTrue(tag4.Equals(tag5) && tag4.Equals(tag5) && tag4.Equals(tag5));
            Assert.IsFalse(tag4.Equals(null));

        }
    }
}

#endif