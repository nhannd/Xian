using System;
using System.Runtime.InteropServices;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal unsafe class ImageInterpolatorBilinear : ImageInterpolator
	{
		private static readonly float _floatSlightlyMoreThanOne = 1.001F;
		
		/// <summary>
		/// Implements the True (floating point) Bilinear Interpolation algorithm.
		/// </summary>
		/// <remarks>
		/// This code (in C# or C++) is far too slow to be used (at least while stacking).  It could be used, however,
		/// in conjunction with the Fast! bilinear interpolation algorithm to improve the stacking experience,  hence
		/// why the code is still here.  Basically, we could use the fast interpolation while doing anything dynamic
		/// (stacking, w/l, zoom, pan, etc) and then when the user stops the operation, redraw using this algorithm.
		/// Regardless, both methods still require a bit more work in terms of speeding them up, probably by writing
		/// at least part of them in assembly (inner 'x' loop).
		/// </remarks>
		public override unsafe void Interpolate(
			Rectangle srcRegionRectangle,
			byte* pSrcPixelData,
			int srcWidth,
			int srcHeight,
			int srcBytesPerPixel,
			Rectangle dstRegionRectangle,
			byte* pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			bool swapXY,
			int* pLutData,
			bool isRGB,
			bool isPlanar,
			bool IsSigned)
		{
			int dstRegionHeight, dstRegionWidth,
				xDstStride, yDstStride,
				xDstIncrement, yDstIncrement;

			if (swapXY)
			{
				dstRegionHeight = Math.Abs(dstRegionRectangle.Right - dstRegionRectangle.Left);
				dstRegionWidth = Math.Abs(dstRegionRectangle.Bottom - dstRegionRectangle.Top);
				xDstStride = dstWidth * dstBytesPerPixel;
				yDstStride = dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRectangle.Bottom - dstRegionRectangle.Top) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRectangle.Right - dstRegionRectangle.Left) * yDstStride;

				dstRegionRectangle = RectangleUtilities.MakeRectangleZeroBased(dstRegionRectangle);
				pDstPixelData += (dstRegionRectangle.Top * xDstStride) + (dstRegionRectangle.Left * yDstStride);
			}
			else
			{
				dstRegionHeight = Math.Abs(dstRegionRectangle.Bottom - dstRegionRectangle.Top);
				dstRegionWidth = Math.Abs(dstRegionRectangle.Right - dstRegionRectangle.Left);
				xDstStride = dstBytesPerPixel;
				yDstStride = dstWidth * dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRectangle.Right - dstRegionRectangle.Left) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRectangle.Bottom - dstRegionRectangle.Top) * yDstStride;

				dstRegionRectangle = RectangleUtilities.MakeRectangleZeroBased(dstRegionRectangle); 
				pDstPixelData += (dstRegionRectangle.Top * yDstStride) + (dstRegionRectangle.Left * xDstStride);
			}

			int xSrcStride, ySrcStride;

			if (isRGB && !isPlanar)
			{
				xSrcStride = 3;
				ySrcStride = srcWidth * 3;
			}
			else
			{
				xSrcStride = srcBytesPerPixel;
				ySrcStride = srcWidth * srcBytesPerPixel;
			}

			int srcNextChannelOffset = 0;
			if (isRGB)
			{
				if (!isPlanar)
					srcNextChannelOffset = 1;
				else
					srcNextChannelOffset = srcWidth * srcHeight;
			}

			int srcRegionWidth = srcRegionRectangle.Right - srcRegionRectangle.Left;
			int srcRegionHeight = srcRegionRectangle.Bottom - srcRegionRectangle.Top;

			float srcSlightlyLessThanWidthMinusOne = (float)srcWidth - _floatSlightlyMoreThanOne;
			float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - _floatSlightlyMoreThanOne;

			float xRatio = (float)srcRegionWidth / dstRegionWidth;
			float yRatio = (float)srcRegionHeight / dstRegionHeight;

			int[] xSrcPixelCoordinates = new int[dstRegionWidth];
			float[] dxValuesAtXSrcPixelCoordinates = new float[dstRegionWidth];

			// Calculate these values outside of the y-x loop.  Improves speed by a lot.
			for (int x = 0; x < dstRegionWidth; ++x)
			{
				float xSrcPixelCoordinate = srcRegionRectangle.Left + ((float)x + 0.5F) * xRatio;

				//a necessary evil, I'm afraid.
				if (xSrcPixelCoordinate < 0)
					xSrcPixelCoordinate = 0;
				if (xSrcPixelCoordinate > srcSlightlyLessThanWidthMinusOne)
					xSrcPixelCoordinate = srcSlightlyLessThanWidthMinusOne; //force it to be just barely before the last pixel.

				xSrcPixelCoordinates[x] = (int)xSrcPixelCoordinate;
				dxValuesAtXSrcPixelCoordinates[x] = ((xSrcPixelCoordinate - (float)xSrcPixelCoordinates[x]));
			}

			bool is8Bit = (srcBytesPerPixel == 1);

			for (int y = 0; y < dstRegionHeight; ++y)
			{
				int* pRowDstPixelData = (int*)pDstPixelData;

				float ySrcPixelCoordinate = srcRegionRectangle.Top + ((float)y + 0.5F) * yRatio;

				//a necessary evil, I'm afraid.
				if (ySrcPixelCoordinate < 0)
					ySrcPixelCoordinate = 0;
				else if (ySrcPixelCoordinate > srcSlightlyLessThanHeightMinusOne)
					ySrcPixelCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

				int ySrcPixel = (int)ySrcPixelCoordinate;
				float dy = ySrcPixelCoordinate - (float)ySrcPixel;

				byte* pRowSrcPixelData = pSrcPixelData + ySrcPixel * ySrcStride;

				for (int x = 0; x < dstRegionWidth; ++x)
				{
					if (!isRGB)
					{
						if (!is8Bit)
						{
							if (!IsSigned)
							{
								ushort* pSrcPixel00 = (ushort*)pRowSrcPixelData + xSrcPixelCoordinates[x];
								ushort* pSrcPixel01 = pSrcPixel00 + 1;
								ushort* pSrcPixel10 = pSrcPixel00 + srcWidth;
								ushort* pSrcPixel11 = pSrcPixel10 + 1;

								float yInterpolated1 = (float)(*pSrcPixel00) + (*pSrcPixel10 - *pSrcPixel00) * dy;
								float yInterpolated2 = (float)(*pSrcPixel01) + (*pSrcPixel11 - *pSrcPixel01) * dy;
								float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dxValuesAtXSrcPixelCoordinates[x];
								int value = pLutData[(ushort)interpolated];
								*pRowDstPixelData = value;
							}
							else
							{
								short* pSrcPixel00 = (short*)pRowSrcPixelData + xSrcPixelCoordinates[x];
								short* pSrcPixel01 = pSrcPixel00 + 1;
								short* pSrcPixel10 = pSrcPixel00 + srcWidth;
								short* pSrcPixel11 = pSrcPixel10 + 1;

								float yInterpolated1 = (float)(*pSrcPixel00) + (*pSrcPixel10 - *pSrcPixel00) * dy;
								float yInterpolated2 = (float)(*pSrcPixel01) + (*pSrcPixel11 - *pSrcPixel01) * dy;

								float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dxValuesAtXSrcPixelCoordinates[x];
								int value = pLutData[(ushort)interpolated];
								*pRowDstPixelData = value;
							}
						}
						else
						{
							if (!IsSigned)
							{
								byte* pSrcPixel00 = (byte*)pRowSrcPixelData + xSrcPixelCoordinates[x];
								byte* pSrcPixel01 = pSrcPixel00 + 1;
								byte* pSrcPixel10 = pSrcPixel00 + srcWidth;
								byte* pSrcPixel11 = pSrcPixel10 + 1;

								float yInterpolated1 = (float)(*pSrcPixel00) + (*pSrcPixel10 - *pSrcPixel00) * dy;
								float yInterpolated2 = (float)(*pSrcPixel01) + (*pSrcPixel11 - *pSrcPixel01) * dy;

								float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dxValuesAtXSrcPixelCoordinates[x];
								int value = pLutData[(byte)interpolated];
								*pRowDstPixelData = value;
							}
							else
							{
								sbyte* pSrcPixel00 = (sbyte*)pRowSrcPixelData + xSrcPixelCoordinates[x];
								sbyte* pSrcPixel01 = pSrcPixel00 + 1;
								sbyte* pSrcPixel10 = pSrcPixel00 + srcWidth;
								sbyte* pSrcPixel11 = pSrcPixel10 + 1;

								float yInterpolated1 = (float)(*pSrcPixel00) + (*pSrcPixel10 - *pSrcPixel00) * dy;
								float yInterpolated2 = (float)(*pSrcPixel01) + (*pSrcPixel11 - *pSrcPixel01) * dy;

								float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dxValuesAtXSrcPixelCoordinates[x];
								int value = pLutData[(byte)interpolated];
								*pRowDstPixelData = value;
							}
						}
					}
					else
					{
						pRowDstPixelData[3] = 0xff;  //A

						byte* pSrcPixel00 = pRowSrcPixelData + xSrcPixelCoordinates[x] * xSrcStride;
						float dx = dxValuesAtXSrcPixelCoordinates[x];

						for (int i = 0; i < 3; ++i)
						{
							byte* pSrcPixel01 = pSrcPixel00 + xSrcStride;
							byte* pSrcPixel10 = pSrcPixel00 + ySrcStride;
							byte* pSrcPixel11 = pSrcPixel10 + xSrcStride;

							float yInterpolated1 = (float)(*pSrcPixel00) + (*pSrcPixel10 - *pSrcPixel00) * dy;
							float yInterpolated2 = (float)(*pSrcPixel01) + (*pSrcPixel11 - *pSrcPixel01) * dy;
							float interpolated = yInterpolated1 + (yInterpolated2 - yInterpolated1) * dx;

							//2-i because the destination pixel data goes BGRA and the source goes RGB
							pRowDstPixelData[2 - i] = (byte)interpolated;

							pSrcPixel00 += srcNextChannelOffset;
						}
					}

					pRowDstPixelData++;
				}

				pDstPixelData += yDstIncrement;
			}
		}
	}
}
