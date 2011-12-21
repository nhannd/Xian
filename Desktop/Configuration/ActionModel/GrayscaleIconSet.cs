#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	//TODO: might be useful in core code some day
	internal sealed class GrayscaleIconSet : IconSet
	{
		public GrayscaleIconSet(IconSet source)
			: base(source.SmallIcon, source.MediumIcon, source.LargeIcon) {}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			//TODO: make unsafe. Not enabling unsafe code just for this, though.
			var bitmap = (Bitmap) base.CreateIcon(iconSize, resourceResolver);
			for (int x = 0; x < bitmap.Width; ++x)
			{
				for (int y = 0; y < bitmap.Height; ++y)
				{
					var pixel = bitmap.GetPixel(x, y);
					int gray = (int) (pixel.R*0.3f + pixel.G*0.59F + pixel.B*0.11f);
					if (gray > 255)
						gray = 255;

					bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, gray, gray, gray));
				}
			}
			return bitmap;
		}

		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			return base.GetIconKey(iconSize, resourceResolver) + "_ConvertedToGrayscale";
		}
	}
}