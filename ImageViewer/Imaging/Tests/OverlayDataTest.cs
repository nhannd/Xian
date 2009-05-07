#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class OverlayDataTest
	{
		public OverlayDataTest() {}

		[Test]
		public void TestBitUnpackingSingleFrame()
		{
			byte[] testData;

			// 1 frame, 5x3
			// 11111
			// 10001
			// 11111
			// continuous stream: 11111100 0111111x (little end first)
			// continuous LE byte stream: 00111111 x1111110
			// continuous BE word stream: x1111110 00111111
			testData = new byte[] {0x3f, 0x7e};
			TestFrame(testData, 5*3, 0, "111111000111111", "continuous stream: 11111100 0111111x");

			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous stream: 11111100 1000000x (little end first)
			// continuous LE byte stream: 00111111 x0000001
			testData = new byte[] {0x3f, 0x81};
			TestFrame(testData, 5*3, 0, "111111001000000", "continuous stream: 11111100 1000000x");
		}

		[Test]
		public void TestBitUnpackingMultiFrame()
		{
			byte[] testData;

			// 7 frames, each 3x2 (cols, rows) = 42 bits = 6 bytes
			// 111 111 111 111 111 111 111
			// 000 001 010 011 100 101 110
			// continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx
			// continuous LE byte stream: 11000111 01111001 11011101 11001111 11111011 xxxxxx01
			testData = new byte[] {0xc7, 0x79, 0xdd, 0xcf, 0xfb, 0xf1};
			TestFrame(testData, 3*2, 0, "111000", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 1, "111001", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 2, "111010", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 3, "111011", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 4, "111100", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 5, "111101", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 6, "111110", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
		}

		private void TestFrame(byte[] packedData, int outBufferSize, int frameNum, string expectedConcat, string datamsg)
		{
			byte[] result = new byte[outBufferSize];
			OverlayData.TestUnpack(packedData, result, frameNum * outBufferSize, false);
			Assert.AreEqual(expectedConcat, DumpNonZeroBytes(result), "LittleEndianWords Frame {0} of {1}", frameNum, datamsg);

			// you should get the exact same frame data (scanning horizontally from top left to bottom right) if you had packed data in little endian or big endian
			byte[] swappedPackedData = SwapBytes(packedData);
			result = new byte[outBufferSize];
			OverlayData.TestUnpack(swappedPackedData, result, frameNum * outBufferSize, true);
			Assert.AreEqual(expectedConcat, DumpNonZeroBytes(result), "BigEndianWords Frame {0} of {1}", frameNum, datamsg);
		}

		private static byte[] SwapBytes(byte[] swapBytes)
		{
			byte[] output = new byte[swapBytes.Length];
			for (int n = 0; n < swapBytes.Length; n += 2)
			{
				output[n + 1] = swapBytes[n];
				output[n] = swapBytes[n + 1];
			}
			return output;
		}

		private static string DumpNonZeroBytes(byte[] unpackedBits)
		{
			string s = "";

			for (int n = 0; n < unpackedBits.Length; n++)
			{
				if (unpackedBits[n] > 0)
					s += "1";
				else
					s += "0";
			}

			return s;
		}
	}
}

#endif