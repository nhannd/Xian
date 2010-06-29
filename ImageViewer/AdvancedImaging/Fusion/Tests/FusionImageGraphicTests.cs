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
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[TestFixture(Description = "Tests for validating fused image pixel data alignment and orientation (including MPR of overlay data)")]
	public class FusionImageGraphicTests
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test(Description = "This validates that the test support mechanism does indeed know what are perfectly fused images.")]
		public void ValidateImageDifferenceEngine()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.AreEqual(0, results[n], 100*double.Epsilon, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionLowPETResolution()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.7f, 0.7f, 0.7f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionDifferentPETOrientation()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), -Vector3D.yUnit, -Vector3D.zUnit, Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionUnsignedCTUnsignedPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     false, new Vector3D(0.7f, 0.7f, 0.7f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     false, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionSignedCTUnsignedPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     true, new Vector3D(0.7f, 0.7f, 0.7f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     false, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionUnsignedCTSignedPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     false, new Vector3D(0.7f, 0.7f, 0.7f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     true, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionSignedCTSignedPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     true, new Vector3D(0.7f, 0.7f, 0.7f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     true, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion4To3AnisotropicCTIsotropicPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
												 new Vector3D(0.8f, 0.6f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion3To4AnisotropicCTIsotropicPET()
		{
			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
												 new Vector3D(0.6f, 0.8f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		private const string _anisotropicPixelAspectRatioMPRSupport = "MPR currently disallows anisotropic input (listed in outstanding ticket #6160)";

		[Test]
		public void TestFusionIsotropicCT4To3AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.2f, 0.9f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.027, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionIsotropicCT3To4AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(0.9f, 1.2f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.027, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion4To3AnisotropicCT4To3AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.8f, 0.6f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.2f, 0.9f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion3To4AnisotropicCT4To3AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.6f, 0.8f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(1.2f, 0.9f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion4To3AnisotropicCT3To4AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.8f, 0.6f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(0.9f, 1.2f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusion3To4AnisotropicCT3To4AnisotropicPET()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.6f, 0.8f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(0.9f, 1.2f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionAnisotropicSpecialCase1Test()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.75f, 0.7f, 0.8f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                     new Vector3D(0.75f, 0.7f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionAnisotropicSpecialCase2Test()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
												 new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionAnisotropicSpecialCase3Test()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
												 new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.yUnit, Vector3D.zUnit, -Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		[Test]
		public void TestFusionAnisotropicSpecialCase4Test()
		{
			Assert.Ignore(_anisotropicPixelAspectRatioMPRSupport);

			string testName = MethodBase.GetCurrentMethod().Name;
			using (var data = new FusionTestData(TestDataFunction.Threed,
			                                     new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
												 new Vector3D(0.9f, 1.0f, 0.8f), Vector3D.yUnit, Vector3D.zUnit, Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		private static IList<double> DiffFusionImages(FusionTestData data, string testName)
		{
			var outputPath = new DirectoryInfo(Path.Combine(typeof (FusionImageGraphicTests).FullName, testName));
			if (outputPath.Exists)
				outputPath.Delete(true);
			outputPath.Create();

			using (var log = File.CreateText(Path.Combine(outputPath.FullName, "data.csv")))
			{
				using (var referenceDisplaySet = data.CreateBaseDisplaySet())
				{
					using (var testDisplaySet = data.CreateFusionDisplaySet())
					{
						var list = new List<double>();
						int index = 0;
						foreach (var testImage in testDisplaySet.PresentationImages)
						{
							var referenceImage = referenceDisplaySet.PresentationImages[index];
							NormalizePresentationImageDisplay(testImage);
							NormalizePresentationImageDisplay(referenceImage);

							Bitmap diff;
							double result = ImageDiff.Compare(ImageDiffAlgorithm.Euclidian, referenceImage, testImage, out diff);
							diff.Save(Path.Combine(outputPath.FullName, string.Format("image{0}.png", index)));
							diff.Dispose();
							log.WriteLine("{0}, {1:f6}", index, result);
							list.Add(result);

							++index;
						}

						return list;
					}
				}
			}
		}

		private static void NormalizePresentationImageDisplay(IPresentationImage image)
		{
			if (image is IColorMapProvider)
			{
				var colorMapProvider = (IColorMapProvider) image;
				colorMapProvider.ColorMapManager.InstallColorMap("Grayscale");
			}

			if (image is ILayerOpacityProvider)
			{
				var layerOpacityProvider = (ILayerOpacityProvider) image;
				layerOpacityProvider.LayerOpacityManager.Thresholding = false;
				layerOpacityProvider.LayerOpacityManager.Opacity = 0.5f;
			}
		}

		private class FusionTestData : IDisposable
		{
			public delegate IEnumerable<ISopDataSource> CreateSopsDelegate();

			private IEnumerable<ISopDataSource> _baseSops;
			private IEnumerable<ISopDataSource> _overlaySops;
			private TestDisplaySetGenerator _testDisplaySetGenerator;

			public FusionTestData(TestDataFunction function,
			                      Vector3D baseSpacing, Vector3D baseOrientationX, Vector3D baseOrientationY, Vector3D baseOrientationZ,
			                      Vector3D overlaySpacing, Vector3D overlayOrientationX, Vector3D overlayOrientationY, Vector3D overlayOrientationZ)
			{
				_baseSops = function.CreateSops(true, Modality.CT, baseSpacing, baseOrientationX, baseOrientationY, baseOrientationZ);
				_overlaySops = function.CreateSops(true, Modality.PT, overlaySpacing, overlayOrientationX, overlayOrientationY, overlayOrientationZ);
				_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSops, _overlaySops);
			}

			public FusionTestData(TestDataFunction function,
			                      bool baseIsSigned, Vector3D baseSpacing, Vector3D baseOrientationX, Vector3D baseOrientationY, Vector3D baseOrientationZ,
			                      bool overlayIsSigned, Vector3D overlaySpacing, Vector3D overlayOrientationX, Vector3D overlayOrientationY, Vector3D overlayOrientationZ)
			{
				_baseSops = function.CreateSops(baseIsSigned, Modality.CT, baseSpacing, baseOrientationX, baseOrientationY, baseOrientationZ);
				_overlaySops = function.CreateSops(overlayIsSigned, Modality.PT, overlaySpacing, overlayOrientationX, overlayOrientationY, overlayOrientationZ);
				_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSops, _overlaySops);
			}

			public FusionTestData(CreateSopsDelegate baseSopCreatorDelegate, CreateSopsDelegate overlaySopCreatorDelegate)
			{
				_baseSops = baseSopCreatorDelegate.Invoke();
				_overlaySops = overlaySopCreatorDelegate.Invoke();
				_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSops, _overlaySops);
			}

			public IList<ImageSop> BaseSops
			{
				get { return _testDisplaySetGenerator.BaseSops; }
			}

			public IList<ImageSop> OverlaySops
			{
				get { return _testDisplaySetGenerator.OverlaySops; }
			}

			public IDisplaySet CreateBaseDisplaySet()
			{
				return _testDisplaySetGenerator.CreateBaseDisplaySet();
			}

			public IDisplaySet CreateOverlayDisplaySet()
			{
				return _testDisplaySetGenerator.CreateOverlayDisplaySet();
			}

			public IDisplaySet CreateFusionDisplaySet()
			{
				return _testDisplaySetGenerator.CreateFusionDisplaySet();
			}

			public void Dispose()
			{
				if (_testDisplaySetGenerator != null)
				{
					_testDisplaySetGenerator.Dispose();
					_testDisplaySetGenerator = null;
				}
				if (_overlaySops != null)
				{
					Dispose(_overlaySops);
					_overlaySops = null;
				}
				if (_baseSops != null)
				{
					Dispose(_baseSops);
					_baseSops = null;
				}
			}

			private static void Dispose<T>(IEnumerable<T> list) where T : IDisposable
			{
				foreach (var item in list)
					item.Dispose();
			}
		}
	}
}

#endif