#if	UNIT_TESTS

using System;
using System.Collections;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class LUTTest
	{
		public LUTTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void CreateLUT()
		{
			LUT lut = new LUT(10);
			Assert.AreEqual(10, lut.NumEntries);

			lut[0] = -8;
			lut[9] = 10;

			Assert.AreEqual(-8, lut[0]);
			Assert.AreEqual(0, lut[1]);
			Assert.AreEqual(10, lut[9]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NumEntriesZero()
		{
			LUT lut = new LUT(0);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NumEntriesNegative()
		{
			LUT lut = new LUT(-1);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			LUT lut = new LUT(10);
			lut[-1] = -8;
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			LUT lut = new LUT(10);
			lut[10] = -8;
		}

	}
}

#endif