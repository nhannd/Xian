#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using NUnit.Framework;

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
			bool isSigned = false;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
			bool isSigned = false;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
		public void SetPixelSigned16Grayscale()
		{
			int columns = 7;
			int rows = 19;
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				pixelData);

			int x = 3;
			int y = 4;

			// Test the API
			int testValue = -1234;
			pixelDataWrapper.SetPixel(x, y, testValue);
			int actualValue = pixelDataWrapper.GetPixel(x, y);
			Assert.AreEqual(testValue, actualValue);

			int index = y * columns + x;
			actualValue = pixelDataWrapper.GetPixel(index);
			Assert.AreEqual(testValue, actualValue);
		}

		[Test]
		public void SetPixelARGB()
		{
			int rows = 19;
			int columns = 7;
			int bitsAllocated = 8;
			int samplesPerPixel = 4;
			int imageSize = rows * columns * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			ColorPixelData pixelDataWrapper = new ColorPixelData(
				rows,
				columns,
				pixelData);

			int x = 3;
			int y = 4;

			Color testValue = Color.FromArgb(10, 20, 30);
			pixelDataWrapper.SetPixel(x, y, testValue);
			Color actualValue = pixelDataWrapper.GetPixelAsColor(x, y);
			Assert.AreEqual(testValue, actualValue);

			pixelDataWrapper.SetPixel(x, y, testValue.ToArgb());
			actualValue = pixelDataWrapper.GetPixelAsColor(x, y);
			Assert.AreEqual(testValue, actualValue);

			pixelDataWrapper.SetPixel(x, y, testValue.ToArgb());
			actualValue = Color.FromArgb(pixelDataWrapper.GetPixel(x, y));
			Assert.AreEqual(testValue, actualValue);

			// Make sure it works with unsafe code too
			fixed (byte* pPixelData = pixelDataWrapper.Raw)
			{
				int bytesPerPixel = 4;
				int stride = columns * bytesPerPixel;
				int i = (y * stride) + (x * bytesPerPixel);
				actualValue = Color.FromArgb(pPixelData[i + 2], pixelData[i + 1], pixelData[i]);
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
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
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

		[Test]
		public void ForEachPixel()
		{
			int columns = 5;
			int rows = 4;
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			bool isSigned = true;
			int samplesPerPixel = 1;
			int imageSize = columns * rows * bitsAllocated / 8 * samplesPerPixel;
			byte[] pixelData = new byte[imageSize];

			PixelData pixelDataWrapper = new GrayscalePixelData(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				pixelData);

			int i = 0;

			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < columns; x++)
				{
					pixelDataWrapper.SetPixel(x, y, i);
					i++;
				}
			}

			int left = 1, top = 1, right = 3, bottom = 3;

			int pixelCount = 0;

			pixelDataWrapper.ForEachPixel(left, top, right, bottom,
				delegate(int j, int x, int y, int pixelIndex)
				{
					if (j == 0)
						Assert.AreEqual(6, pixelDataWrapper.GetPixel(pixelIndex));
					else if (j == 1)
						Assert.AreEqual(7, pixelDataWrapper.GetPixel(pixelIndex));
					else if (j == 3)
						Assert.AreEqual(11, pixelDataWrapper.GetPixel(pixelIndex));

					pixelCount++;
				});

			Assert.AreEqual(9, pixelCount);
		}
	}
}

#endif