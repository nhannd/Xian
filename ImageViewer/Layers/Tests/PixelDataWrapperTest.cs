#if	UNIT_TESTS

using System;
using System.Collections;
using System.Drawing;
using NUnit.Framework;
using ClearCanvas.Workstation.Model.Imaging;
using ClearCanvas.Workstation.Model.Layers;

namespace ClearCanvas.Workstation.Model.Tests
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
			int width = 7;
			int height = 19;
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretations pi = PhotometricInterpretations.Monochrome2;
			int imageSize = width * height * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelDataWrapper pixelDataWrapper = new PixelDataWrapper(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				pi,
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
				int i = y * width + x;
				actualValue = pPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}
		
		[Test]
		public void SetPixel16()
		{
			int width = 7;
			int height = 19;
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretations pi = PhotometricInterpretations.Monochrome2;
			int imageSize = width * height * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelDataWrapper pixelDataWrapper = new PixelDataWrapper(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				pi,
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
				int i = y * width + x;
				actualValue = pUShortPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelRGBTriplet()
		{
			int width = 7;
			int height = 19;
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 3;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretations pi = PhotometricInterpretations.Rgb;
			int imageSize = width * height * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelDataWrapper pixelDataWrapper = new PixelDataWrapper(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				pi,
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
				int stride = width * bytesPerPixel;
				int i = (y * stride) + (x * bytesPerPixel);
				actualValue = Color.FromArgb(pPixelData[i], pixelData[i + 1], pixelData[i + 2]);
			}

			Assert.AreEqual(testValue, actualValue);
		}

	}
}

#endif