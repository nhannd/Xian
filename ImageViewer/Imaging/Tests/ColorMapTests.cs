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

using System.Globalization;
using NUnit.Framework;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ColorMapTests
	{
		public ColorMapTests()
		{
		}

		[Test]
		public void Test12Unsigned()
		{
			GrayscaleColorMap colorMap = new GrayscaleColorMap();
			colorMap.MinInputValue = 0;
			colorMap.MaxInputValue = 4095;
			
			Assert.IsTrue(colorMap.Length == 4096);

			Color color = Color.FromArgb(colorMap.Data[0]);
			Assert.IsTrue(255 == color.A && 0 == color.R && color.R == color.G && color.G == color.B);

			color = Color.FromArgb(colorMap.Data[2047]);
			Assert.IsTrue(127 == color.R && color.R == color.G && color.G == color.B);

			color = Color.FromArgb(colorMap.Data[4095]);
			Assert.IsTrue(255 == color.R && color.R == color.G && color.G == color.B);
		}

		public void Test12Signed()
		{
			GrayscaleColorMap colorMap = new GrayscaleColorMap();
			colorMap.MinInputValue = -2048;
			colorMap.MaxInputValue = 2047;

			Assert.IsTrue(colorMap.Length == 4096);

			Assert.AreEqual(colorMap.Data[0], colorMap[-2048]);
			Color color = Color.FromArgb(colorMap.Data[0]);
			Assert.IsTrue(255 == color.A && 0 == color.R && color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[2048], colorMap[0]);
			color = Color.FromArgb(colorMap.Data[2048]);
			Assert.IsTrue(127 == color.R && color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[4095], colorMap[2047]);
			color = Color.FromArgb(colorMap.Data[4095]);
			Assert.IsTrue(255 == color.R && color.R == color.G && color.G == color.B);
		}
	}
}

#endif