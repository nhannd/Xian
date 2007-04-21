#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal unsafe class ImageRenderer
	{
		public static void Render(
			ImageGraphic imageGraphic,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			RectangleF clientRectangle)
		{
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
				return;

			RectangleF srcViewableRectangle;
			RectangleF dstViewableRectangle;

			ImageRenderer.CalculateVisibleRectangles(imageGraphic, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

			byte[] srcPixelData = imageGraphic.PixelData.Raw;

			int[] lutData = null;
			int srcBytesPerPixel = imageGraphic.BitsAllocated / 8;

			IndexedImageGraphic grayscaleImage = imageGraphic as IndexedImageGraphic;

			if (grayscaleImage != null)
				lutData = grayscaleImage.OutputLUT;

			bool swapXY = ImageRenderer.IsRotated(imageGraphic);

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				fixed (int* pLutData = lutData)
				{
					if (imageGraphic.InterpolationMode == 
						ClearCanvas.ImageViewer.Graphics.InterpolationMode.Bilinear)
					{
						ImageInterpolatorBilinear.Interpolate(
							srcViewableRectangle,
							pSrcPixelData,
							imageGraphic.Columns,
							imageGraphic.Rows,
							srcBytesPerPixel,
							imageGraphic.BitsStored,
							dstViewableRectangle,
							(byte*)pDstPixelData,
							dstWidth,
							dstBytesPerPixel,
							swapXY,
							pLutData,
							imageGraphic.IsColor,
							imageGraphic.IsPlanar,
							imageGraphic.IsSigned);
					}
				}
			}
		}

		private static bool IsRotated(ImageGraphic imageGraphic)
		{
			float m12 = imageGraphic.SpatialTransform.CumulativeTransform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		private static void CalculateVisibleRectangles(
			ImageGraphic imageGraphic,
			RectangleF clientRectangle,
			out RectangleF dstVisibleRectangle,
			out RectangleF srcVisibleRectangle)
		{
			Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);
			RectangleF dstRectangle = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);

			// Find the intersection between the drawable client rectangle and
			// the transformed destination rectangle
			dstVisibleRectangle = RectangleUtilities.Intersect(clientRectangle, dstRectangle);

			if (dstVisibleRectangle.IsEmpty)
			{
				dstVisibleRectangle = RectangleF.Empty;
				srcVisibleRectangle = RectangleF.Empty;
				return;
			}

			// From that intersection, figure out what portion of the image
			// is Visible in source coordinates
			srcVisibleRectangle = imageGraphic.SpatialTransform.ConvertToSource(dstVisibleRectangle);

			// Converting back and forth between source and destination can result in 
			// floating point errors, so make sure that the visible rectangles are within
			// the allowed bounds of the source and destination rectangles.
			dstVisibleRectangle = RectangleUtilities.Intersect(dstVisibleRectangle, clientRectangle);
			srcVisibleRectangle = RectangleUtilities.Intersect(srcVisibleRectangle, srcRectangle);
		}
	}
}