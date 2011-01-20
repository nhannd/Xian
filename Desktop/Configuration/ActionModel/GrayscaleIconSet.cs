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
	//TODO: might be useful in core, but not right now.
	internal class GrayscaleIconSet : IconSet
	{
		private readonly IconSet _source;

		public GrayscaleIconSet(IconSet source)
			: base(IconScheme.Monochrome, source.SmallIcon, source.MediumIcon, source.LargeIcon)
		{
			_source = source;
		}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			//Already gray.
			if (_source.Scheme == IconScheme.Monochrome)
				return _source.CreateIcon(iconSize, resourceResolver);
			
			//TODO: make unsafe. Not enabling unsafe code just for this, though.
			var colorImage = (Bitmap)base.CreateIcon(iconSize, resourceResolver);
			for (int x = 0; x < colorImage.Width; ++x)
			{
				for (int y = 0; y < colorImage.Height; ++y)
				{
					var pixel = colorImage.GetPixel(x, y);
					int gray = (int)(pixel.R * 0.3f + pixel.G * 0.59F + pixel.B * 0.11f);
					if (gray > 255)
						gray = 255;

					colorImage.SetPixel(x, y, Color.FromArgb(pixel.A, gray, gray, gray));
				}
			}

			return colorImage;
		}

		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			if (_source.Scheme == IconScheme.Monochrome)
				return base.GetIconKey(iconSize, resourceResolver);

			return base.GetIconKey(iconSize, resourceResolver) + "_ConvertedToGrayscale";
		}
	}
}