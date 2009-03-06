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
		/// <returns></returns>
		public GrayscalePixelData ToPixelData()
		{
			return new GrayscalePixelData(_rows, _columns, 8, 1, 1, false, Unpack(_rawOverlayData, _rows*_columns, _bigEndianWords));
		}

		//public static OverlayData FromPixelData(PixelData pixelData)
		//{

		//}

		#region Private Bit Packing Code

		private unsafe static byte[] Unpack(byte[] packedBits, int length, bool bigEndianWords)
		{
			const byte ONE = 0xFF;
			const byte ZERO = 0x00;

			byte[] unpackedBits = new byte[length];
			int outPos = 0;
			int inLen = packedBits.Length;

			fixed (byte* input = packedBits) // byte[] input = packedBits;
			{
				fixed (byte* output = unpackedBits) //byte[] output = unpackedBits;
				{
					byte window;
					if (bigEndianWords)
					{
						for (int inPos = 0; inPos < inLen; inPos += 2)
						{
							// process the lower byte
							window = input[inPos + 1];
							for (byte mask = 0x01; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (window & mask) > 0 ? ONE : ZERO;
							}

							// process the upper byte
							window = input[inPos];
							for (byte mask = 0x01; mask > 0 && outPos < length; mask = (byte) (mask << 1))
							{
								output[outPos++] = (window & mask) > 0 ? ONE : ZERO;
							}
						}
					}
					else
					{
						for (int inPos = 0; inPos < inLen; inPos++)
						{
							window = input[inPos];
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