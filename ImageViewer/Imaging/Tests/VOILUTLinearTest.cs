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

using System;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class VOILUTLinearTest
	{
		public VOILUTLinearTest()
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
		public void BasicTest()
		{
			double windowWidth = 4096;
			double windowLevel = 0;

			BasicVoiLutLinear lut = new BasicVoiLutLinear();
			lut.MinInputValue = -2048;
			lut.MaxInputValue = 2047;

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(-2048, lut[-2048]);
			Assert.AreEqual(0, lut[0]);
			Assert.AreEqual(2047, lut[2047]);
		}

		[Test]
		public void AlterWindowLevel()
		{
			double windowWidth = 4096;
			double windowLevel = 2047;

			BasicVoiLutLinear lut = new BasicVoiLutLinear();
			lut.MinInputValue = 0;
			lut.MaxInputValue = 4095;

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[0]);
			Assert.AreEqual(2048, lut[2047]);
			Assert.AreEqual(4095, lut[4095]);

			lut.WindowWidth = 512;
			lut.WindowCenter = 1023;

			Assert.AreEqual(512, lut.WindowWidth);
			Assert.AreEqual(1023, lut.WindowCenter);
			Assert.AreEqual(0, lut[767]);
			Assert.AreEqual(4095, lut[1279]);
		}

		[Test]
		public void Threshold()
		{
			double windowWidth = 1;
			double windowLevel = 0;

			BasicVoiLutLinear lut = new BasicVoiLutLinear();
			lut.MinInputValue = -2048;
			lut.MaxInputValue = 2047;

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(-2048, lut[-2]);
			Assert.AreEqual(-2048, lut[-1]);
			Assert.AreEqual(2047, lut[0]);
			Assert.AreEqual(2047, lut[1]);
		}
	}
}

#endif