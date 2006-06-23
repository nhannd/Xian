#if	UNIT_TESTS

using System;
using System.Collections;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.Tests
{
	/// <summary>
	/// Summary description for GrayscaleLUTTest.
	/// </summary>
	[TestFixture]
	public class GrayscaleLUTTest
	{
		public GrayscaleLUTTest()
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
		public void CreateLUTRangePositive()
		{
			GrayscaleLUT lut = new GrayscaleLUT(10,73);
			
			Assert.IsTrue(lut.NumEntries == 64);
			Assert.IsTrue(lut.MinInputValue == 10);
			Assert.IsTrue(lut.MaxInputValue == 73);
		
			lut[10] = 100;
			lut[73] = 200;

			Assert.IsTrue(lut[10] == 100);
			Assert.IsTrue(lut[73] == 200);
		}

		[Test]
		public void CreateLUTRangeSigned()
		{
			GrayscaleLUT lut = new GrayscaleLUT(-10,53);
			
			Assert.IsTrue(lut.NumEntries == 64);
			Assert.IsTrue(lut.MinInputValue == -10);
			Assert.IsTrue(lut.MaxInputValue == 53);
		
			lut[-10] = 100;
			lut[53] = 200;

			Assert.IsTrue(lut[-10] == 100);
			Assert.IsTrue(lut[53] == 200);
		}
		
		[Test]
		public void CreateLUTRangeNegative()
		{
			GrayscaleLUT lut = new GrayscaleLUT(-74,-11);
			
			Assert.IsTrue(lut.NumEntries == 64);
			Assert.IsTrue(lut.MinInputValue == -74);
			Assert.IsTrue(lut.MaxInputValue == -11);
		
			lut[-74] = 100;
			lut[-11] = 200;

			Assert.IsTrue(lut[-74] == 100);
			Assert.IsTrue(lut[-11] == 200);
		}

		[Test]
		public void CreateLUTRangeNegativeZero()
		{
			GrayscaleLUT lut = new GrayscaleLUT(-63,0);
			
			Assert.IsTrue(lut.NumEntries == 64);
			Assert.IsTrue(lut.MinInputValue == -63);
			Assert.IsTrue(lut.MaxInputValue == 0);
		
			lut[-63] = 100;
			lut[0] = 200;

			Assert.IsTrue(lut[-63] == 100);
			Assert.IsTrue(lut[0] == 200);
		}
	}
}

#endif