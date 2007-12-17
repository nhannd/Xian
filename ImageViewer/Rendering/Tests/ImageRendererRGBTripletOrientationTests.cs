#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#if	UNIT_TESTS

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
/*	[TestFixture]
	public class ImageRendererRGBTripletOrientationTests
	{
		CompositeGraphic _layerGroup;
		MockImageGraphic _layer;
		int _srcWidth, _srcHeight;
		int _dstWidth, _dstHeight;
		Rectangle _srcRect, _dstRect;
		Color _leftTop, _rightTop, _leftBottom, _rightBottom;

		public ImageRendererRGBTripletOrientationTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			CreateRGBTripletImageLayer();
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

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightBottom, bitmap);
		}

		[Test]
		public void RenderOrientation2()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipVertical = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightTop, bitmap);
		}

		[Test]
		public void RenderOrientation3()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipHorizontal = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftBottom, bitmap);
		}

		[Test]
		public void RenderOrientation4()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.FlipHorizontal = true;
			_layer.SpatialTransform.FlipVertical = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftTop, bitmap);
		}

		[Test]
		public void RenderOrientation5()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftBottom, bitmap);
		}

		[Test]
		public void RenderOrientation6()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90;
			_layer.SpatialTransform.FlipHorizontal = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightBottom, bitmap);
		}

		[Test]
		public void RenderOrientation7()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation
			_layer.SpatialTransform.FlipVertical = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _rightTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _leftTop, bitmap);
		}

		[Test]
		public void RenderOrientation8()
		{
			_layer.SpatialTransform.Initialize();
			_layer.SpatialTransform.Rotation = -90; // CCW rotation
			_layer.SpatialTransform.FlipVertical = true;
			_layer.SpatialTransform.FlipHorizontal = true;

			Bitmap bitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);

			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Top, _leftBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Top, _leftTop, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Left, _dstRect.Bottom - 1, _rightBottom, bitmap);
			ImageRendererTestUtilities.VerifyRGBPixelValue(_dstRect.Right - 1, _dstRect.Bottom - 1, _rightTop, bitmap);
		}

		private void CreateRGBTripletImageLayer()
		{
			_srcWidth = 10;
			_srcHeight = 10;
			_dstWidth = 23;
			_dstHeight = 23;

			_layer = ImageLayerFactory.CreateRGBTripletImageLayer(_srcWidth, _srcHeight);

			_layerGroup = new CompositeGraphic();
			_layerGroup.Graphics.Add(_layer);

			_srcRect = new Rectangle(0, 0, _srcWidth, _srcHeight);
			_dstRect = new Rectangle(0, 0, _dstWidth, _dstHeight);

			_layer.SpatialTransform.SourceRectangle = _srcRect;
			_layer.SpatialTransform.DestinationRectangle = _dstRect;

			_layer.NormalInterpolationMethod = ImageGraphic.InterpolationMethods.NearestNeighbour;
		}

		private void CreatePhantom()
		{
			_leftTop = Color.FromArgb(10, 20, 30);
			_rightTop = Color.FromArgb(40, 50, 60);
			_leftBottom = Color.FromArgb(70, 80, 90);
			_rightBottom = Color.FromArgb(100, 110, 120);

			// Set a pixel with a different value at each corner of the image
			_layer.PixelDataWrapper.SetPixelRGB(_srcRect.Left, _srcRect.Top, _leftTop);
			_layer.PixelDataWrapper.SetPixelRGB(_srcRect.Right - 1, _srcRect.Top, _rightTop);
			_layer.PixelDataWrapper.SetPixelRGB(_srcRect.Left, _srcRect.Bottom - 1, _leftBottom);
			_layer.PixelDataWrapper.SetPixelRGB(_srcRect.Right - 1, _srcRect.Bottom - 1, _rightBottom);
		}
	}
*/}
#endif