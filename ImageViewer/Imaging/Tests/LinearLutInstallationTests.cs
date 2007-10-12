#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using NUnit.Framework;

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

/*		[Test]
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
			IMockImageSopSetters setters = (IMockImageSopSetters)imageSop;

			setters.PixelRepresentation = pixelRepresentation;
			setters.BitsStored = bitsStored;
			setters.WindowCenterAndWidth = new Window[] { new Window(windowWidth, windowCenter) };

			BasicPresentationImage image = new BasicPresentationImage(imageSop);

			//WindowLevelOperator.InstallVOILUTLinear(image);
			//IVOILUTLinear lut = image.VoiLut;
			
			//Assert.AreEqual(lut.WindowWidth, expectedWindowWidth);
			//Assert.AreEqual(lut.WindowCenter, expectedWindowCenter);
		}
*/
	}
}

#endif