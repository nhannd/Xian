using System;
using System.Runtime.InteropServices;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal unsafe class ImageInterpolatorNearestNeighbour : ImageInterpolator
    {
        public override unsafe void Interpolate(
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
            bool isSigned)
		{
			
			int srcRegionHeight = Math.Abs(srcRegionRect.Bottom - srcRegionRect.Top);
			int srcRegionWidth = Math.Abs(srcRegionRect.Right - srcRegionRect.Left);
			int xSrcStride, ySrcStride;

			// If the image is RGB triplet, then the x-stride is 3 bytes
			if (isRGB && !isPlanar)
			{
				xSrcStride = 3;
				ySrcStride = srcWidth * 3;
			}
			// Otherwise, it's just the number of bytes per pixel
			else
			{
				xSrcStride = srcBytesPerPixel;
				ySrcStride = srcWidth * srcBytesPerPixel;
			}

			int xSrcIncrement = Math.Sign(srcRegionRect.Right - srcRegionRect.Left) * xSrcStride;
			int ySrcIncrement = Math.Sign(srcRegionRect.Bottom - srcRegionRect.Top) * ySrcStride;

			srcRegionRect = RectangleUtilities.MakeRectangleZeroBased(srcRegionRect);
			pSrcPixelData += (srcRegionRect.Top * ySrcStride) + (srcRegionRect.Left * xSrcStride);

			int dstRegionHeight, dstRegionWidth,
				xDstStride, yDstStride,
				xDstIncrement, yDstIncrement;

			if (swapXY)
			{
				dstRegionHeight = Math.Abs(dstRegionRect.Right - dstRegionRect.Left);
				dstRegionWidth = Math.Abs(dstRegionRect.Bottom - dstRegionRect.Top);
				xDstStride = dstWidth * dstBytesPerPixel;
				yDstStride = dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRect.Bottom - dstRegionRect.Top) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRect.Right - dstRegionRect.Left) * yDstStride;
				
				dstRegionRect = RectangleUtilities.MakeRectangleZeroBased(dstRegionRect);
				pDstPixelData += (dstRegionRect.Top * xDstStride) + (dstRegionRect.Left * yDstStride);
			}
			else
			{
				dstRegionHeight = Math.Abs(dstRegionRect.Bottom - dstRegionRect.Top);
				dstRegionWidth = Math.Abs(dstRegionRect.Right - dstRegionRect.Left);
				xDstStride = dstBytesPerPixel;
				yDstStride = dstWidth * dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRect.Right - dstRegionRect.Left) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRect.Bottom - dstRegionRect.Top) * yDstStride;
				
				dstRegionRect = RectangleUtilities.MakeRectangleZeroBased(dstRegionRect);
				pDstPixelData += (dstRegionRect.Top * yDstStride) + (dstRegionRect.Left * xDstStride);
			}

			int ey = srcRegionHeight - dstRegionHeight;

			int greenOffset, blueOffset;

			if (isPlanar)
			{
				greenOffset = srcWidth * srcHeight;
				blueOffset = greenOffset * 2;
			}
			else
			{
				greenOffset = 1;
				blueOffset = 2;
			}

            for (int y = 0; y < dstRegionHeight; y++)
			{
				if (isRGB)
				{
                    StretchRowRGB(
                         pSrcPixelData,
                         pDstPixelData,
                         pLutData,
                         srcRegionWidth,
                         dstRegionWidth,
                         xSrcIncrement,
                         xDstIncrement,
                         greenOffset,
                         blueOffset);
				}
				else
				{
					if (srcBytesPerPixel == 2)
					{
                        StretchRow16(
                            pSrcPixelData,
                            pDstPixelData,
                            pLutData,
                            srcRegionWidth,
                            dstRegionWidth,
                            xSrcIncrement,
                            xDstIncrement);
					}
					else if (srcBytesPerPixel == 1)
					{
						StretchRow8(
							pSrcPixelData, 
							pDstPixelData, 
							pLutData, 
							srcRegionWidth, 
							dstRegionWidth, 
							xSrcIncrement, 
							xDstIncrement);
					}
				}

				while (ey >= 0)
				{
					pSrcPixelData += ySrcIncrement;
					ey -= dstRegionHeight;
				}

				pDstPixelData += yDstIncrement;
				ey += srcRegionHeight;
			}
		}

		private static void StretchRow8(
			byte* pSrcPixelData,
			byte* pDstPixelData,
			byte* pLutData,
			int srcRegionWidth,
			int dstRegionWidth,
			int xSrcIncrement,
			int xDstIncrement)
		{
			byte* pRowSrcPixelData = pSrcPixelData;
			byte* pRowDstPixelData = pDstPixelData;

			int ex = srcRegionWidth - dstRegionWidth;

			for (int x = 0; x < dstRegionWidth; x++)
			{
				byte value = pLutData[*pRowSrcPixelData];

				pRowDstPixelData[0] = value; //B
				pRowDstPixelData[1] = value; //G
				pRowDstPixelData[2] = value; //R
				pRowDstPixelData[3] = 0xff;  //A

				while (ex >= 0)
				{
					pRowSrcPixelData += xSrcIncrement;
					ex -= dstRegionWidth;
				}

				pRowDstPixelData += xDstIncrement;
				ex += srcRegionWidth;
			}
		}

		private static void StretchRow16(
			byte* pSrcPixelData, 
			byte* pDstPixelData, 
			byte* pLutData, 
			int srcRegionWidth,
			int dstRegionWidth, 
			int xSrcIncrement, 
			int xDstIncrement)
		{
			byte* pRowSrcPixelData = pSrcPixelData;
			byte* pRowDstPixelData = pDstPixelData;

			int ex = srcRegionWidth - dstRegionWidth;

			for (int x = 0; x < dstRegionWidth; x++)
			{
                byte value = pLutData[*((ushort*)pRowSrcPixelData)];
                pRowDstPixelData[0] = value; //B
                pRowDstPixelData[1] = value; //G
                pRowDstPixelData[2] = value; //R
                pRowDstPixelData[3] = 0xff;  //A

				while (ex >= 0)
				{
					pRowSrcPixelData += xSrcIncrement;
					ex -= dstRegionWidth;
				}

				pRowDstPixelData += xDstIncrement;
				ex += srcRegionWidth;
			}
		}

		private static void StretchRowRGB(
			byte* pSrcPixelData, 
			byte* pDstPixelData, 
			byte* pLutData, 
			int srcRegionWidth, 
			int dstRegionWidth, 
			int xSrcIncrement, 
			int xDstIncrement,
			int greenOffset, 
			int blueOffset)
		{
			byte* pRowSrcPixelData = pSrcPixelData;
			byte* pRowDstPixelData = pDstPixelData;

			int ex = srcRegionWidth - dstRegionWidth;

			for (int x = 0; x < dstRegionWidth; x++)
			{
				pRowDstPixelData[0] = pRowSrcPixelData[blueOffset]; //B
				pRowDstPixelData[1] = pRowSrcPixelData[greenOffset]; //G
				pRowDstPixelData[2] = pRowSrcPixelData[0]; //R
				pRowDstPixelData[3] = 0xff;  //A

				while (ex >= 0)
				{
					pRowSrcPixelData += xSrcIncrement;
					ex -= dstRegionWidth;
				}

				pRowDstPixelData += xDstIncrement;
				ex += srcRegionWidth;
			}
		}
	}
}
