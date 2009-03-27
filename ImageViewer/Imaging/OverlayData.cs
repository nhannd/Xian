using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class OverlayData
	{
		private int _rows;
		private int _columns;
		private bool _bigEndianWords;
		private byte[] _rawOverlayData;

		public OverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayData)
		{
			_rows = rows;
			_columns = columns;
			_bigEndianWords = bigEndianWords;
			_rawOverlayData = overlayData;
		}

		public byte[] Raw
		{
			get { return _rawOverlayData; }
		}

		/// <summary>
		/// Converts this OverlayData chunk into a PixelData chunk.
		/// </summary>
		/// <returns>A greyscale PixelData chunk containing 8-bit overlay data (1 bit stored).</returns>
		public GrayscalePixelData ToPixelData()
		{
			return new GrayscalePixelData(_rows, _columns, 8, 1, 1, false, Unpack(_rawOverlayData, _rows*_columns, _bigEndianWords));
		}

		/// <summary>
		/// Converts a PixelData chunk into an OverlayData chunk.
		/// </summary>
		/// <param name="pixelData">The pixel data to convert.</param>
		/// <returns>An OverlayData chunk containing packed 1-bit overlay data.</returns>
		public static OverlayData FromPixelData(PixelData pixelData)
		{
			throw new NotImplementedException("Conversion of PixelData to OverlayData has not been implemented yet.");
		}

		#region Private Bit Packing Code

		private static byte[] Pack(byte[] unpackedBits, int length, bool bigEndianWords) 
		{
			if (bigEndianWords && length % 2 == 1)
				throw new ArgumentException("Output byte length must be even-length.", "length");

			byte[] packedBits = new byte[length];
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
							if (input[inPos] > 0)
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
						if (mask > 0)
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
							if (input[inPos] > 0)
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
						if (mask > 0)
							output[outPos] = window;
					}
				}
			}

			return packedBits;
		}

		private unsafe static byte[] Unpack(byte[] packedBits, int length, bool bigEndianWords)
		{
			const byte ONE = 0x01;
			const byte ZERO = 0x00;

			if (bigEndianWords && packedBits.Length % 2 == 1)
				throw new ArgumentException("Input byte array must be even-length.", "packedBits");

			byte[] unpackedBits = new byte[length];
			int outPos = 0;
			int inLen = packedBits.Length;

			fixed (byte* input = packedBits)
			{
				fixed (byte* output = unpackedBits)
				{
					if (bigEndianWords)
					{
						for (int inPos = 0; inPos < inLen; inPos += 2)
						{
							// process the lower byte
							byte lowerWindow = input[inPos + 1];
							for (byte mask = 0x01; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (lowerWindow & mask) > 0 ? ONE : ZERO;
							}

							// process the upper byte
							byte upperWindow = input[inPos];
							for (byte mask = 0x01; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (upperWindow & mask) > 0 ? ONE : ZERO;
							}
						}
					}
					else
					{
						for (int inPos = 0; inPos < inLen; inPos++)
						{
							byte window = input[inPos];
							for (byte mask = 0x01; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (window & mask) > 0 ? ONE : ZERO;
							}
						}
					}
				}
			}

			return unpackedBits;
		}

		#endregion
	}
}