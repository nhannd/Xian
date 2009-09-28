#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	public abstract class RoiTestBase<T>
	{
		/// <summary>
		/// Tests the <see cref="Roi.Contains(System.Drawing.PointF)"/> method for a given shape. The image is used to provide a basis for the coordinate space.
		/// </summary>
		protected void TestRoiContains(ImageKey key, T shapeData, string name)
		{
			using (IPresentationImage image = GetImage(key))
			{
				IImageSopProvider provider = (IImageSopProvider) image;
				using (Bitmap bmp = new Bitmap(provider.Frame.Columns, provider.Frame.Rows))
				{
					using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
					{
						g.Clear(Color.Black);

						// baseline ROI using GDI's GraphicsPath
						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							AddShapeToGraphicsPath(graphicsPath, shapeData);
							g.FillPath(Brushes.Blue, graphicsPath);
						}
					}

					// simulates ROIs that an end-user would create using graphics constructed by the tools
					Roi userRoi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
					userRoi.PixelData.ForEachPixel(delegate(int i, int x, int y, int pixelIndex)
					                               	{
					                               		if (userRoi.Contains(x, y))
					                               			bmp.SetPixel(x, y, ShiftColor(bmp.GetPixel(x, y), true, false, false));
					                               	});

					// simulates ROIs that a programmer might create using the SDK directly, rather than via graphics
					Roi sdkRoi = CreateRoiFromImage(image, shapeData);
					sdkRoi.PixelData.ForEachPixel(delegate(int i, int x, int y, int pixelIndex)
					                              	{
					                              		if (sdkRoi.Contains(x, y))
					                              			bmp.SetPixel(x, y, ShiftColor(bmp.GetPixel(x, y), false, true, false));
					                              	});

					string filename = string.Format("{0}.{1}.containment.test.png", name, this.ShapeName).TrimStart('.');
					bmp.Save(filename);
					Trace.WriteLine(string.Format("Pixel containment map has been dumped to {0}.", filename));

					int totalCount = 0;
					int errorCount = 0;
					for (int y = 0; y < bmp.Height; y++)
					{
						for (int x = 0; x < bmp.Width; x++)
						{
							Color c = bmp.GetPixel(x, y);
							if (c.R > 0 || c.G > 0 || c.B > 0)
							{
								totalCount++;
								if (c.R < 255 || c.G < 255 || c.B < 255)
									errorCount++;
							}
						}
					}

					if (errorCount > 0)
					{
						WriteLine("The pixel containment test results are not perfect. {0} differences were found out of {1} pixels.", errorCount, totalCount);
						WriteLine("The image should be mostly a white shape on a black background.");
						WriteLine("Any coloured areas indicate locations where Roi.Contains(...) did not return perfectly coincident results.");
						WriteLine("There will invariably be such areas along shape boundaries, but the shape should be white overall.");
						WriteLine("Red channel is painted by user mode ROI. Green channel is painted by SDK. Blue channel is painted by GDI+.");
					}
					Assert.AreEqual(0, errorCount, 0.01*totalCount, "Automated pixel containment test failed. Please review the test output manually.");
				}
			}
		}

		/// <summary>
		/// Tests the area, mean and standard deviation calculations for a specific shape on a specific image.
		/// </summary>
		protected void TestRoiStatsCalculations(ImageKey key, T shapeData, double expectedPerimeter, double expectedArea, double expectedMean, double expectedSigma)
		{
			string description = string.Format("{0} on {1}", shapeData, key);
			using (IPresentationImage image = GetImage(key))
			{
				// simulates ROIs that an end-user would create using graphics constructed by the tools
				Roi userRoi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
				PerformRoiStatsCalculations(userRoi, expectedPerimeter, expectedArea, expectedMean, expectedSigma, "user", description);

				// simulates ROIs that a programmer might create using the SDK directly, rather than via graphics
				Roi sdkRoi = CreateRoiFromImage(image, shapeData);
				PerformRoiStatsCalculations(sdkRoi, expectedPerimeter, expectedArea, expectedMean, expectedSigma, "SDK", description);
			}
		}

		private void PerformRoiStatsCalculations(Roi roi, double expectedPerimeter, double expectedArea, double expectedMean, double expectedSigma, string mode, string shapeData)
		{
			WriteLine("Testing {1}-MODE {2} ROI using {0}", shapeData, mode.ToUpperInvariant(), this.ShapeName);

			RoiStatistics roiStats = RoiStatistics.Calculate(roi);
			double measuredArea = 0;
			double measuredMean = roiStats.Mean;
			double measuredSigma = roiStats.StandardDeviation;

			if (roi is IRoiAreaProvider)
				measuredArea = ((IRoiAreaProvider) roi).Area;

			if (this.VerboseOutput)
			{
				WriteLine("Expecting  A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", expectedArea, expectedMean, expectedSigma);
				WriteLine("Actual     A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", measuredArea, measuredMean, measuredSigma);
			}

			double errorArea = Math.Abs(expectedArea - measuredArea);
			double errorMean = Math.Abs(expectedMean - measuredMean);
			double errorSigma = Math.Abs(expectedSigma - measuredSigma);

			WriteLine("Errors    \u0394A={0:f2} \u0394\u03BC={1:f2} \u0394\u03C3={2:f2}", errorArea, errorMean, errorSigma);

			double areaToleranceFactor = 0.01;
			if (expectedPerimeter/expectedArea > 0.25)
			{
				WriteLine("High Perimeter-to-Area Ratio (P={0:f3})", expectedPerimeter);
				areaToleranceFactor = 0.05;
			}

			double toleranceArea = areaToleranceFactor*expectedPerimeter;
			double toleranceMean = 2.0;
			double toleranceSigma = 2.0;

			Assert.AreEqual(expectedArea, measuredArea, toleranceArea, string.Format("\u0394area exceeds tolerance of {0:p0} of ROI perimeter", areaToleranceFactor));
			Assert.AreEqual(expectedMean, measuredMean, toleranceMean, string.Format("\u0394mean exceeds tolerance of 1% of pixel value range"));
			Assert.AreEqual(expectedSigma, measuredSigma, toleranceSigma, string.Format("\u0394stdev exceeds tolerance of 1% of pixel value range"));
		}

		/// <summary>
		/// Tests the statistics calculation for consistency over a number of trial runs using the same shape.
		/// </summary>
		protected void TestRoiStatsCalculationConsistency()
		{
			const int samples = 100;
			const int gridSize = 4;
			foreach (ImageKey imageKey in Enum.GetValues(typeof (ImageKey)))
			{
				WriteLine("Testing on Image {0}", imageKey.ToString());
				using (IPresentationImage image = GetImage(imageKey))
				{
					IImageSopProvider provider = (IImageSopProvider) image;
					IOverlayGraphicsProvider overlayGraphicsProvider = (IOverlayGraphicsProvider) image;
					int rows = provider.Frame.Rows;
					int cols = provider.Frame.Columns;

					for (int r = rows/gridSize/2; r < rows; r += rows/gridSize)
					{
						for (int c = cols/gridSize/2; c < cols; c += cols/gridSize)
						{
							if (this.VerboseOutput)
								WriteLine("Checking {0} core samples for consistent results at location {1}", samples, new PointF(c, r));

							T shapeData = CreateCoreSampleShape(new PointF(c, r), rows, cols);
							double expectedArea = 0, expectedMean = 0, expectedSigma = 0;
							for (int n = 0; n < samples; n++)
							{
								Roi userRoi = CreateRoiFromGraphic(overlayGraphicsProvider, shapeData);
								RoiStatistics stats = RoiStatistics.Calculate(userRoi);
								if (n == 0)
								{
									if (userRoi is IRoiAreaProvider)
										expectedArea = ((IRoiAreaProvider) userRoi).Area;
									expectedMean = stats.Mean;
									expectedSigma = stats.StandardDeviation;

									if (this.VerboseOutput)
										WriteLine("First sample reported A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", expectedArea, expectedMean, expectedSigma);

									continue;
								}

								// very strict tolerance. performing the calculation the first time should yield the same result as the next hundred times.
								if (userRoi is IRoiAreaProvider)
									Assert.AreEqual(expectedArea, ((IRoiAreaProvider) userRoi).Area, double.Epsilon, "Area calculation consistency fail.");
								Assert.AreEqual(expectedMean, stats.Mean, double.Epsilon, "Mean calculation consistency fail.");
								Assert.AreEqual(expectedSigma, stats.StandardDeviation, double.Epsilon, "Stdev calculation consistency fail.");
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the name of the shape covered by this test.
		/// </summary>
		protected abstract string ShapeName { get; }

		/// <summary>
		/// Gets a value indicating whether or not output should be verbose.
		/// </summary>
		protected virtual bool VerboseOutput
		{
			get { return true; }
		}

		protected abstract T CreateCoreSampleShape(PointF location, int imageRows, int imageCols);
		protected abstract Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, T shapeData);
		protected abstract Roi CreateRoiFromImage(IPresentationImage image, T shapeData);
		protected abstract void AddShapeToGraphicsPath(GraphicsPath graphicsPath, T shapeData);

		protected static void WriteLine(string message, params object[] args)
		{
			if (args != null && args.Length > 0)
				message = string.Format(message, args);
			Trace.WriteLine(message);
		}

		protected static IPresentationImage GetImage(ImageKey key)
		{
			string filename = string.Format(@"RoiGraphics\Tests\Images\{0}.dcm", key.ToString().ToLower());
			try
			{
				LocalSopDataSource dataSource = new LocalSopDataSource(filename);
				ImageSop imageSop = new ImageSop(dataSource);
				IPresentationImage theOne = null;
				foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
				{
					if (theOne == null)
					{
						theOne = image;
						continue;
					}
					image.Dispose();
				}
				imageSop.Dispose();
				return theOne;
			}
			catch (Exception ex)
			{
				throw new FileNotFoundException("Unable to load requested test image. Please check that the assembly has been built.", filename, ex);
			}
		}

		private static Color ShiftColor(Color original, bool shiftRed, bool shiftGreen, bool shiftBlue)
		{
			return Color.FromArgb(shiftRed ? 255 : (int) original.R, shiftGreen ? 255 : (int) original.G, shiftBlue ? 255 : (int) original.B);
		}

		private class GraphicsPathRoi : Roi, IRoiAreaProvider
		{
			private readonly GraphicsPath _graphicsPath;

			public GraphicsPathRoi(GraphicsPath graphicsPath, IPresentationImage image)
				: base(image)
			{
				Assert.IsNotNull(graphicsPath);
				_graphicsPath = graphicsPath;
			}

			protected override RectangleF ComputeBounds()
			{
				return new RectangleF(0, 0, base.ImageColumns - 1, base.ImageRows - 1);
			}

			public override bool Contains(PointF point)
			{
				return _graphicsPath.IsVisible(point);
			}

			public double Area
			{
				get
				{
					int count = 0;
					base.PixelData.ForEachPixel(delegate(int i, int x, int y, int pixelIndex)
					                            	{
					                            		if (this.Contains(x, y))
					                            			count++;
					                            	});
					return count;
				}
			}

			[Obsolete("This method is not implemented on this test class.")]
			public override Roi CopyTo(IPresentationImage presentationImage)
			{
				throw new NotImplementedException("This method is not implemented on this test class.");
			}

			Units IRoiAreaProvider.Units
			{
				get { return Units.Pixels; }
				set { }
			}

			bool IRoiAreaProvider.IsCalibrated
			{
				get { return false; }
			}
		}

		protected enum ImageKey
		{
			Simple01,
			Simple02,
			Simple03,
			Simple04,
			Simple05,
			Simple06,
			Complex01,
			Complex02,
			Complex03,
			Complex04,
			Complex05,
			Complex06,
			Complex07,
			Complex08,
			Complex09,
			Complex10,
			Complex11,
			Complex12,
			Real01,
			Real02,
			Real03,
			Real04,
			Real05,
			Real06
		}
	}
}

#endif