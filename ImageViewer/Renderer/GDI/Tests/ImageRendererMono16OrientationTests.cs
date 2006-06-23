#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Workstation.Renderer.GDI;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ImageRendererMono16OrientationTests
	{
		LayerGroup _layerGroup;
		MockImageLayer _layer;
		int _srcWidth, _srcHeight;
		int _dstWidth, _dstHeight;
		Rectangle _srcRect, _dstRect;
		ushort _leftTop, _rightTop, _leftBottom, _rightBottom;

		public ImageRendererMono16OrientationTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			CreateMonochrome16ImageLayer();
			CreatePhantom();
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void RenderOrientation1()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);
			
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightBottom, bitmap);
		}


		[Test]
		public void RenderOrientation2()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipVertical = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightTop, bitmap);
		}

		[Test]
		public void RenderOrientation3()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipHorizontal = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftBottom, bitmap);
		}

		[Test]
		public void RenderOrientation4()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipHorizontal = true;
			_layer.SpatialTransform.FlipVertical = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftTop, bitmap);
		}

		[Test]
		public void RenderOrientation5()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftBottom, bitmap);
		}

		[Test]
		public void RenderOrientation6()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90;
			_layer.SpatialTransform.FlipHorizontal = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightBottom, bitmap);
		}

		[Test]
		public void RenderOrientation7()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation
			_layer.SpatialTransform.FlipVertical = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftTop, bitmap);
		}

		[Test]
		public void RenderOrientation8()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation
			_layer.SpatialTransform.FlipVertical = true;
			_layer.SpatialTransform.FlipHorizontal = true;
			_layer.SpatialTransform.Calculate();

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Left, _dstRect.Bottom - 1, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyMonochromePixelValue16(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightTop, bitmap);
		}

		private void CreateMonochrome16ImageLayer()
		{
			_srcWidth = 10;
			_srcHeight = 10;
			_dstWidth = 23;
			_dstHeight = 23;

			_layer = ImageLayerFactory.CreateMonochrome16ImageLayer(_srcWidth, _srcHeight);

			_layerGroup = new LayerGroup();
			_layerGroup.Layers.Add(_layer);

			_srcRect = new Rectangle(0, 0, _srcWidth, _srcHeight);
			_dstRect = new Rectangle(0, 0, _dstWidth, _dstHeight);

			_layer.SpatialTransform.SourceRectangle = _srcRect;
			_layer.SpatialTransform.DestinationRectangle = _dstRect;
		}

		private void CreatePhantom()
		{
			// Test values.  The mock layer uses a linear LUT with width 65536 and center 32767,
			// which means that the 16-bit range of [0,65535] is mapped to the 8-bit range
			// of [0,255].  So, for example, 2560 in the 16-bit range maps to 10 in the 8-bit range.
			_leftTop = 2560 + 50; // 10 in 8-bit; we add 50 just to be sure we're in the right range
			_rightTop = 5120 + 50; // 20
			_leftBottom = 7680 + 50; //30
			_rightBottom = 10240 + 50; //40

			// Set a pixel with a different value at each corner of the image
			_layer.PixelDataWrapper.SetPixel16(_srcRect.Left, _srcRect.Top, _leftTop);
			_layer.PixelDataWrapper.SetPixel16(_srcRect.Right - 1, _srcRect.Top, _rightTop);
			_layer.PixelDataWrapper.SetPixel16(_srcRect.Left, _srcRect.Bottom - 1, _leftBottom);
			_layer.PixelDataWrapper.SetPixel16(_srcRect.Right - 1, _srcRect.Bottom - 1, _rightBottom);
		}
	}
}
#endif