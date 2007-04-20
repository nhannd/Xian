#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

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
		public void SetPixelUnsigned8()
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

			int testValue = 0xab;
			pixelDataWrapper.SetPixel(x, y, testValue);
			int actualValue = pixelDataWrapper.GetPixel(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				int i = y * columns + x;
				actualValue = pPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelSigned8()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 1;
			int pixelRepresentation = 1;
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

			int testValue = -22;
			pixelDataWrapper.SetPixel(x, y, testValue);
			int actualValue = pixelDataWrapper.GetPixel(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				sbyte* pSBytePixelData = (sbyte*)pPixelData;
				int i = y * columns + x;
				actualValue = pSBytePixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelUnsigned16()
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
			int testValue = 0xabcd;
			pixelDataWrapper.SetPixel(x, y, testValue);
			int actualValue = pixelDataWrapper.GetPixel(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				ushort* pUShortPixelData = (ushort*)pPixelData;
				int i = y * columns + x;
				actualValue = pUShortPixelData[i];
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelSigned16()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			int samplesPerPixel = 1;
			int pixelRepresentation = 1;
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
			int testValue = -1234;
			pixelDataWrapper.SetPixel(x, y, testValue);
			int actualValue = pixelDataWrapper.GetPixel(x, y);
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				short* pShortPixelData = (short*)pPixelData;
				int i = y * columns + x;
				actualValue = pShortPixelData[i];
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

			pixelDataWrapper.SetPixel(x, y, testValue.ToArgb());
			actualValue = pixelDataWrapper.GetPixelRGB(x, y);
			Assert.AreEqual(testValue, actualValue);

			pixelDataWrapper.SetPixel(x, y, testValue.ToArgb());
			actualValue = Color.FromArgb(pixelDataWrapper.GetPixel(x, y));
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				int bytesPerPixel = 3;
				int stride = columns * bytesPerPixel;
				int i = (y * stride) + (x * bytesPerPixel);
				actualValue = Color.FromArgb(pPixelData[i], pixelData[i + 1], pixelData[i + 2]);
			}

			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void Signed9Bit()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 16;
			int bitsStored = 9;
			int highBit = 8;
			int samplesPerPixel = 1;
			int pixelRepresentation = 1;
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


			pixelData[0] = 255;
			pixelData[1] = 1;

			int actualValue = pixelDataWrapper.GetPixel(0, 0);

			Assert.AreEqual(-1, actualValue);

			int expectedValue = -1;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = -256;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = 255;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = 0;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);
		}
	
		[Test]
		public void Signed5Bit8BitsAllocated()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 8;
			int bitsStored = 5;
			int highBit = 4;
			int samplesPerPixel = 1;
			int pixelRepresentation = 1;
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

			Signed5Bit(pixelData, pixelDataWrapper);
		}

		[Test]
		public void Signed5Bit16BitsAllocated()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 16;
			int bitsStored = 5;
			int highBit = 4;
			int samplesPerPixel = 1;
			int pixelRepresentation = 1;
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

			Signed5Bit(pixelData, pixelDataWrapper);
		}

		private static void Signed5Bit(byte[] pixelData, PixelData pixelDataWrapper)
		{
			pixelData[0] = 31;

			int actualValue = pixelDataWrapper.GetPixel(0, 0);

			Assert.AreEqual(-1, actualValue);

			int expectedValue = -1;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = -16;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = 15;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);

			expectedValue = 0;
			pixelDataWrapper.SetPixel(0, 0, expectedValue);
			actualValue = pixelDataWrapper.GetPixel(0, 0);
			Assert.AreEqual(expectedValue, actualValue);
		}
	}
}

#endif