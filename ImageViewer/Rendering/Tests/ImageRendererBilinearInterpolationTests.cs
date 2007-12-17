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
/*	public unsafe class PublicMethodRenderer : ImageRenderer
	{
		new public static bool IsRotated(ImageGraphic imageLayer)
		{
			return ImageRenderer.IsRotated(imageLayer);
		}

		new public static void CalculateVisibleRectangles(
			ImageGraphic imageLayer,
			RectangleF clientRectangle,
			out Rectangle dstVisibleRectangle,
			out Rectangle srcVisibleRectangle)
		{
			ImageRenderer.CalculateVisibleRectangles(imageLayer,
				clientRectangle,
				out dstVisibleRectangle,
				out srcVisibleRectangle);
		}
	}

	[TestFixture]
    public class ImageRendererBilinearInterpolationTests
    {
        enum ImageTypes { Mono8, Mono16, RgbPlanar, RgbTriplet};

		ImageGraphic.InterpolationMethods _interpolationMethod; 
        CompositeGraphic _layerGroup;
        MockImageGraphic _layer;

        int _srcWidth, _srcHeight;
        int _dstWidth, _dstHeight;
        bool _flipHorizontal;
        bool _flipVertical;
        int _rotation;
        float _scale;
        bool _scaleToFit;
        int _translationX;
        int _translationY;

        bool _trace = false;
        bool _tracePhantom = false;
        bool _traceBitmap = false;

        static readonly float _oneQuarter = 0.25F;
        static readonly float _threeQuarters = 0.75F;

        ImageTypes _sourceImageType;

		static readonly float _fixedScale = 128;
		static readonly int _fixedPrecision = 7;

        public ImageRendererBilinearInterpolationTests()
        {
        }

        [TestFixtureSetUp]
        public void Init()
        {
			//_tracePhantom = true;
			//_traceBitmap = true;
			//_trace = true;
		}

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void TestImageTwoByTwo()
        {
            //Test a 2x2 source image.  The results aren't so important here as it is for the Render call to *not* fail.
            _srcWidth = 2;
            _srcHeight = 2;
            _dstWidth = 11;
            _dstHeight = 17;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageArbitrarySizeLargerDestination1()
        {
            _srcWidth = 7;
            _srcHeight = 11;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageArbitrarySizeLargerDestination2()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 41;
            _dstHeight = 53;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageArbitrarySizeLargerSource1()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 7;
            _dstHeight = 11;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageArbitrarySizeLargerSource2()
        {
            _srcWidth = 41;
            _srcHeight = 53;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageSameSize1()
        {
            _srcWidth = 7;
            _srcHeight = 11;
            _dstWidth = 7;
            _dstHeight = 11;

            TestVariousPointAllMethods();
        }

        [Test]
        public void TestImageSameSize2()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPointAllMethods();
        }

        private void TestVariousPointAllMethods()
        {
			_interpolationMethod = ImageGraphic.InterpolationMethods.Bilinear;

            TestAtCentreOfQuadrants();
            TestAtBoundaries();
            TestNearBoundaries();

			_interpolationMethod = ImageGraphic.InterpolationMethods.FastBilinear;

			TestAtCentreOfQuadrants();
			TestAtBoundaries();
			TestNearBoundaries();
		}

        private void TestAtCentreOfQuadrants()
        {
            Point[] arrayOfTestPoints = new Point[4];
            
            //quadrant 1
            arrayOfTestPoints[0].X = (int)(_dstWidth * _oneQuarter);
            arrayOfTestPoints[0].Y = (int)(_dstHeight * _oneQuarter);

            //quadrant 2
            arrayOfTestPoints[1].X = (int)(_dstWidth * _threeQuarters);
            arrayOfTestPoints[1].Y = (int)(_dstHeight * _oneQuarter);

            //quadrant 3
            arrayOfTestPoints[2].X = (int)(_dstWidth * _oneQuarter);
            arrayOfTestPoints[2].Y = (int)(_dstHeight * _threeQuarters);

            //quadrant 4
            arrayOfTestPoints[3].X = (int)(_dstWidth * _threeQuarters);
            arrayOfTestPoints[3].Y = (int)(_dstHeight * _threeQuarters);

            TestAllImageTypes(arrayOfTestPoints);
        }

        private void TestAtBoundaries()
        {
            Point[] arrayOfTestPoints = new Point[4];
            
            //top left
            arrayOfTestPoints[0].X = 0;
            arrayOfTestPoints[0].Y = 0;

            //bottom left
            arrayOfTestPoints[1].X = 0;
            arrayOfTestPoints[1].Y = _dstHeight - 1;

            //top right
            arrayOfTestPoints[2].X = _dstWidth - 1;
            arrayOfTestPoints[2].Y = 0;

            //bottom right
            arrayOfTestPoints[3].X = _dstWidth - 1;
            arrayOfTestPoints[3].Y = _dstHeight - 1;

            TestAllImageTypes(arrayOfTestPoints);
        }

        private void TestNearBoundaries()
        {
            Point[] arrayOfTestPoints = new Point[4];
            
            //top left
            arrayOfTestPoints[0].X = 1;
            arrayOfTestPoints[0].Y = 1;

            //bottom left
            arrayOfTestPoints[1].X = _dstWidth - 2;
            arrayOfTestPoints[1].Y = 1;

            //top right
            arrayOfTestPoints[2].X = 1;
            arrayOfTestPoints[2].Y = _dstHeight - 2;

            //bottom right
            arrayOfTestPoints[3].X = _dstWidth - 2;
            arrayOfTestPoints[3].Y = _dstHeight - 2;

            TestAllImageTypes(arrayOfTestPoints);
        }

        private void InitializeTransform()
        {
            _flipHorizontal = false;
            _flipVertical = false;
            _rotation = 0;
            _scale = 1.0F;
            _scaleToFit = true;
            _translationX = 0;
            _translationY = 0;
        }

        private void TestAllImageTypes(Point[] ArrayOfPoints)
        {
            _sourceImageType = ImageTypes.Mono16;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.Mono8;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.RgbPlanar;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.RgbTriplet;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);
        }

        private void TestImageVariousTranslationsScalesAndOrientations(Point[] arrayOfTestPoints)
        {
			_translationX = 0;
			_translationY = 0;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = _dstWidth / 4;
			_translationY = 0;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = 0;
			_translationY = _dstHeight / 4;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = _dstWidth / 4;
			_translationY = _dstHeight / 4;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);
        }

        private void TestImageVariousScalesAndOrientations(Point[] arrayOfTestPoints)
        {
            _scaleToFit = true;
            _scale = 1.0F; 
            TestImageVariousOrientations(arrayOfTestPoints);

            _scaleToFit = false;
            _scale = 1.0F;
            TestImageVariousOrientations(arrayOfTestPoints);

            _scale = 1.5F;
            TestImageVariousOrientations(arrayOfTestPoints);

            _scale = 2.0F;
            TestImageVariousOrientations(arrayOfTestPoints);
        }

        private void TestImageVariousOrientations(Point [] arrayOfTestPoints)
        {
            //there are 8 different possible (orthogonal) orientations.
            _flipHorizontal = false;
            _flipVertical = false;
            TestImage(arrayOfTestPoints);

            _flipHorizontal = true;
            _flipVertical = false;
            TestImage(arrayOfTestPoints);

            _flipHorizontal = false;
            _flipVertical = true;
            TestImage(arrayOfTestPoints);

            _flipHorizontal = true;
            _flipVertical = true;
            TestImage(arrayOfTestPoints);

            _rotation = 90;
            _flipHorizontal = false;
            _flipVertical = false;
            TestImage(arrayOfTestPoints);

            _rotation = 90;
            _flipHorizontal = false;
            _flipVertical = true;
            TestImage(arrayOfTestPoints);

            _rotation = 90;
            _flipHorizontal = true;
            _flipVertical = false;
            TestImage(arrayOfTestPoints);

            _rotation = 90;
            _flipHorizontal = true;
            _flipVertical = true;
            TestImage(arrayOfTestPoints);
        }

		private void TestImage(Point[] arrayOfTestPoints)
		{
			if (_trace)
			{
				string imageType = String.Empty;
				if (_sourceImageType == ImageTypes.Mono16)
					imageType = "Mono16";
				else if (_sourceImageType == ImageTypes.Mono8)
					imageType = "Mono8";
				else if (_sourceImageType == ImageTypes.RgbPlanar)
					imageType = "RgbPlanar";
				else
					imageType = "RgbTriplet";

				Trace.WriteLine("");
				Trace.WriteLine(imageType);
				Trace.WriteLine(String.Format("Scale (Fit/Scale): {0}/{1}", _scaleToFit, _scale));
				Trace.WriteLine(String.Format("Orientation(FH/FV/R): {0}/{1}/{2}", _flipHorizontal, _flipVertical, _rotation));
				Trace.WriteLine(String.Format("Translation: {0}, {1}", _translationX, _translationY));
			}

			CreateImageLayer(_sourceImageType);
			CreatePhantom();

			//render the image to a bitmap
			Bitmap dstBitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);
			if (_traceBitmap)
			{
				string strTraceBitmap = "Bitmap:\n";

				for (int y = 0; y < dstBitmap.Height; y++)
				{
					for (int x = 0; x < dstBitmap.Width; x++)
					{
						byte pixelValue = dstBitmap.GetPixel(x, y).R;
						strTraceBitmap += String.Format("{0}  ", (int)pixelValue);
					}

					strTraceBitmap += "\n";
				}

				Trace.WriteLine(strTraceBitmap);
			}

			foreach (Point dstPoint in arrayOfTestPoints)
			{
				// The point of the unit test here is to do the same bilinear calculation as is done in the 
				// actual interpolation code, but in a more reliable & simpler way (e.g. not using pointer
				// arithmetic).
				Rectangle dstViewableRectangle = new Rectangle();
				Rectangle srcViewableRectangle = new Rectangle();
				PublicMethodRenderer.CalculateVisibleRectangles(_layer, new Rectangle(0, 0, _dstWidth, _dstHeight), out dstViewableRectangle, out srcViewableRectangle);

				int dstViewableWidth = dstViewableRectangle.Width;
				int dstViewableHeight = dstViewableRectangle.Height;

				byte dstValue = dstBitmap.GetPixel(dstPoint.X, dstPoint.Y).R; //just check the value of R.
				byte backCalculateValue = 0;

				if (dstViewableRectangle.Contains(dstPoint))
				{
					dstViewableRectangle = RectangleUtilities.MakeRectangleZeroBased(dstViewableRectangle);
					float dstTranslatedX = ((float)dstPoint.X + 0.5F) - (float)dstViewableRectangle.Left;
					float dstTranslatedY = ((float)dstPoint.Y + 0.5F) - (float)dstViewableRectangle.Top;
					PointF dstPointTranslated = new PointF(dstTranslatedX, dstTranslatedY);

					PointF srcPoint00 = new PointF();
					if (_rotation != 0)
					{
						int temp = dstViewableHeight;
						dstViewableHeight = dstViewableWidth;
						dstViewableWidth = temp;

						float temp2 = dstPointTranslated.X;
						dstPointTranslated.X = dstPointTranslated.Y;
						dstPointTranslated.Y = temp2;
					}

					float xRatio = (float)srcViewableRectangle.Width / dstViewableWidth;
					float yRatio = (float)srcViewableRectangle.Height / dstViewableHeight;
					srcPoint00.X = srcViewableRectangle.Left + xRatio * dstPointTranslated.X;
					srcPoint00.Y = srcViewableRectangle.Top + yRatio * dstPointTranslated.Y;

					backCalculateValue = PerformBilinearInterpolationAt(srcPoint00);

					string strMessage = String.Format("Image({0}, {1}): {2}, BackCalculated({3:F8}, {4:F8}): {5}\n",
											dstPoint.X, dstPoint.Y, dstValue, srcPoint00.X, srcPoint00.Y, backCalculateValue);
					Trace.Write(strMessage);

					Assert.AreEqual(dstValue, backCalculateValue, strMessage);
				}
				else
				{
					//The bilinear interpolation algorithm should not calculate any values outside the dstViewableRectangle.
					string strMessage = String.Format("Point outside rectangle ({0}, {1})", dstPoint.X, dstPoint.Y);
					Assert.IsTrue(dstValue == 0);
				}
			}
		}

        private byte PerformBilinearInterpolationAt(PointF srcPoint00)
        {
            if (srcPoint00.Y < 0)
                srcPoint00.Y = 0;
            if (srcPoint00.X < 0)
                srcPoint00.X = 0;

			if (srcPoint00.X > (_srcWidth - 1.001F))
				srcPoint00.X = (_srcWidth - 1.001F);
			if (srcPoint00.Y > (_srcHeight - 1.001F))
				srcPoint00.Y = (_srcHeight - 1.001F);

            Point srcPointInt00 = new Point((int)srcPoint00.X, (int)srcPoint00.Y);

            float[,] arrayOfValues = new float[2, 2] { { 0, 0 }, { 0, 0 } };

            if (_layer.IsColor)
            {
                // Just test the R value, the calculation is done in exactly the same way 
                // for G & B, so if it's OK for the R channel it's OK for them too.

                //Get the 4 neighbour pixels for performing bilinear interpolation.
                arrayOfValues[0, 0] = (float)_layer.PixelDataWrapper.GetPixelRGB(srcPointInt00.X, srcPointInt00.Y).R;
                arrayOfValues[0, 1] = (float)_layer.PixelDataWrapper.GetPixelRGB(srcPointInt00.X, srcPointInt00.Y + 1).R;
                arrayOfValues[1, 0] = (float)_layer.PixelDataWrapper.GetPixelRGB(srcPointInt00.X + 1, srcPointInt00.Y).R;
                arrayOfValues[1, 1] = (float)_layer.PixelDataWrapper.GetPixelRGB(srcPointInt00.X + 1, srcPointInt00.Y + 1).R;
            }
            else if (_layer.IsGrayscale)
            {
                if (_layer.BitsAllocated == 16)
                {
                    //Get the 4 neighbour pixels for performing bilinear interpolation.
                    arrayOfValues[0, 0] = (float)_layer.PixelDataWrapper.GetPixel16(srcPointInt00.X, srcPointInt00.Y);
                    arrayOfValues[0, 1] = (float)_layer.PixelDataWrapper.GetPixel16(srcPointInt00.X, srcPointInt00.Y + 1);
                    arrayOfValues[1, 0] = (float)_layer.PixelDataWrapper.GetPixel16(srcPointInt00.X + 1, srcPointInt00.Y);
                    arrayOfValues[1, 1] = (float)_layer.PixelDataWrapper.GetPixel16(srcPointInt00.X + 1, srcPointInt00.Y + 1);
                }
                else if (_layer.BitsAllocated == 8)
                {
                    //Get the 4 neighbour pixels for performing bilinear interpolation.
                    arrayOfValues[0, 0] = (float)_layer.PixelDataWrapper.GetPixel8(srcPointInt00.X, srcPointInt00.Y);
                    arrayOfValues[0, 1] = (float)_layer.PixelDataWrapper.GetPixel8(srcPointInt00.X, srcPointInt00.Y + 1);
                    arrayOfValues[1, 0] = (float)_layer.PixelDataWrapper.GetPixel8(srcPointInt00.X + 1, srcPointInt00.Y);
                    arrayOfValues[1, 1] = (float)_layer.PixelDataWrapper.GetPixel8(srcPointInt00.X + 1, srcPointInt00.Y + 1);
                }
            }

            //this actually performs the bilinear interpolation within the source image using 4 neighbour pixels.
            float dx = srcPoint00.X - (float)srcPointInt00.X;
			float dy = srcPoint00.Y - (float)srcPointInt00.Y;

			if (_interpolationMethod == ImageGraphic.InterpolationMethods.Bilinear)
			{
				float yInterpolated1 = arrayOfValues[0, 0] + (arrayOfValues[1, 0] - arrayOfValues[0, 0]) * dy;
				float yInterpolated2 = arrayOfValues[0, 1] + (arrayOfValues[1, 1] - arrayOfValues[0, 1]) * dy;
				float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dx;

				if (_layer.IsColor)
					return (byte)interpolated;

				return _layer.GrayscaleLUTPipeline.OutputLUT[(ushort)interpolated];
			}
			else if (_interpolationMethod == ImageGraphic.InterpolationMethods.FastBilinear)
			{
				int dyFixed = (int)(dy * _fixedScale);
				int dxFixed = (int)(dx * _fixedScale);
				int yInterpolated1 = (((int)(arrayOfValues[0, 0])) << _fixedPrecision) + ((dyFixed * ((int)((arrayOfValues[1, 0] - arrayOfValues[0, 0])) << _fixedPrecision)) >> _fixedPrecision);
				int yInterpolated2 = (((int)(arrayOfValues[0, 1])) << _fixedPrecision) + ((dyFixed * ((int)((arrayOfValues[1, 1] - arrayOfValues[0, 1])) << _fixedPrecision)) >> _fixedPrecision);
				int interpolated = (yInterpolated1 + (((dxFixed) * (yInterpolated2 - yInterpolated1)) >> _fixedPrecision)) >> _fixedPrecision;

				if (_layer.IsColor)
					return (byte)interpolated;

				return _layer.GrayscaleLUTPipeline.OutputLUT[(ushort)interpolated];
			}
			else
			{
				throw new Exception("No algorithm exists.");
			}
		}

        private void CreateImageLayer(ImageTypes imageType)
        {
            if (imageType == ImageTypes.Mono16)
                _layer = ImageLayerFactory.CreateMonochrome16ImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.Mono8)
                _layer = ImageLayerFactory.CreateMonochrome8ImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.RgbPlanar)
                _layer = ImageLayerFactory.CreateRGBPlanarImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.RgbTriplet)
                _layer = ImageLayerFactory.CreateRGBTripletImageLayer(_srcWidth, _srcHeight);

			_layerGroup = new CompositeGraphic();
            _layerGroup.Graphics.Add(_layer);

            _layer.SpatialTransform.Initialize(); 
            
            _layer.SpatialTransform.SourceRectangle = new Rectangle(0, 0, _srcWidth, _srcHeight);
            _layer.SpatialTransform.DestinationRectangle = new Rectangle(0, 0, _dstWidth, _dstHeight);
            _layer.SpatialTransform.FlipHorizontal = _flipHorizontal;
            _layer.SpatialTransform.FlipVertical = _flipVertical;
            _layer.SpatialTransform.Rotation = _rotation;
            _layer.SpatialTransform.Scale = _scale;
            //_layer.SpatialTransform.ScaleToFit = _scaleToFit;
            _layer.SpatialTransform.TranslationX = _translationX;
            _layer.SpatialTransform.TranslationY = _translationY;

			_layer.NormalInterpolationMethod = _interpolationMethod;
        }

        private void CreatePhantom()
        {
            int max16 = 65535;
            int max8 = 255;
            int maxXPlusY;
            if (_srcHeight > 1 && _srcWidth > 1)
                maxXPlusY = _srcWidth + _srcHeight - 2;
            else
                maxXPlusY = 1;

            float scale16 = max16 / maxXPlusY;
            float scale8 = max8 / maxXPlusY;

            string strTracePhantom = "Phantom:\n";

            //fill the pixel data with values spanning the full possible range (using x + y as a base).
            for (int y = 0; y < _srcHeight; ++y) 
            {
                for (int x = 0; x < _srcWidth; ++x)
                {
                    if (_layer.IsColor)
                    {
                        Color colour = new Color();
                        int value = (int)((x + y) * scale8);
                        colour = Color.FromArgb(0xFF, value, value, value);
                        _layer.PixelDataWrapper.SetPixelRGB(x, y, colour);

                        if (_tracePhantom)
                            strTracePhantom += String.Format("{0}  ", value);
                    }
                    else
                    {
                        if (_layer.BitsAllocated == 16)
                        {
                            ushort PixelValue = (ushort)((x + y) * scale16);
                            _layer.PixelDataWrapper.SetPixel16(x, y, PixelValue);
                            if (_tracePhantom)
                                strTracePhantom += String.Format("{0}  ", PixelValue);
                        }
                        else
                        {
                            byte PixelValue = (byte)((x + y) * scale8);
                            _layer.PixelDataWrapper.SetPixel8(x, y, PixelValue);
                            if (_tracePhantom)
                                strTracePhantom += String.Format("{0}  ", PixelValue);
                        }
                    }
                }

                strTracePhantom += "\n";
            }

            if (_tracePhantom)
                Trace.WriteLine(strTracePhantom);
        }
    }
*/
}

#endif
