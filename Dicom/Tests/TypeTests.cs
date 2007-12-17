#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ClearCanvas.Dicom.OffisNetwork;
    using NUnit.Framework;
    using ClearCanvas.Dicom;

	internal class PathTest : DicomTagPath
	{
		public PathTest()
			: base()
		{ 
		}

		public void SetPath(string path)
		{
			base.Path = path;
		}

		public void SetPath(IList<DicomTag> tags)
		{
			base.TagsInPath = tags;
		}
	}

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
            /*
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
            */
        }

		[Test]
		public void DicomTagPathTests()
		{
			//the paths here are not real, they are just made up for the test.

			DicomTagPath path = new PathTest();
			((PathTest)path).SetPath("(0010,0010)");

			Assert.AreEqual(path, "(0010,0010)"); 
			Assert.AreEqual(path.ToString(), "(0010,0010)");
			Assert.AreEqual(path, (uint)0x00100010);
			Assert.AreEqual(path, NewDicomTag(0x00100010));
			Assert.IsTrue(path.Equals("(0010,0010)"));
			Assert.IsTrue(path.Equals((uint)0x00100010));
			Assert.IsTrue(path.Equals(NewDicomTag(0x00100010)));

			((PathTest)path).SetPath("(0010,0010)\\(0010,0018)");
			Assert.AreEqual(path, "(0010,0010)\\(0010,0018)"); 

			Assert.AreNotEqual(path, (uint)0x00100010);
			Assert.AreNotEqual(path, NewDicomTag(0x00100010));
			Assert.IsFalse(path.Equals("(0010,0010)"));
			Assert.IsFalse(path.Equals((uint)0x00100010));
			Assert.IsFalse(path.Equals(NewDicomTag(0x00100010)));

			((PathTest)path).SetPath("(0010,0010)\\(0010,0018)\\(0010,0022)");
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");

			path = new DicomTagPath("(0010,0010)");
			Assert.AreEqual(path.ToString(), "(0010,0010)");

			path = new DicomTagPath("(0010,0010)\\(0010,0018)");
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)");

			path = new DicomTagPath("(0010,0010)\\(0010,0018)\\(0010,0022)");
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");

			path = new PathTest();
			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00100010) }));
			Assert.AreEqual(path.ToString(), "(0010,0010)");

			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00100010), NewDicomTag(0x00100018) }));
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)");

			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00100010), NewDicomTag(0x00100018), NewDicomTag(0x00100022) }));
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");

			path = new DicomTagPath(NewDicomTag(0x00100010));
			Assert.AreEqual(path.ToString(), "(0010,0010)");

			path = new DicomTagPath(new DicomTag[] { NewDicomTag(0x00100010), NewDicomTag(0x00100018) });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)");

			path = new DicomTagPath(new DicomTag[] { NewDicomTag(0x00100010), NewDicomTag(0x00100018), NewDicomTag(0x00100022) });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");

			path = new DicomTagPath(NewDicomTag(0x0010, 0x0010));
			Assert.AreEqual(path.ToString(), "(0010,0010)");

			path = new DicomTagPath(new DicomTag[] { NewDicomTag(0x0010, 0x0010), NewDicomTag(0x0010, 0x0018) });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)");

			path = new DicomTagPath(new DicomTag[] { NewDicomTag(0x0010, 0x0010), NewDicomTag(0x0010, 0x0018), NewDicomTag(0x0010, 0x0022) });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");

			path = new DicomTagPath(0x00100010);
			Assert.AreEqual(path.ToString(), "(0010,0010)");

			path = new DicomTagPath(new uint[] { 0x00100010, 0x00100018 });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)");

			path = new DicomTagPath(new uint[] { 0x00100010, 0x00100018, 0x00100022 });
			Assert.AreEqual(path.ToString(), "(0010,0010)\\(0010,0018)\\(0010,0022)");
		}

		private DicomTag NewDicomTag(ushort group, ushort element)
		{
			return new DicomTag(DicomTag.GetTagValue(group, element), "Throwaway Tag", "ThrowawayTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}

		private DicomTag NewDicomTag(uint tag)
		{
			return new DicomTag(tag, "Throwaway Tag", "ThrowawayTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}
    }
}

#endif