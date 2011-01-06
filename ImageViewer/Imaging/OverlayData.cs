#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Represents DICOM overlay data.
	/// </summary>
	public class OverlayData
	{
		private readonly int _offset;
		private readonly int _rows;
		private readonly int _columns;
		private readonly bool _bigEndianWords;
		private readonly byte[] _rawOverlayData;

		/// <summary>
		/// Constructs a new overlay data object.
		/// </summary>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		/// <param name="bigEndianWords">A value indicating if the <paramref name="overlayData"/> is stored as 16-bit words with big endian byte ordering</param>
		/// <param name="overlayData">The packed bits overlay data buffer.</param>
		public OverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayData)
			: this(0, rows, columns, bigEndianWords, overlayData) {}

		/// <summary>
		/// Constructs a new overlay data object.
		/// </summary>
		/// <param name="offset">The initial offset of the first bit within the <paramref name="overlayData"/>.</param>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		/// <param name="bigEndianWords">A value indicating if the <paramref name="overlayData"/> is stored as 16-bit words with big endian byte ordering</param>
		/// <param name="overlayData">The packed bits overlay data buffer.</param>
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

		/// <summary>
		/// Gets the raw, packed bits overlay data buffer.
		/// </summary>
		public byte[] Raw
		{
			get { return _rawOverlayData; }
		}

		/// <summary>
		/// Unpacks the overlay data into an 8-bit overlay pixel data buffer.
		/// </summary>
		/// <returns>The unpacked, 8-bit overlay pixel data buffer.</returns>
		public byte[] Unpack()
		{
			byte[] unpackedPixelData = MemoryManager.Allocate<byte>(_rows * _columns);
			Unpack(_rawOverlayData, unpackedPixelData, _offset, _bigEndianWords);
			return unpackedPixelData;
		}

		/// <summary>
		/// Extracts an overlay embedded in a DICOM Pixel Data buffer and unpacks it to form an 8-bit overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method processes all pixel allocations in the pixel data buffer. If the pixel data contains mutliple image frames,
		/// the unpacked overlay data buffer will also be multi-frame (with the key difference that the overlay data will always
		/// be 8 bits per pixel, rather than potentially 16 bits per pixel).
		/// </para>
		/// <para>
		/// Embedded overlays were last defined in the 2004 DICOM Standard. Since then, their usage has been retired.
		/// As such, there is no mechanism to directly read or encode embedded overlays. This method may be used as a
		/// helper to extract overlays in images encoded with a previous version of the standard for display in compatibility
		/// mode or storage as packed bit data.
		/// </para>
		/// </remarks>
		/// <param name="bitPosition">The bit position the the overlay is embedded at within the DICOM Pixel Data buffer.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel. Must be 8 or 16.</param>
		/// <param name="bigEndianWords">A value indicating if the pixel data buffer is stored as 16-bit words with big endian byte ordering.</param>
		/// <param name="pixelData">The DICOM Pixel Data buffer containing an embedded overlay.</param>
		/// <returns>The unpacked, 8-bit overlay pixel data buffer.</returns>
		public static byte[] UnpackFromPixelData(int bitPosition, int bitsAllocated, bool bigEndianWords, byte[] pixelData)
		{
			//TODO (CR December 2010): sanity check on bitPosition vs bitsAllocated?
			const byte ONE = 0xff;
			const byte ZERO = 0x00;

			if(bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("BitsAllocated must be either 8 or 16 bits.", "bitsAllocated");

			int inLen = pixelData.Length;
			int outLen = inLen/(bitsAllocated/8);
			int outPos = 0;
			byte[] extractedPixels = MemoryManager.Allocate<byte>(outLen);

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

		//TODO (CR December 2010): The API of these 2 methods isn't entirely clear, especially this overload.
		//TODO (CR December 2010): there are no unit tests for this code even though our presentation state code uses it.

		/// <summary>
		/// Creates a packed overlay data object using the specified 8-bit pixel data buffer.
		/// </summary>
		/// <remarks>
		/// This function creates an overlay from the non-zero pixels of the specified pixel data.
		/// </remarks>
		/// <param name="rows">The number of rows of pixels.</param>
		/// <param name="columns">The number of columns of pixels.</param>
		/// <param name="bigEndianWords">A value indicating if the output overlay data should be encoded as 16-bit words with big endian byte ordering.</param>
		/// <param name="overlayPixelData">The pixel data buffer containing the overlay contents.</param>
		/// <returns>A packed overlay data object representing the overlay contents.</returns>
		public static OverlayData CreateOverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayPixelData)
		{
			return CreateOverlayData(rows, columns, 8, 8, 7, bigEndianWords, overlayPixelData);
		}

		/// <summary>
		/// Creates a packed overlay data object using the specified pixel data buffer.
		/// </summary>
		/// <remarks>
		/// This function creates an overlay from the non-zero pixels of the specified pixel data. Only the bits
		/// specified by <paramref name="bitsStored"/> and <paramref name="highBit"/> are considered.
		/// </remarks>
		/// <param name="rows">The number of rows of pixels.</param>
		/// <param name="columns">The number of columns of pixels.</param>
		/// <param name="bitsStored">The number of bits stored per pixel.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel. Must be 8 or 16.</param>
		/// <param name="highBit">The bit position of the most significant bit of a pixel.</param>
		/// <param name="bigEndianWords">A value indicating if the output overlay data should be encoded as 16-bit words with big endian byte ordering.</param>
		/// <param name="overlayPixelData">The pixel data buffer containing the overlay contents.</param>
		/// <returns>A packed overlay data object representing the overlay contents.</returns>
		public static OverlayData CreateOverlayData(int rows, int columns, int bitsStored, int bitsAllocated, int highBit, bool bigEndianWords, byte[] overlayPixelData)
		{
			int minBytesNeeded = (int) Math.Ceiling(rows*columns/8d);
			if (minBytesNeeded % 2 == 1)
				minBytesNeeded++;
			byte[] packedOverlayData = MemoryManager.Allocate<byte>(minBytesNeeded);

			uint mask = (uint) ((1 << bitsStored) - 1) << (bitsAllocated - highBit - 1);
			Pack(overlayPixelData, packedOverlayData, 0, mask, bigEndianWords);
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

						//TODO (CR December 2010): just do this unconditionally?  What if the passed in buffer
						//weren't initialized to zeros?
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
						//TODO (CR December 2010): just do this unconditionally?  What if the passed in buffer
						//weren't initialized to zeros?
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