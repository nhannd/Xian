#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class LinearLutInstallationTests
	{
		public LinearLutInstallationTests()
		{
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
		public void TestNoWindowLevelSpecified()
		{
			TestSingleConfiguration(0, 7, double.NaN, double.NaN, 128, 64);
			TestSingleConfiguration(0, 8, double.NaN, double.NaN, 256, 128);
			TestSingleConfiguration(0, 10, double.NaN, double.NaN, 1024, 512);
			TestSingleConfiguration(0, 11, double.NaN, double.NaN, 2048, 1024);
			TestSingleConfiguration(0, 12, double.NaN, double.NaN, 4096, 2048);
			TestSingleConfiguration(0, 13, double.NaN, double.NaN, 8192, 4096);
			TestSingleConfiguration(0, 14, double.NaN, double.NaN, 16384, 8192);
			TestSingleConfiguration(0, 15, double.NaN, double.NaN, 32768, 16384);
			TestSingleConfiguration(0, 16, double.NaN, double.NaN, 65536, 32768);

			TestSingleConfiguration(1, 7, double.NaN, double.NaN, 128, 0);
			TestSingleConfiguration(1, 8, double.NaN, double.NaN, 256, 0);
			TestSingleConfiguration(1, 10, double.NaN, double.NaN, 1024, 0);
			TestSingleConfiguration(1, 11, double.NaN, double.NaN, 2048, 0);
			TestSingleConfiguration(1, 12, double.NaN, double.NaN, 4096, 0);
			TestSingleConfiguration(1, 13, double.NaN, double.NaN, 8192, 0);
			TestSingleConfiguration(1, 14, double.NaN, double.NaN, 16384, 0);
			TestSingleConfiguration(1, 15, double.NaN, double.NaN, 32768, 0);
			TestSingleConfiguration(1, 16, double.NaN, double.NaN, 65536, 0);
		}

		[Test]
		public void TestOnlyWindowSpecified()
		{
			TestSingleConfiguration(1, 10, 411, double.NaN, 411, 0);
			TestSingleConfiguration(0, 10, 411, double.NaN, 411, 205);
		}

		[Test]
		public void TestOnlyLevelSpecified()
		{
			TestSingleConfiguration(1, 10, double.NaN, 410, 1024, 410); 
			TestSingleConfiguration(0, 10, double.NaN, 410, 1024, 410);
		}

		[Test]
		public void TestWindowLevelSpecified()
		{
			TestSingleConfiguration(1, 10, 411, 205, 411, 205);
		}

		public void TestSingleConfiguration(
			int pixelRepresentation,
			int bitsStored,
			double windowWidth,
			double windowCenter,
			double expectedWindowWidth,
			double expectedWindowCenter)
		{
			MockImageSop imageSop = new MockImageSop();
			
			imageSop.PixelRepresentation = pixelRepresentation;
			imageSop.BitsStored = bitsStored;
			imageSop.WindowWidth = windowWidth;
			imageSop.WindowCenter = windowCenter;

			DicomPresentationImage image = new DicomPresentationImage(imageSop);

			WindowLevelOperator.InstallVOILUTLinear(image);
			VOILUTLinear lut = image.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.VoiLUT as VOILUTLinear;
			
			Assert.AreEqual(lut.WindowWidth, expectedWindowWidth);
			Assert.AreEqual(lut.WindowCenter, expectedWindowCenter);
		}
	}
}

#endif