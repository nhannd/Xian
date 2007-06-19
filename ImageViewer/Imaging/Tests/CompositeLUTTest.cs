#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;

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
			int pixelRepresentation = 0;
			double windowWidth = 128;
			double windowLevel = 74;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(255, modalityLUT.MaxInputValue);
			Assert.AreEqual(10, modalityLUT.MinOutputValue);
			Assert.AreEqual(137, modalityLUT.MaxOutputValue);
			Assert.AreEqual(10, modalityLUT[0]);
			Assert.AreEqual(137, modalityLUT[255]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(10, voiLUT.MinInputValue);
			Assert.AreEqual(137, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(0, voiLUT[10]);
			Assert.AreEqual(255, voiLUT[137]);
			Assert.AreEqual(128, voiLUT[74]);

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(0, lutComposer.OutputLUT[0]);
			Assert.AreEqual(255, lutComposer.OutputLUT[255]);
		}

		[Test]
		public void ComposeUnsigned12()
		{
			int bitsStored = 12;
			int pixelRepresentation = 0;
			double windowWidth = 2800;
			double windowLevel = 1600;
			double rescaleSlope = 0.683760684;
			double rescaleIntercept = 200;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(4095, modalityLUT.MaxInputValue);
			Assert.AreEqual(200, modalityLUT.MinOutputValue);
			Assert.AreEqual(3000, modalityLUT.MaxOutputValue);
			Assert.AreEqual(200, modalityLUT[0]);
			Assert.AreEqual(3000, modalityLUT[4095]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(200, voiLUT.MinInputValue);
			Assert.AreEqual(3000, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(0, voiLUT[200]);
			Assert.AreEqual(255, voiLUT[3000]);

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(0, lutComposer.OutputLUT[0]);
			Assert.AreEqual(255, lutComposer.OutputLUT[4095]);
		}

		[Test]
		public void ComposeUnsigned16()
		{
			int bitsStored = 16;
			int pixelRepresentation = 0;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(65535, modalityLUT.MaxInputValue);
			Assert.AreEqual(-1024, modalityLUT.MinOutputValue);
			Assert.AreEqual(64511, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-1024, modalityLUT[0]);
			Assert.AreEqual(64511, modalityLUT[65535]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(-1024, voiLUT.MinInputValue);
			Assert.AreEqual(64511, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(0, voiLUT[-1024]);
			Assert.AreEqual(255, voiLUT[64511]);

			// Left Window
			Assert.AreEqual(0, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(255, voiLUT[215]);
			// Window center
			Assert.AreEqual(127, voiLUT[40]);
			
			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(0, lutComposer.OutputLUT[890]);
			Assert.AreEqual(127, lutComposer.OutputLUT[1064]);
			Assert.AreEqual(255, lutComposer.OutputLUT[1239]);
		}

		[Test]
		public void ComposeSigned8()
		{
			// Use case:  Window width is 1
			int bitsStored = 8;
			int pixelRepresentation = 1;
			double windowWidth = 1;
			double windowLevel = 0;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-128, modalityLUT.MinInputValue);
			Assert.AreEqual(127, modalityLUT.MaxInputValue);
			Assert.AreEqual(-54, modalityLUT.MinOutputValue);
			Assert.AreEqual(73, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-54, modalityLUT[-128]);
			Assert.AreEqual(73, modalityLUT[127]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(-54, voiLUT.MinInputValue);
			Assert.AreEqual(73, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(0, voiLUT[-1]);
			Assert.AreEqual(255, voiLUT[0]);

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(255, lutComposer.OutputLUT[0]);
			Assert.AreEqual(255, lutComposer.OutputLUT[127]);
			Assert.AreEqual(0, lutComposer.OutputLUT[128]); // 128 = -128
			Assert.AreEqual(0, lutComposer.OutputLUT[234]); // 234 = -21
			Assert.AreEqual(255, lutComposer.OutputLUT[235]); // 235 = -20
		}

		[Test]
		public void ComposeSigned12()
		{
			int bitsStored = 12;
			int pixelRepresentation = 1;
			double windowWidth = 16384;
			double windowLevel = 4096;
			double rescaleSlope = 1.0;
			double rescaleIntercept = 0;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-2048, modalityLUT.MinInputValue);
			Assert.AreEqual(2047, modalityLUT.MaxInputValue);
			Assert.AreEqual(-2048, modalityLUT.MinOutputValue);
			Assert.AreEqual(2047, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-2048, modalityLUT[-2048]);
			Assert.AreEqual(2047, modalityLUT[2047]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(-2048, voiLUT.MinInputValue);
			Assert.AreEqual(2047, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(31, voiLUT[-2047]);
			Assert.AreEqual(63, voiLUT[0]);
			Assert.AreEqual(95, voiLUT[2047]);

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(63, lutComposer.OutputLUT[0]);
			Assert.AreEqual(95, lutComposer.OutputLUT[2047]);
			Assert.AreEqual(31, lutComposer.OutputLUT[2048]);
			Assert.AreEqual(63, lutComposer.OutputLUT[4095]);
		}

		[Test]
		public void ComposeSigned16()
		{
			int bitsStored = 16;
			int pixelRepresentation = 1;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-32768, modalityLUT.MinInputValue);
			Assert.AreEqual(32767, modalityLUT.MaxInputValue);
			Assert.AreEqual(-33792, modalityLUT.MinOutputValue);
			Assert.AreEqual(31743, modalityLUT.MaxOutputValue);
			Assert.AreEqual(-33792, modalityLUT[-32768]);
			Assert.AreEqual(31743, modalityLUT[32767]);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			Assert.AreEqual(-33792, voiLUT.MinInputValue);
			Assert.AreEqual(31743, voiLUT.MaxInputValue);
			Assert.AreEqual(0, voiLUT.MinOutputValue);
			Assert.AreEqual(255, voiLUT.MaxOutputValue);
			Assert.AreEqual(0, voiLUT[-33792]);
			Assert.AreEqual(255, voiLUT[31743]);

			// Left Window
			Assert.AreEqual(0, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(255, voiLUT[215]);
			// Window center
			Assert.AreEqual(127, voiLUT[40]);
			
			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);

			Assert.AreEqual(0, lutComposer.OutputLUT[890]);
			Assert.AreEqual(127, lutComposer.OutputLUT[1064]);
			Assert.AreEqual(255, lutComposer.OutputLUT[1239]);
		}

		[Test]
		public void ComposeSingleLUT()
		{
			double windowWidth = 350;
			double windowLevel = 40;

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(0, 4095);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(voiLUT);
			//lutComposer.Compose();
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			int bitsStored = 16;
			int pixelRepresentation = 1;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);
			//lutComposer.Compose();
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			int bitsStored = 16;
			int pixelRepresentation = 1;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);
			//lutComposer.Compose();
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void NoLUTsAdded()
		{
			LUTComposer lutComposer = new LUTComposer();
			//lutComposer.Compose();
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void InputOutputRangeDoNotMatch()
		{
			int bitsStored = 16;
			int pixelRepresentation = 1;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			StatefulVoiLutLinear voiLUT = new StatefulVoiLutLinear(0, 65535);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			lutComposer.LUTCollection.Add(voiLUT);
			//lutComposer.Compose();
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void LastOutputRangeNot8Bit()
		{
			int bitsStored = 16;
			int pixelRepresentation = 1;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			ModalityLUTLinear modalityLUT = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			LUTComposer lutComposer = new LUTComposer();
			lutComposer.LUTCollection.Add(modalityLUT);
			//lutComposer.Compose();
		}
	}
}

#endif