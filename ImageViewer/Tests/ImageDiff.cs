#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// Identifies a standard algorithm implementation for use with <see cref="ImageDiff"/>.
	/// </summary>
	public enum ImageDiffAlgorithm
	{
		/// <summary>
		/// Performs a pixel-by-pixel comparison using the Euclidian distance method.
		/// </summary>
		/// <remarks>
		/// The result is the Euclidian distance between the two images.
		/// If the images have different sizes, the result is always <see cref="double.NaN"/>.
		/// Colour images are handled by computing the magnitude of the distance between the colours of two pixels in RGB 3-space and normalizing to be between 0 and 255.
		/// </remarks>
		Euclidian,

		/// <summary>
		/// Performs a pixel-by-pixel comparison by computing the mean pixel-wise difference.
		/// </summary>
		/// <remarks>
		/// The result is the mean pixel-wise difference if the results have low deviation, or <see cref="double.NaN"/> otherwise.
		/// If the images have different sizes, the result is always <see cref="double.NaN"/>.
		/// Colour images are handled by computing the magnitude of the distance between the colours of two pixels in RGB 3-space and normalizing to be between 0 and 255.
		/// </remarks>
		Legacy
	}

	/// <summary>
	/// Computes and quantifies the differences between two images.
	/// </summary>
	public abstract class ImageDiff
	{
		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage, float tolerance)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap, tolerance);
				}
			}
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0);
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage, float tolerance)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0, tolerance);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			return PerformComparison(referenceImage, testImage, false, out diffImage);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage, out Bitmap difference)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap, out difference);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage, out Bitmap difference)
		{
			return PerformComparison(referenceImage, testImage, true, out difference);
		}

		public virtual Bitmap Diff(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Diff(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual Bitmap Diff(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			PerformComparison(referenceImage, testImage, true, out diffImage);
			return diffImage;
		}

		protected abstract double PerformComparison(Bitmap referenceImage, Bitmap testImage, bool generateDiffImage, out Bitmap diffImage);

		protected static Bitmap DrawToBitmap(IPresentationImage presentationImage)
		{
			var imageGraphicProvider = (IImageGraphicProvider) presentationImage;
			var annotationLayoutProvider = presentationImage as IAnnotationLayoutProvider;
			var annotationLayoutVisible = true;

			if (annotationLayoutProvider != null)
			{
				annotationLayoutVisible = annotationLayoutProvider.AnnotationLayout.Visible;
				annotationLayoutProvider.AnnotationLayout.Visible = false;
			}

			try
			{
				return presentationImage.DrawToBitmap(imageGraphicProvider.ImageGraphic.Columns, imageGraphicProvider.ImageGraphic.Rows);
			}
			finally
			{
				if (annotationLayoutProvider != null)
				{
					annotationLayoutProvider.AnnotationLayout.Visible = annotationLayoutVisible;
				}
			}
		}

		protected static Vector3D ToRgbVector(Color color)
		{
			return new Vector3D(color.R, color.G, color.B);
		}

		protected static Vector3D ToRgbVector(int argb)
		{
			return ToRgbVector(Color.FromArgb(argb));
		}

		#region Static Helpers

		public static ImageDiff GetImplementation(ImageDiffAlgorithm algorithm)
		{
			switch (algorithm)
			{
				case ImageDiffAlgorithm.Legacy:
					return new StatisticalDifferenceImageDiff();
				case ImageDiffAlgorithm.Euclidian:
				default:
					return new EuclidianImageDiff();
			}
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, tolerance);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, tolerance);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, out difference);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, out difference);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage);
		}

		#endregion

		#region Default Implementations

		private delegate void Iterator(int x, int y, int index, double delta);

		private static void PerformIterativeComparison(Bitmap referenceImage, Bitmap testImage, bool generateDiffImage, out Bitmap diffImage, Iterator iterator)
		{
			if (referenceImage.Size != testImage.Size)
			{
				diffImage = null;
				return;
			}

			double root3 = Math.Sqrt(3);
			int width = referenceImage.Width;
			int height = referenceImage.Height;
			int count = width*height;
			var rect = new Rectangle(0, 0, width, height);

			diffImage = generateDiffImage ? new Bitmap(width, height) : null;

			unsafe
			{
				var referenceBits = referenceImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				try
				{
					var testBits = testImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					try
					{
						int* testData = (int*) testBits.Scan0;
						int* referenceData = (int*) referenceBits.Scan0;
						for (int n = 0; n < count; n++)
						{
							var referenceValue = ToRgbVector(referenceData[n]);
							var testValue = ToRgbVector(testData[n]);
							var delta = (referenceValue - testValue).Magnitude/root3;

							iterator.Invoke(n%width, n/width, n, delta);

							if (generateDiffImage)
							{
								var v = (int) (Math.Min(255, Math.Max(0, delta)));
								diffImage.SetPixel(n%width, n/width, Color.FromArgb(v, v, v));
							}
						}
					}
					finally
					{
						testImage.UnlockBits(testBits);
					}
				}
				finally
				{
					referenceImage.UnlockBits(referenceBits);
				}
			}
		}

		#region Euclidian Distance

		private class EuclidianImageDiff : ImageDiff
		{
			protected override double PerformComparison(Bitmap referenceImage, Bitmap testImage, bool generateDiffImage, out Bitmap diffImage)
			{
				if (referenceImage.Size != testImage.Size)
				{
					diffImage = null;
					return double.NaN;
				}

				var count = referenceImage.Width*referenceImage.Height;
				var sumDeltaSquares = 0.0;

				PerformIterativeComparison(referenceImage, testImage, generateDiffImage, out diffImage, (x, y, i, d) => sumDeltaSquares += d*d);

				return Math.Sqrt(sumDeltaSquares)/count;
			}
		}

		#endregion

		#region Legacy (Statistical Difference)

		private class StatisticalDifferenceImageDiff : ImageDiff
		{
			protected override double PerformComparison(Bitmap referenceImage, Bitmap testImage, bool generateDiffImage, out Bitmap diffImage)
			{
				if (referenceImage.Size != testImage.Size)
				{
					diffImage = null;
					return double.NaN;
				}

				var list = new List<double>();

				PerformIterativeComparison(referenceImage, testImage, generateDiffImage, out diffImage, (x, y, i, d) => list.Add(d));

				var results = new Statistics(list);
				if (results.StandardDeviation < 16)
					return results.Mean;
				return double.NaN;
			}
		}

		#endregion

		#endregion
	}
}

#endif