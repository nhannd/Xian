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