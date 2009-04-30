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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class MinMaxPixelCalculatedLutTests
	{
		public MinMaxPixelCalculatedLutTests()
		{
		}

		[Test]
		public void TestSimple()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			GrayscalePixelData pixelData = new GrayscalePixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(lut.WindowWidth, 24);
			Assert.AreEqual(lut.WindowCenter, 12);
		}

		[Test]
		public void TestWithModalityLut()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			ModalityLutLinear modalityLut = new ModalityLutLinear(8, true, 1.0, -10);
			GrayscalePixelData pixelData = new GrayscalePixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData, modalityLut);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(lut.WindowWidth, 24);
			Assert.AreEqual(lut.WindowCenter, 2);
		}
	}
}

#endif