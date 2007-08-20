#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

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
			Lut lut = new Lut(10);
			Assert.AreEqual(10, lut.Length);

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
			Lut lut = new Lut(0);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NumEntriesNegative()
		{
			Lut lut = new Lut(-1);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			Lut lut = new Lut(10);
			lut[-1] = -8;
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			Lut lut = new Lut(10);
			lut[10] = -8;
		}

	}
}

#endif