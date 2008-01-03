#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class MinMaxPixelCalculatedLutTests
	{
		public MinMaxPixelCalculatedLutTests()
		{
		}

		[Test]
		public void TestSimple()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			IndexedPixelData pixelData = new IndexedPixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(lut.WindowWidth, 24);
			Assert.AreEqual(lut.WindowCenter, 12);
		}

		[Test]
		public void TestWithModalityLut()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			ModalityLutLinear modalityLut = new ModalityLutLinear(8, true, 1.0, -10);
			IndexedPixelData pixelData = new IndexedPixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData, modalityLut);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(lut.WindowWidth, 24);
			Assert.AreEqual(lut.WindowCenter, 2);
		}
	}
}

#endif