#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class NormalizationLutLinearTests
	{
		[Test]
		public void TestTrivialRescale()
		{
			const double rescaleSlope = 1;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}

		[Test]
		public void TestNormalRescale1()
		{
			const double rescaleSlope = 1.9;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}

		[Test]
		public void TestNormalRescale2()
		{
			const double rescaleSlope = 0.9;
			const double rescaleIntercept = 406;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}

		[Test]
		public void TestNormalRescale3()
		{
			const double rescaleSlope = 1;
			const double rescaleIntercept = -625;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}

		[Test]
		public void TestSubnormalRescale1()
		{
			const double rescaleSlope = 1.5e-9;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}

		[Test]
		public void TestSubnormalRescale2()
		{
			const double rescaleSlope = 1.5e-9;
			const double rescaleIntercept = 351;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			Assert.AreEqual(0, composer[0]);
			Assert.AreEqual(101, composer[101]);
			Assert.AreEqual(2047, composer[2047]);
			Assert.AreEqual(2048, composer[2048]);
			Assert.AreEqual(3993, composer[3993]);
			Assert.AreEqual(4095, composer[4095]);
		}
	}
}

#endif