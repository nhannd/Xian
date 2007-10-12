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

/*		[Test]
		public void Unsigned1()
		{
			double windowWidth = 1;
			double windowLevel = 11;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(10, 11);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[10]);
			Assert.AreEqual(255, lut[11]);
		}

		[Test]
		public void Signed1()
		{
			double windowWidth = 1;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-1,	0);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[-1]);
			Assert.AreEqual(255, lut[0]);
		}

		[Test]
		public void Unsigned12()
		{
			double windowWidth = 4096;
			double windowLevel = 2147;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(100, 4195);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[100]);
			Assert.AreEqual(255, lut[4195]);
			Assert.AreEqual(2147, lut.WindowCenter);
			Assert.AreEqual(4096, lut.WindowWidth);
		}

		[Test]
		public void Signed12()
		{
			double windowWidth = 4096;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[-2048]);
			Assert.AreEqual(127, lut[0]);
			Assert.AreEqual(255, lut[2047]);
		}

		[Test]
		public void AlterWindowLevel()
		{
			double windowWidth = 4096;
			double windowLevel = 2047;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(0, 4095);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[0]);
			Assert.AreEqual(127, lut[2047]);
			Assert.AreEqual(255, lut[4095]);

			lut.WindowWidth = 512;
			lut.WindowCenter = 1023;

			Assert.AreEqual(512, lut.WindowWidth);
			Assert.AreEqual(1023, lut.WindowCenter);
			Assert.AreEqual(0, lut[767]);
			Assert.AreEqual(255, lut[1279]);
		}

		[Test]
		public void Threshold()
		{
			double windowWidth = 1;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(0, lut[-2]);
			Assert.AreEqual(0, lut[-1]);
			Assert.AreEqual(255, lut[0]);
			Assert.AreEqual(255, lut[1]);
		}

		[Test]
		public void WindowWiderThanInputRange()
		{
			double windowWidth = 8192;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			Assert.AreEqual(63, lut[-2048]);
			Assert.AreEqual(127, lut[0]);
			Assert.AreEqual(191, lut[2047]);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			double windowWidth = 4096;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			int val = lut[-2049];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			double windowWidth = 4096;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;

			int val = lut[2048];
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ZeroWidth()
		{
			double windowWidth = 0;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(-2048, 2047);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void MinGreaterThanEqualToMax()
		{
			double windowWidth = 1;
			double windowLevel = 0;

			StatefulVoiLutLinear lut = new StatefulVoiLutLinear(1, 0);

			lut.WindowWidth = windowWidth;
			lut.WindowCenter = windowLevel;
		}*/
	}
}

#endif