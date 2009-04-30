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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class CompositeLUTTest
	{
		public CompositeLUTTest()
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
		public void ComposeUnsigned8()
		{
			int bitsStored = 8;
			bool isSigned = false;
			double windowWidth = 128;
			double windowLevel = 74;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(255, modalityLUT.MaxInputValue);
			Assert.AreEqual(10, modalityLUT.MinOutputValue);
			Assert.AreEqual(137, modalityLUT.MaxOutputValue);
			Assert.AreEqual(10, modalityLUT[0]);
			Assert.AreEqual(137, modalityLUT[255]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);

			Assert.AreEqual(10, voiLUT.MinInputValue);
			Assert.AreEqual(137, voiLUT.MaxInputValue);
			Assert.AreEqual(10, voiLUT.MinOutputValue);
			Assert.AreEqual(137, voiLUT.MaxOutputValue);
			Assert.AreEqual(10, voiLUT[10]);
			Assert.AreEqual(137, voiLUT[137]);
			Assert.AreEqual(73, voiLUT[73]);

			Assert.AreEqual(0, lutComposer.MinInputValue);
			Assert.AreEqual(255, lutComposer.MaxInputValue);
			Assert.AreEqual(10, lutComposer.MinOutputValue);
			Assert.AreEqual(137, lutComposer.MaxOutputValue);

			Assert.AreEqual(10, lutComposer[0]);
			Assert.AreEqual(137, lutComposer[255]);
			Assert.AreEqual(73, lutComposer[127]);
		}

		[Test]
		public void ComposeUnsigned12()
		{
			int bitsStored = 12;
			bool isSigned = false;
			double windowWidth = 2800;
			double windowLevel = 1600;
			double rescaleSlope = 0.683760684;
			double rescaleIntercept = 200;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(4095, modalityLUT.MaxInputValue);
			Assert.AreEqual(200, modalityLUT.MinOutputValue);
			Assert.AreEqual(3000, modalityLUT.MaxOutputValue);
			Assert.AreEqual(200, modalityLUT[0]);
			Assert.AreEqual(3000, modalityLUT[4095]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);

			Assert.AreEqual(200, voiLUT.MinInputValue);
			Assert.AreEqual(3000, voiLUT.MaxInputValue);
			Assert.AreEqual(200, voiLUT.MinOutputValue);
			Assert.AreEqual(3000, voiLUT.MaxOutputValue);
			Assert.AreEqual(200, voiLUT[200]);
			Assert.AreEqual(3000, voiLUT[3000]);
			Assert.AreEqual(1600, voiLUT[1600]);

			Assert.AreEqual(200, lutComposer.Data[0]);
			Assert.AreEqual(3000, lutComposer.Data[4095]);
			Assert.AreEqual(1600, lutComposer.Data[2048]);
		}

		[Test]
		public void ComposeUnsigned16()
		{
			int bitsStored = 16;
			bool isSigned = false;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(65535, modalityLUT.MaxInputValue);
			Assert.AreEqual(-1024, modalityLUT.MinOutputValue);
			Assert.AreEqual(64511, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-1024, modalityLUT[0]);
			Assert.AreEqual(64511, modalityLUT[65535]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);

			Assert.AreEqual(-1024, voiLUT.MinInputValue);
			Assert.AreEqual(64511, voiLUT.MaxInputValue);
			Assert.AreEqual(-1024, voiLUT.MinOutputValue);
			Assert.AreEqual(64511, voiLUT.MaxOutputValue);

			// Left Window
			Assert.AreEqual(-1024, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(64511, voiLUT[215]);
			// Window center
			// 31837 is correct according to Dicom: See PS3.5 C.11.2.1.2 for the calculation.
			// Although you might think it should be 31744 (65535/2 - 1024), it is not.
			Assert.AreEqual(31837, voiLUT[40]);
			
			Assert.AreEqual(-1024, lutComposer.Data[0]);
			Assert.AreEqual(64511, lutComposer.Data[65535]);
			Assert.AreEqual(31837, lutComposer.Data[1064]);
		}

		[Test]
		public void ComposeSigned8()
		{
			// Use case:  Window width is 1
			int bitsStored = 8;
			bool isSigned = true;
			double windowWidth = 1;
			double windowLevel = 0;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-128, modalityLUT.MinInputValue);
			Assert.AreEqual(127, modalityLUT.MaxInputValue);
			Assert.AreEqual(-54, modalityLUT.MinOutputValue);
			Assert.AreEqual(73, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-54, modalityLUT[-128]);
			Assert.AreEqual(73, modalityLUT[127]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);

			Assert.AreEqual(-54, voiLUT.MinInputValue);
			Assert.AreEqual(73, voiLUT.MaxInputValue);
			Assert.AreEqual(-54, voiLUT.MinOutputValue);
			Assert.AreEqual(73, voiLUT.MaxOutputValue);
			Assert.AreEqual(-54, voiLUT[-1]);
			Assert.AreEqual(73, voiLUT[0]);

			Assert.AreEqual(-54, lutComposer.Data[0]);
			Assert.AreEqual(-54, lutComposer.Data[106]);
			Assert.AreEqual(73, lutComposer.Data[107]);
		}

		[Test]
		public void ComposeSigned12()
		{
			int bitsStored = 12;
			bool isSigned = true;
			double windowWidth = 16384;
			double windowLevel = 4096;
			double rescaleSlope = 1.0;
			double rescaleIntercept = 0;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-2048, modalityLUT.MinInputValue);
			Assert.AreEqual(2047, modalityLUT.MaxInputValue);
			Assert.AreEqual(-2048, modalityLUT.MinOutputValue);
			Assert.AreEqual(2047, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-2048, modalityLUT[-2048]);
			Assert.AreEqual(2047, modalityLUT[2047]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);
			lutComposer.LutCollection.Add(new GrayscaleColorMap());

			Assert.AreEqual(-2048, voiLUT.MinInputValue);
			Assert.AreEqual(2047, voiLUT.MaxInputValue);
			Assert.AreEqual(-2048, voiLUT.MinOutputValue);
			Assert.AreEqual(2047, voiLUT.MaxOutputValue);
			Assert.AreEqual(-1535, voiLUT[-2047]);
			Assert.AreEqual(-1024, voiLUT[0]);
			Assert.AreEqual(-512, voiLUT[2047]);

			//This test is a little different from the others, it tests the output using a grayscale color map.
			Assert.AreEqual(31, 0x000000ff & lutComposer.Data[0]);
			Assert.AreEqual(63, 0x000000ff & lutComposer.Data[2048]);
			Assert.AreEqual(95, 0x000000ff & lutComposer.Data[4095]);
		}

		[Test]
		public void ComposeSigned16()
		{
			int bitsStored = 16;
			bool isSigned = true;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-32768, modalityLUT.MinInputValue);
			Assert.AreEqual(32767, modalityLUT.MaxInputValue);
			Assert.AreEqual(-33792, modalityLUT.MinOutputValue);
			Assert.AreEqual(31743, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-33792, modalityLUT[-32768]);
			Assert.AreEqual(31743, modalityLUT[32767]);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(modalityLUT);
			lutComposer.LutCollection.Add(voiLUT);

			Assert.AreEqual(-33792, voiLUT.MinInputValue);
			Assert.AreEqual(31743, voiLUT.MaxInputValue);
			Assert.AreEqual(-33792, voiLUT.MinOutputValue);
			Assert.AreEqual(31743, voiLUT.MaxOutputValue);

			// Left Window
			Assert.AreEqual(-33792, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(31743, voiLUT[215]);
			// Window center
			Assert.AreEqual(-930, voiLUT[40]);
		}

		[Test]
		public void ComposeSingleLUT()
		{
			double windowWidth = 350;
			double windowLevel = 40;

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.MinInputValue = 0;
			voiLUT.MaxInputValue = 4095;

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer();
			lutComposer.LutCollection.Add(voiLUT);
			int[] data = lutComposer.Data;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void NoLUTsAdded()
		{
			LutComposer lutComposer = new LutComposer();
			int[] data = lutComposer.Data;
		}
	}
}

#endif