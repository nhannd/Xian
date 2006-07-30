using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Renderer.GDI
{
	public unsafe class ImageRenderer
	{
		public static void Render(
			ImageLayer imageLayer, 
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			RectangleF clientRectangle)
		{
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
				return;

			Rectangle dstViewableRectangle, srcViewableRectangle;
			CalculateVisibleRectangles(imageLayer, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

			if (srcViewableRectangle.IsEmpty)
				return;

			byte[] srcPixelData = imageLayer.GetPixelData();

			byte[] lutData = null;
			int srcBytesPerPixel = imageLayer.BitsAllocated / 8;

			if (imageLayer.IsGrayscale)
				lutData = imageLayer.GrayscaleLUTPipeline.OutputLUT;

			bool swapXY = IsRotated(imageLayer);

			//CodeClock clock = new CodeClock();
			//clock.Start();

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				fixed (byte* pLutData = lutData)
				{
					StretchNearestNeighbour(
						srcViewableRectangle,
						pSrcPixelData,
						imageLayer.Columns,
						imageLayer.Rows,
						srcBytesPerPixel,
						dstViewableRectangle,
						(byte*)pDstPixelData,
						dstWidth,
						dstBytesPerPixel,
						swapXY,
						pLutData,
						imageLayer.IsColor,
						imageLayer.IsPlanar);
				}
			}

			//clock.Stop();
			//string str = String.Format("Strech: {0}", clock.ToString());
			//Platform.Log(str);
		}

		private static bool IsRotated(ImageLayer imageLayer)
		{
			float m12 = imageLayer.SpatialTransform.Transform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		private static void CalculateVisibleRectangles(
			ImageLayer imageLayer, 
			RectangleF clientRectangle, 
			out Rectangle dstVisibleRectangle, 
			out Rectangle srcVisibleRectangle)
		{
			Rectangle srcRectangleF = imageLayer.SpatialTransform.SourceRectangle;
			RectangleF dstRectangleF = imageLayer.SpatialTransform.ConvertToDestination(srcRectangleF);

			// Find the intersection between the drawable client rectangle and
			// the transformed destination rectangle
			RectangleF dstVisibleRectangleF = RectangleUtilities.Intersect(clientRectangle, dstRectangleF);

			if (dstVisibleRectangleF.IsEmpty)
			{
				dstVisibleRectangle = Rectangle.Empty;
				srcVisibleRectangle = Rectangle.Empty;
				return;
			}

			// From that intersection, figure out what portion of the image
			// is Visible in source coordinates
			RectangleF srcVisibleRectangleF = imageLayer.SpatialTransform.ConvertToSource(dstVisibleRectangleF);

			dstVisibleRectangle = Rectangle.Round(dstVisibleRectangleF);
			srcVisibleRectangle = Rectangle.Round(srcVisibleRectangleF);

			dstVisibleRectangle = RectangleUtilities.MakeRectangleZeroBased(dstVisibleRectangle);
			srcVisibleRectangle = RectangleUtilities.MakeRectangleZeroBased(srcVisibleRectangle);
		}

		private static void StretchNearestNeighbour(
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
			bool isPlanar)
		{
			int srcRegionHeight = Math.Abs(srcRegionRect.Bottom - srcRegionRect.Top) + 1;
			int srcRegionWidth = Math.Abs(srcRegionRect.Right - srcRegionRect.Left) + 1;
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
			pSrcPixelData += (srcRegionRect.Top * ySrcStride) + (srcRegionRect.Left * xSrcStride);

			int dstRegionHeight, dstRegionWidth,
				xDstStride, yDstStride,
				xDstIncrement, yDstIncrement;

			if (swapXY)
			{
				dstRegionHeight = Math.Abs(dstRegionRect.Right - dstRegionRect.Left) + 1;
				dstRegionWidth = Math.Abs(dstRegionRect.Bottom - dstRegionRect.Top) + 1;
				xDstStride = dstWidth * dstBytesPerPixel;
				yDstStride = dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRect.Bottom - dstRegionRect.Top) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRect.Right - dstRegionRect.Left) * yDstStride;
				pDstPixelData += (dstRegionRect.Top * xDstStride) + (dstRegionRect.Left * yDstStride);
			}
			else
			{
				dstRegionHeight = Math.Abs(dstRegionRect.Bottom - dstRegionRect.Top) + 1;
				dstRegionWidth = Math.Abs(dstRegionRect.Right - dstRegionRect.Left) + 1;
				xDstStride = dstBytesPerPixel;
				yDstStride = dstWidth * dstBytesPerPixel;
				xDstIncrement = Math.Sign(dstRegionRect.Right - dstRegionRect.Left) * xDstStride;
				yDstIncrement = Math.Sign(dstRegionRect.Bottom - dstRegionRect.Top) * yDstStride;
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
					StretchRowNearestNeighbourRGB(
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
						StretchRowNearestNeighbour16(
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
						StretchRowNearestNeighbour8(
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

		private static void StretchRowNearestNeighbour8(
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

		private static void StretchRowNearestNeighbour16(
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

		private static void StretchRowNearestNeighbourRGB(
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

//string str = String.Format("dstRectangle: {0}", dstRectangleF);
//Platform.Log(str);
//str = String.Format("dstViewableRectangle: {0}", dstViewableRectangle);
//Platform.Log(str);
//str = String.Format("srcViewableRectangle: {0}", srcViewableRectangle);
//Platform.Log(str);

//Matrix matrix = imageLayer.SpatialTransform.Transform;
//str = String.Format("matrix: {0},{1},{2},{3}", matrix.Elements[0], matrix.Elements[1], matrix.Elements[2], matrix.Elements[3]);
//Platform.Log(str);

			//    srcBytesPerPixel = imageLayer.BitsAllocated / 8;
			//}
			//else
			//{
			//    if (imageLayer.IsPlanar)
			//    {
			//        srcBytesPerPixel = imageLayer.BitsAllocated;
			//    }
			//    else
			//    {
			//        int samplesPerPixel = 3;
			//        srcStride = imageLayer.Columns * imageLayer.BitsAllocated / 8 * samplesPerPixel;
			//        srcBytesPerPixel = imageLayer.BitsAllocated * samplesPerPixel;
			//    }
			//}

			//int planeSize = srcStride * imageLayer.Rows;
