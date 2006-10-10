using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Layers;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A common (abstract) class for creating different interpolators.  Later, we can use an ExtensionPoint.
	/// </summary>
	internal abstract class ImageInterpolator
	{
		public static ImageInterpolator AllocateInterpolator(ImageLayer.InterpolationMethods interpolationMethod)
		{
			if (interpolationMethod == ImageLayer.InterpolationMethods.BILINEAR)
				return new ImageInterpolatorBilinear();

			if (interpolationMethod == ImageLayer.InterpolationMethods.BILINEAR_FAST)
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
