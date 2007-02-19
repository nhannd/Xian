#if	UNIT_TESTS

using System;
using System.Collections;
using System.Drawing;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	unsafe public class PixelDataWrapperTest
	{
		public PixelDataWrapperTest()
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
		public void SetPixel8()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new PixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation,
				pixelData);

			int x = 3;
			int y = 4;

			byte testValue = 0xab;
			pixelDataWrapper.SetPixel8(x, y, testValue);
			byte actualValue = pixelDataWrapper.GetPixel8(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelData)
			{
				int i = y * columns + x;
				actualValue = pPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}
		
		[Test]
		public void SetPixel16()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new PixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation,
				pixelData);

			int x = 3;
			int y = 4;

			// Test the API
			ushort testValue = 0xabcd;
			pixelDataWrapper.SetPixel16(x, y, testValue);
			ushort actualValue = pixelDataWrapper.GetPixel16(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelData)
			{
				ushort* pUShortPixelData = (ushort*)pPixelData;
				int i = y * columns + x;
				actualValue = pUShortPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelRGBTriplet()
		{
			int rows = 19;
			int columns = 7;
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 3;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Rgb;
			int imageSize = rows * columns * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new PixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation,
				pixelData);

			int x = 3;
			int y = 4;

			Color testValue = Color.FromArgb(10, 20, 30);
			pixelDataWrapper.SetPixelRGB(x, y, testValue);
			Color actualValue = pixelDataWrapper.GetPixelRGB(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelData)
			{
				int bytesPerPixel = 3;
				int stride = columns * bytesPerPixel;
				int i = (y * stride) + (x * bytesPerPixel);
				actualValue = Color.FromArgb(pPixelData[i], pixelData[i + 1], pixelData[i + 2]);
			}

			Assert.AreEqual(testValue, actualValue);
		}

	}
}

#endif