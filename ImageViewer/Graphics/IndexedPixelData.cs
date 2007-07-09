using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class IndexedPixelData : PixelData
	{
		#region Private fields

		private bool _isSigned;
		private int _bitsStored;
		private int _highBit;
		private int _absoluteMinPixelValue;
		private int _absoluteMaxPixelValue;

		#endregion

		#region Public constructor

		public IndexedPixelData(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			byte[] pixelData)
			: base(rows, columns, bitsAllocated, pixelData)
		{
			ImageValidator.ValidateBitsStored(bitsStored);
			ImageValidator.ValidateHighBit(highBit);

			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;

			CalculateAbsolutePixelValueRange();
		}

		#endregion

		#region Public properties

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

		#endregion

		#region Public methods

		public override PixelData Clone()
		{
			return new IndexedPixelData(
				_rows,
				_columns,
				_bitsAllocated,
				_bitsStored,
				_highBit,
				_isSigned,
				(byte[])_pixelData.Clone());
		}

		#region GetPixel methods

		/// <summary>
		/// Gets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>
		/// The value of the pixel.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public override int GetPixel(int x, int y)
		{
			int i = GetIndex(x, y);
			return GetPixelInternal(i);
		}

		public override int GetPixel(int pixelIndex)
		{
			int i = pixelIndex * _bytesPerPixel;
			return GetPixelInternal(i);
		}

		#endregion

		#region SetPixel methods

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
		public override void SetPixel(int x, int y, int value)
		{
			int i = GetIndex(x,y);
			SetPixelInternal(i, value);
		}

		public override void SetPixel(int pixelIndex, int value)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, value);
		}

		#endregion

		/// <summary>
		/// Calculates the Minimum and Maximum pixel values from the pixel data efficiently, using unsafe code.
		/// </summary>
		/// <param name="minPixelValue">returns the minimum pixel value</param>
		/// <param name="maxPixelValue">returns the maximum pixel value</param>
		unsafe public void CalculateMinMaxPixelValue(out int minPixelValue, out int maxPixelValue)
		{
#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			if (_isSigned)
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

		#endregion

		#region Private get methods

		private int GetPixelInternal(int i)
		{
			if (_bytesPerPixel == 1) // 8 bit
			{
				if (_isSigned)
					return (int)GetPixelSigned8(i);
				else
					return (int)GetPixelUnsigned8(i);
			}
			else // 16 bit
			{
				if (_isSigned)
					return (int)GetPixelSigned16(i);
				else
					return (int)GetPixelUnsigned16(i);
			}
		}

		private byte GetPixelUnsigned8(int i)
		{
			return _pixelData[i];
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

		private ushort GetPixelUnsigned16(int i)
		{
			ushort lowbyte = (ushort)_pixelData[i];
			ushort highbyte = (ushort)_pixelData[i + 1];
			ushort pixelValue = (ushort)(lowbyte | (highbyte << 8));

			return pixelValue;
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


		#endregion

		#region Private set methods

		private void SetPixelInternal(int i, int value)
		{
			if (_bytesPerPixel == 1)
			{
				if (_isSigned)
					SetPixelSigned8(i, value);
				else
					SetPixelUnsigned8(i, value);
			}
			else
			{
				if (_isSigned)
					SetPixelSigned16(i, value);
				else
					SetPixelUnsigned16(i, value);
			}
		}

		private void SetPixelUnsigned8(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			_pixelData[i] = (byte)value;
		}


		private void SetPixelSigned8(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel8(i, (byte)value);
			else
			{
				byte raw = (byte)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
				byte maskedRaw = (byte)(raw & mask);
				SetPixel8(i, maskedRaw);
			}
		}

		private void SetPixel8(int i, byte value)
		{
			_pixelData[i] = value;
		}

		private void SetPixelUnsigned16(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			SetPixel16(i, (ushort)value);
		}

		private void SetPixelSigned16(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel16(i, (ushort)value);
			else
			{
				ushort raw = (ushort)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				ushort mask = (ushort)(ushort.MaxValue >> (_bitsAllocated - _bitsStored));
				ushort maskedRaw = (ushort)(raw & mask);
				SetPixel16(i, maskedRaw);
			}
		}

		private void SetPixel16(int i, ushort value)
		{
			_pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			_pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		#endregion

		#region Other private methods

		private void CalculateAbsolutePixelValueRange()
		{
			if (_isSigned)
			{
				_absoluteMinPixelValue = -(1 << (_bitsStored - 1));
				_absoluteMaxPixelValue = (1 << (_bitsStored - 1)) - 1;
			}
			else
			{
				_absoluteMinPixelValue = 0;
				_absoluteMaxPixelValue = (1 << _bitsStored) - 1;
			}
		}

		#endregion
	}
}
