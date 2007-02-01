using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class PixelDataWrapper
	{
		private int _height;
		private int _width;
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

		public PixelDataWrapper(
			int width,
			int height,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation,
			byte[] pixelData)
		{
			_height = height;
			_width = width;
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
				_planeOffset = _bytesPerPixel * _width * _height;
		}

		public byte[] GetPixelData()
		{
			return _pixelData;
		}

		public byte GetPixel8(int x, int y)
		{
			int i = GetIndex(x, y);
			return _pixelData[i];
		}

		public ushort GetPixel16(int x, int y)
		{
			int i = GetIndex(x, y);
			ushort lowbyte = (ushort)_pixelData[i];
			ushort highbyte = (ushort)_pixelData[i + 1];
			ushort pixelValue = (ushort)(lowbyte | (highbyte << 8));

			return pixelValue;
		}

		public Color GetPixelRGB(int x, int y)
		{
			int i = GetIndex(x, y);

			byte r = _pixelData[i];
			byte g = _pixelData[i + _planeOffset];
			byte b = _pixelData[i + 2 * _planeOffset];

			return Color.FromArgb(r, g, b);
		}

		public void SetPixel8(int x, int y, byte value)
		{
			if (_bitsAllocated != 8)
				throw new InvalidOperationException();

			int i = GetIndex(x, y);
			_pixelData[i] = value;
		}

		public void SetPixel16(int x, int y, ushort value)
		{
			if (_bitsAllocated != 16)
				throw new InvalidOperationException();

			int i = GetIndex(x, y);
			_pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			_pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		public void SetPixelRGB(int x, int y, Color rgb)
		{
			SetPixelRGB(x, y, rgb.R, rgb.G, rgb.B);
		}

		public void SetPixelRGB(int x, int y, byte r, byte g, byte b)
		{
			if (_photometricInterpretation != PhotometricInterpretation.Rgb)
				throw new InvalidOperationException();

			int i = GetIndex(x, y);

			_pixelData[i] = r;
			_pixelData[i + _planeOffset] = g;
			_pixelData[i + 2 * _planeOffset] = b;
		}

		private int GetIndex(int x, int y)
		{
			int stride = _width * _bytesPerPixel;
			int i = (y * stride) + (x * _bytesPerPixel);
			return i;
		}
	}
}
