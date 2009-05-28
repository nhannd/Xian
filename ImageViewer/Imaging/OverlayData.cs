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

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class OverlayData
	{
		private int _offset;
		private int _rows;
		private int _columns;
		private bool _bigEndianWords;
		private byte[] _rawOverlayData;

		public OverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayData) : this(0, rows, columns, bigEndianWords, overlayData) {}

		public OverlayData(int offset, int rows, int columns, bool bigEndianWords, byte[] overlayData)
		{
			Platform.CheckNonNegative(offset, "offset");

			if (overlayData == null)
				overlayData = new byte[0];

			_offset = offset;
			_rows = rows;
			_columns = columns;
			_bigEndianWords = bigEndianWords;
			_rawOverlayData = overlayData;
		}

		public byte[] Raw
		{
			get { return _rawOverlayData; }
		}

		public byte[] Unpack()
		{
			byte[] unpackedPixelData = new byte[_rows * _columns];
			Unpack(_rawOverlayData, unpackedPixelData, _offset, _bigEndianWords);
			return unpackedPixelData;
		}

		public static byte[] UnpackFromPixelData(int bitPosition, int bitsAllocated, bool bigEndianWords, byte[] pixelData)
		{
			const byte ONE = 0xff;
			const byte ZERO = 0x00;

			if(bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("BitsAllocated must be either 8 or 16 bits.", "bitsAllocated");

			int inLen = pixelData.Length;
			int outLen = inLen/(bitsAllocated/8);
			int outPos = 0;
			byte[] extractedPixels = new byte[outLen];

			unsafe
			{

				fixed(byte* input = pixelData)
				{
					fixed(byte* output = extractedPixels)
					{
						if(bitsAllocated == 16)
						{
							if (inLen % 2 != 0)
								throw new ArgumentException("Pixel data length must be even.", "pixelData");

							int mask = 1 << bitPosition;
							if (bigEndianWords)
							{
								for (int inPos = 0; inPos < inLen; inPos += 2)
								{
									int value = (input[inPos] << 8) + input[inPos + 1];
									output[outPos++] = ((value & mask) > 0) ? ONE : ZERO;
								}
							}
							else
							{
								for (int inPos = 0; inPos < inLen; inPos += 2)
								{
									int value = (input[inPos + 1] << 8) + input[inPos];
									output[outPos++] = ((value & mask) > 0) ? ONE : ZERO;
								}
							}
						}
						else
						{
							byte mask = (byte) (1 << bitPosition);
							for (int inPos = 0; inPos < inLen; inPos++)
							{
								output[outPos++] = ((input[inPos] & mask) > 0) ? ONE : ZERO;
							}
						}

					}
				}
			}

			return extractedPixels;
		}

		/// <summary>
		/// Creates a packed overlay data object using the specified pixel data buffer.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsStored"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="highBit"></param>
		/// <param name="bigEndianWords"></param>
		/// <param name="pixelData"></param>
		/// <returns></returns>
		public static OverlayData CreateOverlayData(int rows, int columns, int bitsStored, int bitsAllocated, int highBit, bool bigEndianWords, byte[] pixelData)
		{
			int minBytesNeeded = (int) Math.Ceiling(rows*columns/8d);
			if (minBytesNeeded % 2 == 1)
				minBytesNeeded++;
			byte[] packedOverlayData = new byte[minBytesNeeded];
			uint mask = (uint) ((1 << bitsStored) - 1) << (bitsAllocated - highBit - 1);
			Pack(pixelData, packedOverlayData, 0, mask, bigEndianWords);
			return new OverlayData(rows, columns, bigEndianWords, packedOverlayData);
		}

		#region Private Bit Packing Code

		private static void Pack(byte[] unpackedBits, byte[] packedBits, int start, uint inputMask, bool bigEndianWords) 
		{
			if (bigEndianWords && packedBits.Length % 2 == 1)
				throw new ArgumentException("Output byte length must be even-length.", "packedBits");

			int length = packedBits.Length;
			int outPos = 0;
			int inLen = unpackedBits.Length;

			byte[] input = unpackedBits;
			{
				byte[] output = packedBits;
				{
					if(bigEndianWords)
					{
						ushort window = 0x00;
						ushort mask = 0x01;
						for (int inPos = 0; inPos < inLen; inPos++)
						{
							if ((input[inPos] & inputMask) > 0)
								window |= mask;

							mask = (ushort) (mask << 1);
							if (mask == 0)
							{
								output[outPos] = (byte) (window >> 8);
								output[outPos + 1] = (byte) (window);
								outPos += 2;
								window = 0x00;
								mask = 0x01;
							}
						}
						if (window > 0)
						{
							output[outPos] = (byte)(window >> 8);
							output[outPos + 1] = (byte)(window);
						}
					}
					else
					{
						byte window = 0x00;
						byte mask = 0x01;
						for (int inPos = 0; inPos < inLen; inPos++)
						{
							if ((input[inPos] & inputMask) > 0)
								window |= mask;

							mask = (byte) (mask << 1);
							if (mask == 0)
							{
								output[outPos] = window;
								outPos++;
								window = 0x00;
								mask = 0x01;
							}
						}
						if (window > 0)
							output[outPos] = window;
					}
				}
			}
		}

		private unsafe static void Unpack(byte[] packedBits, byte[] unpackedBits, int start, bool bigEndianWords)
		{
			const byte ONE = 0xff;
			const byte ZERO = 0x00;

			if (bigEndianWords && packedBits.Length % 2 == 1)
				throw new ArgumentException("Input byte array must be even-length.", "packedBits");

			int length = unpackedBits.Length;
			int outPos = 0;
			int inLen = packedBits.Length;

			fixed (byte* input = packedBits)
			{
				fixed (byte* output = unpackedBits)
				{
					if (bigEndianWords)
					{
						ushort initMask = (ushort) (1 << (start%16));
						for (int inPos = 2*(start/16); inPos < inLen; inPos += 2)
						{
							ushort window = (ushort) ((input[inPos] << 8) + input[inPos + 1]);
							for (ushort mask = initMask; mask > 0 && outPos < length; mask = (ushort) (mask << 1))
							{
								output[outPos++] = (window & mask) > 0 ? ONE : ZERO;
							}
							initMask = 0x01;
						}
					}
					else
					{
						byte initMask = (byte) (1 << start%8);
						for (int inPos = start/8; inPos < inLen; inPos++)
						{
							byte window = input[inPos];
							for (byte mask = initMask; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (window & mask) > 0 ? ONE : ZERO;
							}
							initMask = 0x01;
						}
					}
				}
			}
		}

		#region Unit Test Entry Points

#if UNIT_TESTS

		internal static void TestUnpack(byte[] packedBits, byte[] unpackedBits, int start, bool bigEndianWords)
		{
			Unpack(packedBits, unpackedBits, start, bigEndianWords);
		}

		internal static void TestPack()
		{
			//Pack()
		}

#endif

		#endregion

		#endregion
	}
}