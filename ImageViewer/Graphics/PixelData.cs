using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class PixelData
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsAllocated;
		private int _bitsStored;
		private int _highBit;
		private int _samplesPerPixel;
		private int _pixelRepresentation;
		private int _planarConfiguration;
		private PhotometricInterpretation _photometricInterpretation;
		private byte[] _pixelData;
		private int _bytesPerPixel;
		private int _planeOffset;
		private int _stride;

		#endregion

		public PixelData(ImageGraphic imageGraphic) :
			this(imageGraphic.Rows, 
				imageGraphic.Columns,
				imageGraphic.BitsAllocated,
				imageGraphic.BitsStored,
				imageGraphic.HighBit,
				imageGraphic.SamplesPerPixel,
				imageGraphic.PixelRepresentation,
				imageGraphic.PlanarConfiguration,
				imageGraphic.PhotometricInterpretation,
				imageGraphic.PixelData.Raw)
		{

		}

		public PixelData(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation,
			byte[] pixelData)
		{
			if (photometricInterpretation != PhotometricInterpretation.Monochrome1 &&
				photometricInterpretation != PhotometricInterpretation.Monochrome2 &&
				photometricInterpretation != PhotometricInterpretation.Rgb)
				throw new InvalidOperationException("Photometric Interpretation must be one of MONOCHROME1, MONOCHROME2 or RGB");

			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;
			_bitsStored = bitsStored;
			_highBit = highBit;
			_samplesPerPixel = samplesPerPixel;
			_pixelRepresentation = pixelRepresentation;
			_planarConfiguration = planarConfiguration;
			_photometricInterpretation = photometricInterpretation;
			_pixelData = pixelData;

			if (_photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				if (_planarConfiguration == 0)
					_bytesPerPixel = 3;
				else
					_bytesPerPixel = 1;
			}
			else
				_bytesPerPixel = _bitsAllocated / 8;

			if (_planarConfiguration == 0)
				_planeOffset = 1;
			else
				_planeOffset = _bytesPerPixel * _columns * _rows;

			_stride = _columns * _bytesPerPixel;
		}

		public byte[] Raw
		{
			get { return _pixelData; }
		}

		#region Public methods

		public int GetPixel(int x, int y)
		{
			if (_photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				return GetPixelRGBInternal(x, y);
			}
			else // MONOCHROME 1,2
			{
				if (_bytesPerPixel == 1) // 8 bit
				{
					if (_pixelRepresentation != 0) // Signed
						return (int)GetPixelSigned8(x, y);
					else // Unsigned
						return (int)GetPixelUnsigned8(x, y);
				}
				else // 16 bit
				{
					if (_pixelRepresentation != 0) // Signed
						return (int)GetPixelSigned16(x, y);
					else // Unsigned
						return (int)GetPixelUnsigned16(x, y);
				}
			}
		}

		public Color GetPixelRGB(int x, int y)
		{
			if (_photometricInterpretation != PhotometricInterpretation.Rgb)
				throw new Exception("Photometric Interpretation is not RGB");

			return Color.FromArgb(GetPixelRGBInternal(x, y));
		}

		public void SetPixel(int x, int y, int value)
		{
			if (_photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				SetPixelRGB(x, y, Color.FromArgb(value));
			}
			else // MONOCHROME 1,2
			{
				if (_bytesPerPixel == 1)
				{
					if (_pixelRepresentation != 0) // Signed
						SetPixelSigned8(x, y, value);
					else // Unsigned
						SetPixelUnsigned8(x, y, value);
				}
				else
				{
					if (_pixelRepresentation != 0) // Signed
						SetPixelSigned16(x, y, value);
					else // Unsigned
						SetPixelUnsigned16(x, y, value);
				}
			}
		}

		public void SetPixelRGB(int x, int y, Color color)
		{
			SetPixelRGB(x, y, color.R, color.G, color.B);
		}

		public void SetPixelRGB(int x, int y, byte r, byte g, byte b)
		{
			if (_photometricInterpretation != PhotometricInterpretation.Rgb)
				throw new InvalidOperationException("Photometric interpretation must be RGB");

			int i = GetIndex(x, y);

			_pixelData[i] = r;
			_pixelData[i + _planeOffset] = g;
			_pixelData[i + 2 * _planeOffset] = b;
		}

		#endregion

		#region Private methods
		#region Private get methods

		private byte GetPixelUnsigned8(int x, int y)
		{
			int i = GetIndex(x, y);
			return _pixelData[i];
		}

		private sbyte GetPixelSigned8(int x, int y)
		{
			int i = GetIndex(x, y);
			return (sbyte)_pixelData[i];
		}

		private ushort GetPixelUnsigned16(int x, int y)
		{
			int i = GetIndex(x, y);
			ushort lowbyte = (ushort)_pixelData[i];
			ushort highbyte = (ushort)_pixelData[i + 1];
			ushort pixelValue = (ushort)(lowbyte | (highbyte << 8));

			return pixelValue;
		}

		private short GetPixelSigned16(int x, int y)
		{
			return (short)GetPixelUnsigned16(x, y);
		}

		private int GetPixelRGBInternal(int x, int y)
		{
			int i = GetIndex(x, y);

			int a = 0xff;
			int r = (int)_pixelData[i];
			int g = (int)_pixelData[i + _planeOffset];
			int b = (int)_pixelData[i + 2 * _planeOffset];

			int argb = a << 24 | r << 16 | g << 8 | b;

			return argb;
		}

		#endregion

		#region Private set methods

		private void SetPixelUnsigned8(int x, int y, int value)
		{
			if (value < byte.MinValue || value > byte.MaxValue)
				throw new InvalidOperationException("Value must be 8-bit unsigned ");

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)value;
		}

		private void SetPixelSigned8(int x, int y, int value)
		{
			if (value < sbyte.MinValue || value > sbyte.MaxValue)
				throw new InvalidOperationException("Value must be 8-bit signed");

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)value;
		}

		private void SetPixelUnsigned16(int x, int y, int value)
		{
			if (value < ushort.MinValue || value > ushort.MaxValue)
				throw new InvalidOperationException("Value must be 16-bit unsigned");

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			_pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		private void SetPixelSigned16(int x, int y, int value)
		{
			if (value < short.MinValue || value > short.MaxValue)
				throw new InvalidOperationException("Value must be 16-bit signed");

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			_pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		#endregion

		private int GetIndex(int x, int y)
		{
			int i = (y * _stride) + (x * _bytesPerPixel);
			return i;
		}

		#endregion
	}
}
