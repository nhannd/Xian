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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests;
using ClearCanvas.ImageViewer.Tests;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	internal static class ContainsPointTest
	{
		public static bool CheckImageIsBlackWhite(Bitmap image, double tolerance)
		{
			List<double> list = new List<double>();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Color c = image.GetPixel(x, y);
					double v = (c.R + c.G + c.B) / 3.0;
					list.Add(Math.Min(255 - v, v));
				}
			}
			Statistics stats = new Statistics(list);
			Trace.WriteLine(string.Format("CheckImageIsBlackWhite: {0}", stats.ToString()));
			return stats.IsEqualTo(0, tolerance);
		}
	}
}

#endif