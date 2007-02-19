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
	public unsafe class ImageRenderer
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

			Rectangle srcViewableRectangle;
			Rectangle dstViewableRectangle;

			ImageRenderer.CalculateVisibleRectangles(imageGraphic, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

			if (srcViewableRectangle.IsEmpty)
				return;

			byte[] srcPixelData = imageGraphic.PixelData.Raw;

			int[] lutData = null;
			int srcBytesPerPixel = imageGraphic.BitsAllocated / 8;

			IndexedImageGraphic grayscaleImage = imageGraphic as IndexedImageGraphic;

			if (grayscaleImage != null)
			{
				grayscaleImage.LUTComposer.Compose();
				lutData = grayscaleImage.LUTComposer.OutputLUT;
			}

			bool swapXY = ImageRenderer.IsRotated(imageGraphic);

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				fixed (int* pLutData = lutData)
				{
					//Use this loop code to quickly determine how well a particular interpolator will perform.

					//CodeClock clock = new CodeClock();
					//clock.Start();

					//int numimages = 1;
					//if (_showMessageBox)
					//    numimages = 50;

					//for (int i = 0; i < numimages; ++i)
					//{
					ImageInterpolator.AllocateInterpolator(imageGraphic.InterpolationMethod).Interpolate(
						srcViewableRectangle,
						pSrcPixelData,
						imageGraphic.Columns,
						imageGraphic.Rows,
						srcBytesPerPixel,
						dstViewableRectangle,
						(byte*)pDstPixelData,
						dstWidth,
						dstBytesPerPixel,
						swapXY,
						pLutData,
						imageGraphic.IsColor,
						imageGraphic.IsPlanar,
						imageGraphic.IsSigned);
					//}

					//clock.Stop();

					//string str = String.Format("50x Draw: {0}\n", clock.ToString());
					//Trace.Write(str);
					//if (_showMessageBox)
					//    Platform.ShowMessageBox(str);

					//_showMessageBox = false;
				}
			}
		}

		protected static bool IsRotated(ImageGraphic imageGraphic)
		{
			float m12 = imageGraphic.SpatialTransform.CumulativeTransform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		protected static void CalculateVisibleRectangles(
			ImageGraphic imageGraphic,
			RectangleF clientRectangle,
			out Rectangle dstVisibleRectangle,
			out Rectangle srcVisibleRectangle)
		{
			Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);
			RectangleF dstRectangleF = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);

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
			RectangleF srcVisibleRectangleF = imageGraphic.SpatialTransform.ConvertToSource(dstVisibleRectangleF);

			// Round the rectangles, but round away from the centre of the rectangle,
			// rather than simply truncating the LTRB values.
			srcVisibleRectangleF = RectangleUtilities.RoundInflate(srcVisibleRectangleF);
			dstVisibleRectangleF = RectangleUtilities.RoundInflate(dstVisibleRectangleF);

			// Redo the intersection(s) after rounding, otherwise if you render to a bitmap
			// that is exactly the same size as the clientRectangle, the dstVisibleRectangle
			// will occasionally be outside the clientRectangle and cause a crash.
			dstVisibleRectangleF = RectangleUtilities.Intersect(dstVisibleRectangleF, clientRectangle);
			srcVisibleRectangleF = RectangleUtilities.Intersect(srcVisibleRectangleF, srcRectangle);

			// Just a formality now, these are already rounded.
			srcVisibleRectangle = Rectangle.Round(srcVisibleRectangleF);
			dstVisibleRectangle = Rectangle.Round(dstVisibleRectangleF);
		}
	}
}