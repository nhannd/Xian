using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A common (abstract) class for creating different interpolators.  Later, we can use an ExtensionPoint.
	/// </summary>
	internal abstract class ImageInterpolator
	{
		public static ImageInterpolator AllocateInterpolator(ImageGraphic.InterpolationMethods interpolationMethod)
		{
			if (interpolationMethod == ImageGraphic.InterpolationMethods.Bilinear)
				return new ImageInterpolatorBilinear();

			if (interpolationMethod == ImageGraphic.InterpolationMethods.FastBilinear)
				return new ImageInterpolatorBilinearFast();

			return new ImageInterpolatorNearestNeighbour();
		}

		public abstract unsafe void Interpolate(
			Rectangle srcRegionRect,
			byte* pSrcPixelData,
			int srcWidth,
			int srcHeight,
			int srcBytesPerPixel,
			Rectangle dstRegionRect,
			byte* pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			bool swapXY,
			byte* pLutData,
			bool isRGB,
			bool isPlanar,
			bool IsSigned);
	}
}
