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

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{

	[TestFixture]
	public class MiscellaneousLutTests
	{
		public MiscellaneousLutTests()
		{
		}

		[Test]
		public void TestSimpleLut()
		{
			int bitsStored = 8;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			ModalityLutLinear modalityLUT = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			SimpleDataModalityLut simpleLut = 
				new SimpleDataModalityLut(modalityLUT.MinInputValue, modalityLUT.Data, modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue, modalityLUT.GetKey(), modalityLUT.GetDescription()); 

			Assert.AreEqual(modalityLUT.MinInputValue, simpleLut.MinInputValue);
			Assert.AreEqual(modalityLUT.MaxInputValue, simpleLut.MaxInputValue);
			Assert.AreEqual(modalityLUT.MinOutputValue, simpleLut.MinOutputValue);
			Assert.AreEqual(modalityLUT.MaxOutputValue, simpleLut.MaxOutputValue);
			Assert.AreEqual(modalityLUT.GetKey(), simpleLut.GetKey());
			Assert.AreEqual(modalityLUT.GetDescription(), simpleLut.GetDescription());
			Assert.AreEqual(modalityLUT.Length, simpleLut.Length);
		}
	}
}

#endif