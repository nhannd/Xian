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
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// Creates various test pattern <see cref="ImageGraphic"/>s.
	/// </summary>
	internal static class TestPattern
	{
		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of red, blue, green and black in the NW, NE, SW, SE corners respectively.
		/// </summary>
		public static ColorImageGraphic CreateRGBKCorners(Size size)
		{
			int width = size.Width;
			int height = size.Height;
			int halfWidth = width/2;
			int halfHeight = height/2;

			ColorImageGraphic imageGraphic = new ColorImageGraphic(height, width);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Color c;
					if (x < halfWidth)
						c = y < halfHeight ? Color.Red : Color.LimeGreen;
					else
						c = y < halfHeight ? Color.Blue : Color.Black;

					imageGraphic.PixelData.SetPixel(x, y, c);
				}
			}
			return imageGraphic;
		}

		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of a NW to SE black to white gradient.
		/// </summary>
		public static GrayscaleImageGraphic CreateGraydient(Size size)
		{
			int width = size.Width;
			int height = size.Height;

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(height, width);

			int range = (1 << imageGraphic.BitsStored) - 1;
			int offset = imageGraphic.IsSigned ? -(1 << (imageGraphic.BitsStored - 1)) : 0;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int v = (int) (1f*(x+y)/(width+height)*range + offset);
					imageGraphic.PixelData.SetPixel(x, y, v);
				}
			}
			return imageGraphic;
		}

		/// <summary>
		/// Creates a test pattern of <paramref name="size"/> consisting of a black and white checkboard.
		/// </summary>
		public static GrayscaleImageGraphic CreateCheckerboard(Size size)
		{
			int width = size.Width;
			int height = size.Height;

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(height, width);

			int minValue = imageGraphic.IsSigned ? -(1 << (imageGraphic.BitsStored - 1)) : 0;
			int maxValue = (1 << imageGraphic.BitsStored) + minValue - 1;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int file = (int) (8f*x/width);
					int rank = (int) (8f*y/height);
					int v = (file%2 == 0) ^ (rank%2 == 0) ? minValue : maxValue;
					imageGraphic.PixelData.SetPixel(x, y, v);
				}
			}

			return imageGraphic;
		}
	}
}

#endif