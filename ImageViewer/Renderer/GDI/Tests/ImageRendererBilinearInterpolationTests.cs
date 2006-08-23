#if	UNIT_TESTS

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

using NUnit.Framework;
using ClearCanvas.ImageViewer.Renderer.GDI;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
    [TestFixture]
    public class ImageRendererBilinearInterpolationTests
    {
        enum ImageTypes { MONO8, MONO16, RGB_PLANAR, RGB_TRIPLET};
        
        LayerGroup _layerGroup;
        MockImageLayer _layer;

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

        public ImageRendererBilinearInterpolationTests()
        {
            _tracePhantom = true;
            _traceBitmap = true;
            _trace = true;
        }

        [TestFixtureSetUp]
        public void Init()
        {
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        [Ignore("Renderer fails due to memory access violations when images are this small (See Ticket#89).")]
        public void TestImageOneByOne()
        {
            //Test a 1x1 source image.  The results aren't so important here as it is for the Render call to *not* fail.
            _srcWidth = 1;
            _srcHeight = 1;
            _dstWidth = 11;
            _dstHeight = 17;

            TestVariousPoints();
        }

        [Test]
        [Ignore("Renderer fails due to memory access violations when images are this small (See Ticket#89).")]
        public void TestImageTwoByTwo()
        {
            //Test a 2x2 source image.  The results aren't so important here as it is for the Render call to *not* fail.
            _srcWidth = 2;
            _srcHeight = 2;
            _dstWidth = 11;
            _dstHeight = 17;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageArbitrarySizeLargerDestination1()
        {
            _srcWidth = 7;
            _srcHeight = 11;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageArbitrarySizeLargerDestination2()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 41;
            _dstHeight = 53;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageArbitrarySizeLargerSource1()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 7;
            _dstHeight = 11;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageArbitrarySizeLargerSource2()
        {
            _srcWidth = 41;
            _srcHeight = 53;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageSameSize1()
        {
            _srcWidth = 7;
            _srcHeight = 11;
            _dstWidth = 7;
            _dstHeight = 11;

            TestVariousPoints();
        }

        [Test]
		[Ignore("Ignore for now - the unit tests will not work until the off-by-1 in the usage of SpatialTransform is corrected (see ticket #93).  Tests also need to be updated to test both 'fast' and regular bilinear interpolation.")]
        public void TestImageSameSize2()
        {
            _srcWidth = 23;
            _srcHeight = 31;
            _dstWidth = 23;
            _dstHeight = 31;

            TestVariousPoints();
        }

        private void TestVariousPoints()
        {
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
            arrayOfTestPoints[1].X = 1;
            arrayOfTestPoints[1].Y = 1;

            //bottom left
            arrayOfTestPoints[3].X = _dstWidth - 2;
            arrayOfTestPoints[3].Y = 1;

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
            _sourceImageType = ImageTypes.MONO16;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.MONO8;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.RGB_PLANAR;
            InitializeTransform();
            TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

            _sourceImageType = ImageTypes.RGB_TRIPLET;
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

        private void TestImage(Point [] arrayOfTestPoints)
        {
            if (_trace)
            {
                string imageType = String.Empty;
                if (_sourceImageType == ImageTypes.MONO16)
                    imageType = "MONO16";
                else if (_sourceImageType == ImageTypes.MONO8)
                    imageType = "MONO8";
                else if (_sourceImageType == ImageTypes.RGB_PLANAR)
                    imageType = "RGB_PLANAR";
                else
                    imageType = "RGB_TRIPLET";

                Trace.WriteLine(""); 
                Trace.WriteLine(imageType);
                Trace.WriteLine(String.Format("Scale (Fit/Scale): {0}/{1}", _scaleToFit, _scale));
                Trace.WriteLine(String.Format("Orientation(FH/FV/R): {0}/{1}/{2}", _flipHorizontal, _flipVertical, _rotation));
                Trace.WriteLine(String.Format("Translation: {0}, {1}", _translationX, _translationY));
            }

            CreateImageLayer(_sourceImageType);
            CreatePhantom();

            //render the image to a bitmap
            Bitmap dstBitmap = null;

            try
            {
                dstBitmap = ImageRendererTestUtilities.RenderLayer(_layer, _dstWidth, _dstHeight);
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
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            
            foreach (Point dstPoint in arrayOfTestPoints)
            {
                try
                {
                    // Because the renderer does not use the spatial transform to obtain the source coordinate when performing
                    // the interpolation (and the spatial transform results are trusted), we use it here in the unit tests.
                    // After that, it's just a simple bilinear calculation of a value that is then put through the same LUT
                    // as the value(s) in the renderer were put through.  The calculation methods are independent of each other
                    // as they use different code entirely to compute the interpolated value(s).  Hence the reason I
                    // can get away with performing these unit tests automated and not 'by hand'.

                    RectangleF dstViewableRectangle = new RectangleF();
                    RectangleF srcViewableRectangle = new RectangleF();
                    CalculateVisibleRectanglesF(_layer, new Rectangle(0, 0, _dstWidth, _dstHeight), out dstViewableRectangle, out srcViewableRectangle);
                    // because we calculate the 'visible rectangles' in the actual interpolation algorithm
                    // and they are rounded to integer values before performing the interpolation, we have to
                    // account for that discrepancy here as well or we'll get the occasional value that doesn't match.
                    float xCorrection = srcViewableRectangle.Left - (int)srcViewableRectangle.Left;
                    float yCorrection = srcViewableRectangle.Top - (int)srcViewableRectangle.Top;

                    byte dstValue = dstBitmap.GetPixel(dstPoint.X, dstPoint.Y).R; //just check the value of R.
                    
                    PointF srcPoint00 = _layer.SpatialTransform.ConvertToSource(new PointF((float)dstPoint.X, (float)dstPoint.Y));
                    srcPoint00.X -= xCorrection; //correct it for the rounding that occurs in CalculateVisibleRectangles.
                    srcPoint00.Y -= yCorrection; 
                    byte backCalculateValue = PerformBilinearInterpolationAt(srcPoint00);

                    string strMessage = String.Format("Image({0}, {1}): {2}, BackCalculated({3}, {4}): {5}\n",
                                            dstPoint.X, dstPoint.Y, dstValue, srcPoint00.X, srcPoint00.Y, backCalculateValue);
                    Trace.Write(strMessage);
                    
                    Assert.AreEqual(dstValue, backCalculateValue, strMessage);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        // this function is in the test module rather than in the source code
        // because we're not going to use it for anything other than tests right now.
        public static void CalculateVisibleRectanglesF(
            ImageLayer imageLayer,
            RectangleF clientRectangle,
            out RectangleF dstVisibleRectangleF,
            out RectangleF srcVisibleRectangleF)
        {
            Rectangle srcRectangle = imageLayer.SpatialTransform.SourceRectangle;
            dstVisibleRectangleF = imageLayer.SpatialTransform.ConvertToDestination(srcRectangle);

            // Find the intersection between the drawable client rectangle and
            // the transformed destination rectangle
            dstVisibleRectangleF = RectangleUtilities.Intersect(clientRectangle, dstVisibleRectangleF);
            if (dstVisibleRectangleF.IsEmpty)
            {
                dstVisibleRectangleF = Rectangle.Empty;
                srcVisibleRectangleF = Rectangle.Empty;
                return;
            }

            // From that intersection, figure out what portion of the image
            // is Visible in source coordinates
            srcVisibleRectangleF = imageLayer.SpatialTransform.ConvertToSource(dstVisibleRectangleF);

            //dstVisibleRectangleF = RectangleUtilities.MakeRectangleZeroBased(dstVisibleRectangleF);
            //srcVisibleRectangleF = RectangleUtilities.MakeRectangleZeroBased(srcVisibleRectangleF);
        }

        private byte PerformBilinearInterpolationAt(PointF srcPoint00)
        {
            if (srcPoint00.Y < 0)
                srcPoint00.Y = 0;
            if (srcPoint00.X < 0)
                srcPoint00.X = 0;

            if (srcPoint00.X >= (_layer.Columns - 1))
                srcPoint00.X = (_layer.Columns - 1.001F);
            if (srcPoint00.Y >= (_layer.Rows - 1))
                srcPoint00.Y = (_layer.Rows - 1.001F);

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

            try
            {
                //this actually performs the bilinear interpolation within the source image using 4 neighbour pixels.
                float dx = srcPoint00.X - srcPointInt00.X;
                float dy = srcPoint00.Y - srcPointInt00.Y;

                float I1y = arrayOfValues[0, 0] + (arrayOfValues[1, 0] - arrayOfValues[0, 0]) * dy;
                float I2y = arrayOfValues[0, 1] + (arrayOfValues[1, 1] - arrayOfValues[0, 1]) * dy;
                float Ifinal = I1y + (I2y - I1y) * dx;

                if (_layer.IsColor)
                    return (byte)Ifinal;

                try
                {
                    return _layer.GrayscaleLUTPipeline.OutputLUT[(int)Ifinal];
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            return 0;
        }

        private void CreateImageLayer(ImageTypes imageType)
        {
            if (imageType == ImageTypes.MONO16)
                _layer = ImageLayerFactory.CreateMonochrome16ImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.MONO8)
                _layer = ImageLayerFactory.CreateMonochrome8ImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.RGB_PLANAR)
                _layer = ImageLayerFactory.CreateRGBPlanarImageLayer(_srcWidth, _srcHeight);
            else if (imageType == ImageTypes.RGB_TRIPLET)
                _layer = ImageLayerFactory.CreateRGBTripletImageLayer(_srcWidth, _srcHeight);

			_layer.InterpolationMethod = ImageLayer.InterpolationMethods.BILINEAR;

			_layerGroup = new LayerGroup();
            _layerGroup.Layers.Add(_layer);

            _layer.SpatialTransform.Initialize(); 
            
            _layer.SpatialTransform.SourceRectangle = new Rectangle(0, 0, _srcWidth, _srcHeight);
            _layer.SpatialTransform.DestinationRectangle = new Rectangle(0, 0, _dstWidth, _dstHeight);
            _layer.SpatialTransform.FlipHorizontal = _flipHorizontal;
            _layer.SpatialTransform.FlipVertical = _flipVertical;
            _layer.SpatialTransform.Rotation = _rotation;
            _layer.SpatialTransform.Scale = _scale;
            _layer.SpatialTransform.ScaleToFit = _scaleToFit;
            _layer.SpatialTransform.TranslationX = _translationX;
            _layer.SpatialTransform.TranslationY = _translationY;

            _layer.SpatialTransform.Calculate();
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
}

#endif