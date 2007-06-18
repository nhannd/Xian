using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A pixel data wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="PixelData"/> provides a number of convenience methods
	/// to make accessing and changing pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="SetPixel"/> is not recommended if you want
	/// good performance.  Instead, use the <see cref="Raw"/> property 
	/// to get the raw byte array, then use unsafe code to do your processing.
	/// </remarks>
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
		private int _absoluteMinPixelValue;
		private int _absoluteMaxPixelValue;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the
		/// specified <see cref="ImageGraphic"/>.
		/// </summary>
		/// <param name="imageGraphic"></param>
		/// <remarks>
		/// This constructor is provided for convenience so that the
		/// pixel data in an <see cref="ImageGraphic"/> can be easily wrapped.
		/// Note that a reference to <paramref name="imageGraphic"/> is <i>not</i> held
		/// by <see cref="PixelData"/>.
		/// </remarks>
		public PixelData(ImageGraphic imageGraphic)
			:
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

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the
		/// specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="samplesPerPixel"></param>
		/// <param name="pixelRepresentation"></param>
		/// <param name="planarConfiguration"></param>
		/// <param name="photometricInterpretation"></param>
		/// <param name="pixelData"></param>
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

			CalculateBytesPerPixel();
			CalculatePlaneOffset();
			CalculateStride();
			CalculateAbsolutePixelValueRange();
		}

		/// <summary>
		/// Gets the raw pixel data.
		/// </summary>
		public byte[] Raw
		{
			get { return _pixelData; }
		}

		/// <summary>
		/// Returns the absolute minimum possible pixel value for the image based on PixelRepresentation and BitsAllocated.
		/// </summary>
		public int AbsoluteMinPixelValue
		{
			get { return _absoluteMinPixelValue; }
		}

		/// <summary>
		/// Returns the absolute maximum possible pixel value for the image based on PixelRepresentation and BitsAllocated.
		/// </summary>
		public int AbsoluteMaxPixelValue
		{
			get { return _absoluteMaxPixelValue; }
		}

		#region Public methods

		public PixelData Clone()
		{
			return new PixelData(
				_rows,
				_columns,
				_bitsAllocated,
				_bitsStored,
				_highBit,
				_samplesPerPixel,
				_pixelRepresentation,
				_planarConfiguration,
				_photometricInterpretation,
				(byte[])_pixelData.Clone());
		}

		/// <summary>
		/// Gets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>
		/// The value of the pixel.  In the case where the photometric interpretation
		/// is RGB, an ARGB value is returned.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
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

		public int GetPixel(int i)
		{
			if (_photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				return GetPixelRGBInternal(i);
			}
			else // MONOCHROME 1,2
			{
				if (_bytesPerPixel == 1) // 8 bit
				{
					if (_pixelRepresentation != 0) // Signed
						return (int)GetPixelSigned8(i);
					else // Unsigned
						return (int)GetPixelUnsigned8(i);
				}
				else // 16 bit
				{
					if (_pixelRepresentation != 0) // Signed
						return (int)GetPixelSigned16(i * _bytesPerPixel);
					else // Unsigned
						return (int)GetPixelUnsigned16(i * _bytesPerPixel);
				}
			}
		}

		/// <summary>
		/// Gets the RGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// <see cref="GetPixelRGB"/> only works with images whose
		/// photometric interpretation is RGB.
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		/// <exception cref="InvalidOperationException">The photometric
		/// interpretation is not RGB.</exception>
		public Color GetPixelRGB(int x, int y)
		{
			if (_photometricInterpretation != PhotometricInterpretation.Rgb)
				throw new InvalidOperationException("Photometric Interpretation is not RGB");

			return Color.FromArgb(GetPixelRGBInternal(x, y));
		}

		/// <summary>
		/// Sets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="value"></param>
		/// <remarks>
		/// Allowable pixel values are determined by the pixel representation
		/// and the number of bits stored.
		/// represent
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds, or <paramref name="value"/>
		/// is out of range.</exception>
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

		/// <summary>
		/// Sets the RGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public void SetPixelRGB(int x, int y, Color color)
		{
			SetPixelRGB(x, y, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sets the RGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
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

		private byte GetPixelUnsigned8(int i)
		{
			return _pixelData[i];
		}

		private sbyte GetPixelSigned8(int x, int y)
		{
			byte raw = GetPixelUnsigned8(x, y);
			sbyte result = ConvertToSigned8(raw);

			return result; 
		}

		private sbyte GetPixelSigned8(int i)
		{
			byte raw = GetPixelUnsigned8(i);
			sbyte result = ConvertToSigned8(raw);
			
			return result; 
		}

		private sbyte ConvertToSigned8(byte raw)
		{
			// Create a mask that will pick out the sign bit, which is the high bit
			byte signMask = (byte)(1 << (_bitsStored - 1));
			sbyte result;

			// If the sign bit is 0, then just return the raw value,
			// since it's like an unsigned value
			if ((raw & signMask) == 0)
			{
				result = (sbyte)raw;
			}
			// If the sign bit is 1, then the value is in 2's complement, which
			// means we have to compute the corresponding positive number by
			// inverting the bits and adding 1
			else
			{
				byte inverted = (byte)(~raw);
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
				byte maskedInverted = (byte)(inverted & mask);
				result = (sbyte)(-(maskedInverted + 1));
			}

			return result;
		}

		private ushort GetPixelUnsigned16(int x, int y)
		{
			int i = GetIndex(x, y);
			return GetPixelUnsigned16(i);
		}

		private ushort GetPixelUnsigned16(int i)
		{
			ushort lowbyte = (ushort)_pixelData[i];
			ushort highbyte = (ushort)_pixelData[i + 1];
			ushort pixelValue = (ushort)(lowbyte | (highbyte << 8));

			return pixelValue;
		}

		private short GetPixelSigned16(int x, int y)
		{
			ushort raw = GetPixelUnsigned16(x, y);
			short result = ConvertToSigned16(raw);

			return result;
		}

		private short GetPixelSigned16(int i)
		{
			ushort raw = GetPixelUnsigned16(i);
			short result = ConvertToSigned16(raw);

			return result;
		}

		private short ConvertToSigned16(ushort raw)
		{
			// Create a mask that will pick out the sign bit, which is the high bit
			ushort signMask = (ushort)(1 << (_bitsStored - 1));
			short result;

			// If the sign bit is 0, then just return the raw value,
			// since it's like an unsigned value
			if ((raw & signMask) == 0)
			{
				result = (short)raw;
			}
			// If the sign bit is 1, then the value is in 2's complement, which
			// means we have to compute the corresponding positive number by
			// inverting the bits and adding 1
			else
			{
				ushort inverted = (ushort)(~raw);
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				ushort mask = (ushort)(ushort.MaxValue >> (_bitsAllocated - _bitsStored));
				ushort maskedInverted = (ushort)(inverted & mask);
				result = (short)(-(maskedInverted + 1));
			}

			return result;
		}


		private int GetPixelRGBInternal(int x, int y)
		{
			int i = GetIndex(x, y);
			int argb = GetPixelRGBInternal(i);

			return argb;
		}

		private int GetPixelRGBInternal(int i)
		{
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
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)value;
		}

		private void SetPixelSigned8(int x, int y, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel8(x, y, (byte)value);
			else
			{
				byte raw = (byte)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
				byte maskedRaw = (byte)(raw & mask);
				SetPixel8(x, y, maskedRaw);
			}
		}

		private void SetPixel8(int x, int y, byte value)
		{
			int i = GetIndex(x, y);
			_pixelData[i] = value;
		}

		private void SetPixelUnsigned16(int x, int y, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			SetPixel16(x, y, (ushort) value);
		}

		private void SetPixelSigned16(int x, int y, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel16(x, y, (ushort) value);
			else
			{
				ushort raw = (ushort)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				ushort mask = (ushort)(ushort.MaxValue >> (_bitsAllocated - _bitsStored));
				ushort maskedRaw = (ushort)(raw & mask);
				SetPixel16(x, y, maskedRaw);
			}
		}

		private void SetPixel16(int x, int y, ushort value)
		{
			int i = GetIndex(x, y);
			_pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			_pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		#endregion

		/// <summary>
		/// Calculates the Minimum and Maximum pixel values from the pixel data efficiently, using unsafe code.
		/// </summary>
		/// <param name="minPixelValue">returns the minimum pixel value</param>
		/// <param name="maxPixelValue">returns the maximum pixel value</param>
		unsafe public void CalculateMinMaxPixelValue(out int minPixelValue, out int maxPixelValue)
		{
			if (_photometricInterpretation != PhotometricInterpretation.Monochrome1 && _photometricInterpretation != PhotometricInterpretation.Monochrome2)
				throw new InvalidOperationException("Photometric Interpretation must be one of MONOCHROME1 or MONOCHROME2");

#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			if (_pixelRepresentation != 0)
			{
				if (_bitsAllocated == 8)
				{
					fixed (byte* ptr = _pixelData)
					{
						byte* pixel = (byte*)ptr;

						byte signMask = (byte)(1 << (_bitsStored - 1));
						
						sbyte max, min;
						max = sbyte.MinValue;
						min = sbyte.MaxValue;
						
						for (int i = 0; i < _rows * _columns; ++i)
						{
							sbyte result;
							if (0 == ((*pixel) & signMask))
							{
								result = (sbyte)(*pixel);
							}
							else
							{
								byte inverted = (byte)(~(*pixel));
								// Need to mask out the bits greater above the high bit, since they're irrelevant
								byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
								byte maskedInverted = (byte)(inverted & mask);
								result = (sbyte)(-(maskedInverted + 1));
							}

							if (result > max)
								max = result;
							else if (result < min)
								min = result;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
				else
				{
					fixed (byte* ptr = _pixelData)
					{
						UInt16* pixel = (UInt16*)ptr;

						UInt16 signMask = (UInt16)(1 << (_bitsStored - 1));

						Int16 max, min;
						max = Int16.MinValue;
						min = Int16.MaxValue;

						for (int i = 0; i < _rows * _columns; ++i)
						{
							Int16 result;
							if (0 == ((*pixel) & signMask))
							{
								result = (Int16)(*pixel);
							}
							else
							{
								UInt16 inverted = (UInt16)(~(*pixel));
								// Need to mask out the bits greater above the high bit, since they're irrelevant
								UInt16 mask = (UInt16)(UInt16.MaxValue >> (_bitsAllocated - _bitsStored));
								UInt16 maskedInverted = (UInt16)(inverted & mask);
								result = (Int16)(-(maskedInverted + 1));
							}

							if (result > max)
								max = result;
							else if (result < min)
								min = result;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
			}
			else
			{
				if (_bitsAllocated == 8)
				{
					fixed (byte* ptr = _pixelData)
					{
						byte* pixel = ptr;

						byte max, min;
						max = min = *pixel;

						for (int i = 1; i < _rows * _columns; ++i)
						{
							if (*pixel > max)
								max = *pixel;
							else if (*pixel < min)
								min = *pixel;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
				else
				{
					fixed (byte* ptr = _pixelData)
					{
						UInt16* pixel = (UInt16*)ptr;

						UInt16 max, min;
						max = min = *pixel;

						for (int i = 1; i < _rows * _columns; ++i)
						{
							if (*pixel > max)
								max = *pixel;
							else if (*pixel < min)
								min = *pixel;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
			}

#if DEBUG
			clock.Stop();
			Trace.WriteLine(String.Format("Min/Max pixel value calculation took {0:F3} seconds (rows = {1}, columns = {2})", clock.Seconds, _rows, _columns));
#endif
		}

		public delegate void PixelProcessor(int i, int x, int y, int pixelIndex);

		public void ForEachPixel(PixelProcessor processor)
		{
			ForEachPixel(0, 0, _columns - 1, _rows - 1, processor);
		}

		public void ForEachPixel(
			int left, int top, int right, int bottom, 
			PixelProcessor processor)
		{
			int i = 0;
			int temp;

			if (top > bottom)
			{
				temp = top;
				top = bottom;
				bottom = temp;
			}

			if (left > right)
			{
				temp = left;
				left = right;
				right = temp;
			}

			int pixelIndex = top * _columns + left;

			for (int y = top; y <= bottom; y++)
			{
				for (int x = left; x <= right; x++)
				{
					processor(i, x, y, pixelIndex);
					pixelIndex++;
					i++;
				}

				pixelIndex += (_columns - right) + left - 1;
			}
		}

		private void CalculateAbsolutePixelValueRange()
		{
			if (_pixelRepresentation == 0)
			{
				_absoluteMinPixelValue = 0;
				_absoluteMaxPixelValue = (1 << _bitsStored) - 1;
			}
			else
			{
				_absoluteMinPixelValue = -(1 << (_bitsStored - 1));
				_absoluteMaxPixelValue = (1 << (_bitsStored - 1)) - 1;
			}
		}

		private void CalculateStride()
		{
			_stride = _columns * _bytesPerPixel;
		}

		private void CalculatePlaneOffset()
		{
			if (_planarConfiguration == 0)
				_planeOffset = 1;
			else
				_planeOffset = _bytesPerPixel * _columns * _rows;
		}

		private void CalculateBytesPerPixel()
		{
			if (_photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				if (_planarConfiguration == 0)
					_bytesPerPixel = 3;
				else
					_bytesPerPixel = 1;
			}
			else
				_bytesPerPixel = _bitsAllocated / 8;
		}

		private int GetIndex(int x, int y)
		{
			if (x < 0 ||
				x >= _columns ||
				y < 0 ||
				y >= _rows)
				throw new ArgumentException("x and/or y are out of bounds");

			int i = (y * _stride) + (x * _bytesPerPixel);
			return i;
		}

		#endregion
	}
}
