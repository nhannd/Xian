#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ModalityLUTLinearTest
	{
		public ModalityLUTLinearTest()
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
		public void Unsigned1()
		{
			int bitsStored = 1;
			bool isSigned = false;
			double rescaleSlope = 1;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(2, lut.Length);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(101, lut[1]);
		}

		[Test]
		public void Signed1()
		{
			int bitsStored = 1;
			bool isSigned = true;
			double rescaleSlope = 1;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(2, lut.Length);
			Assert.AreEqual(99, lut[-1]);
			Assert.AreEqual(100, lut[0]);
		}

		[Test]
		public void Unsigned12()
		{
			int bitsStored = 12;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(4096, lut.Length);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(1123, lut[2047]);
			Assert.AreEqual(2147, lut[4095]);
		}

		[Test]
		public void Signed12()
		{
			int bitsStored = 12;
			bool isSigned = true;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(4096, lut.Length);
			Assert.AreEqual(-924, lut[-2048]);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(1123, lut[2047]);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			int bitsStored = 12;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			int val = lut[-1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			int bitsStored = 12;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			int val = lut[4096];
		}

		[Test]
		[ExpectedException(typeof(DicomValidationException))]
		public void BitsStoredInvalid()
		{
			int bitsStored = 0;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);
		}

	}
}

#endif