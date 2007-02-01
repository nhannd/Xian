#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
	class MockImageGraphic : ImageGraphic
	{
		int _width;
		int _height;
		int _bitsAllocated;
		int _bitsStored;
		int _highBit;
		int _samplesPerPixel;
		int _pixelRepresentation;
		int _planarConfiguration;
		PhotometricInterpretation _photometricInterpretation;
		byte[] _pixelData;
		PixelDataWrapper _pixelDataWrapper;
		private GrayscaleLUTPipeline _grayscaleLUTPipeline;

		public MockImageGraphic(
			int width,
			int height,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation)
		{
			_width = width;
			_height = height;
			_bitsAllocated = bitsAllocated;
			_bitsStored = bitsStored;
			_highBit = highBit;
			_samplesPerPixel = samplesPerPixel;
			_pixelRepresentation = pixelRepresentation;
			_planarConfiguration = planarConfiguration;
			_photometricInterpretation = photometricInterpretation;

			int imageSize = width * height * bitsAllocated / 8 * samplesPerPixel;

			_pixelData = new byte[imageSize];

			_pixelDataWrapper = new PixelDataWrapper(
				_width,
				_height,
				_bitsAllocated,
				_bitsStored,
				_highBit,
				_samplesPerPixel,
				_pixelRepresentation,
				_planarConfiguration,
				_photometricInterpretation,
				_pixelData);

			InstallLut();
		}

		public override int  Rows
		{
			get { return _height; }
		}

		public override int  Columns
		{
			get { return _width; }
		}

		public override int  BitsAllocated
		{
			get { return _bitsAllocated; }
		}

		public override int  BitsStored
		{
			get { return _bitsStored; }
		}

		public override int  HighBit
		{
			get { return _highBit; }
		}

		public override int  SamplesPerPixel
		{
			get { return _samplesPerPixel; }
		}

		public override int  PixelRepresentation
		{
			get { return _pixelRepresentation; }
		}

		public override int  PlanarConfiguration
		{
			get { return _planarConfiguration; }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get { return _photometricInterpretation; }
		}

		public GrayscaleLUTPipeline GrayscaleLUTPipeline
		{
			get
			{
				if (this.IsColor)
					return null;

				if (_grayscaleLUTPipeline == null)
					_grayscaleLUTPipeline = new GrayscaleLUTPipeline();

				return _grayscaleLUTPipeline;
			}
		}

		public override byte[]  GetPixelData()
		{
			return _pixelData;
		}

		public override byte[] GetGrayscaleLUT()
		{
			if (this.GrayscaleLUTPipeline == null)
				throw new Exception(SR.ExceptionImageIsNotGrayscale);

			return this.GrayscaleLUTPipeline.OutputLUT;
		}

		public PixelDataWrapper PixelDataWrapper
		{
			get { return _pixelDataWrapper; }
		}

		private void InstallLut()
		{
			if (this.IsColor)
				return;

			double rescaleSlope = 1.0;
			double rescaleIntercept = 0.0;

			ModalityLUTLinear modalityLUT =
				new ModalityLUTLinear(
				this.BitsStored,
				this.PixelRepresentation,
				rescaleSlope,
				rescaleIntercept);

			this.GrayscaleLUTPipeline.ModalityLUT = modalityLUT;
			VOILUTLinear linearLut = new VOILUTLinear(modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue);
			linearLut.WindowWidth = 1 << this.BitsStored;
			linearLut.WindowCenter = linearLut.WindowWidth / 2;
			this.GrayscaleLUTPipeline.VoiLUT = linearLut;
			this.GrayscaleLUTPipeline.Execute();
		}

		public override bool HitTest(Point point)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Move(SizeF delta)
		{
			throw new Exception("The method or operation is not implemented.");
		}

	}

	static class ImageLayerFactory
	{
		public static MockImageGraphic CreateMonochrome8ImageLayer(int width, int height)
		{
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;
			
			return new MockImageGraphic(
				width, 
				height, 
				bitsAllocated,
				bitsStored,
				highBit, 
				samplesPerPixel, 
				pixelRepresentation, 
				planarConfiguration, 
				photometricInterpretation);
		}

		public static MockImageGraphic CreateMonochrome16ImageLayer(int width, int height)
		{
			int bitsAllocated = 16;
			int bitsStored = 16;
			int highBit = 15;
			int samplesPerPixel = 1;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;

			return new MockImageGraphic(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation);
		}

		public static MockImageGraphic CreateRGBTripletImageLayer(int width, int height)
		{
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 3;
			int pixelRepresentation = 0;
			int planarConfiguration = 0;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Rgb;

			return new MockImageGraphic(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation);
		}

		public static MockImageGraphic CreateRGBPlanarImageLayer(int width, int height)
		{
			int bitsAllocated = 8;
			int bitsStored = 8;
			int highBit = 7;
			int samplesPerPixel = 3;
			int pixelRepresentation = 0;
			int planarConfiguration = 1;
			PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Rgb;

			return new MockImageGraphic(
				width,
				height,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation);
		}
	}


	static class ImageRendererTestUtilities
	{
		public static Bitmap RenderLayer(MockImageGraphic layer, int dstWidth, int dstHeight)
		{
			Bitmap bitmap = new Bitmap(dstWidth, dstHeight);
			RectangleF clientArea = new RectangleF(0, 0, dstWidth, dstHeight);

			BitmapData bitmapData = LockBitmap(bitmap);
			int bytesPerPixel = 4;
			ImageRenderer.Render(layer, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public static void VerifyMonochromePixelValue16(int x, int y, int expectedPixelValue16, Bitmap bitmap)
		{
			int expectedPixelValue8 = expectedPixelValue16 / 256;

			VerifyMonochromePixelValue8(x, y, expectedPixelValue8, bitmap);
		}

		public static void VerifyMonochromePixelValue8(int x, int y, int expectedPixelValue8, Bitmap bitmap)
		{
			Color expectedPixelColor = Color.FromArgb(expectedPixelValue8, expectedPixelValue8, expectedPixelValue8);

			VerifyRGBPixelValue(x, y, expectedPixelColor, bitmap);
		}

		public static void VerifyRGBPixelValue(int x, int y, Color expectedPixelColor, Bitmap bitmap)
		{
			Color actualPixelColor = bitmap.GetPixel(x, y);
			Assert.AreEqual(expectedPixelColor, actualPixelColor);
		}

		private static BitmapData LockBitmap(Bitmap bitmap)
		{
			BitmapData bitmapData = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadWrite,
				bitmap.PixelFormat);

			return bitmapData;
		}

	}
}
#endif