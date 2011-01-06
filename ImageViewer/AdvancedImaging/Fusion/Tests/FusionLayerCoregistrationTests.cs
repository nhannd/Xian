#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	//TODO (CR Sept 2010): these tests create somewhat "ideal" data sets that are completely coincident in the patient space.
	//Ideally, I'd like to see some tests where the volumes are slightly offset from each other in the patient space.

	//TODO (CR Sept 2010): In general, I'm not sure diffing the images is the right approach for the coregistration tests.  Might
	//be more appropriate to inspect values at certain known locations.

	[TestFixture(Description = "Tests for validating fused image pixel data alignment and orientation (including MPR of overlay data)")]
	public class FusionLayerCoregistrationTests
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
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
			using (var data = new FusionTestDataContainer(TestDataFunction.Threed,
			                                              new Vector3D(0.8f, 0.9f, 1.0f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit,
			                                              new Vector3D(0.9f, 1.0f, 0.8f), Vector3D.yUnit, Vector3D.zUnit, Vector3D.xUnit))
			{
				var results = DiffFusionImages(data, testName);

				for (int n = 0; n < results.Count; n++)
					Assert.Less(results[n], 0.025, "{0}:: slice {1} exceeds difference limit.", testName, n);
			}
		}

		private static IList<double> DiffFusionImages(FusionTestDataContainer data, string testName)
		{
			var outputPath = new DirectoryInfo(Path.Combine(typeof (FusionLayerCoregistrationTests).FullName, testName));
			if (outputPath.Exists)
				outputPath.Delete(true);
			outputPath.Create();

			using (var log = File.CreateText(Path.Combine(outputPath.FullName, "data.csv")))
			{
				//TODO (CR Sept 2010): this is testing that the fused image is essentially unchanged from the "base" image
				//by applying a grayscale color map to the overlay image?  If there were a bug that caused the overlay image
				//to not be rendered at all, this test would pass.

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
							diff.Save(Path.Combine(outputPath.FullName, string.Format("diff{0}.png", index)));
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
	}
}

#endif