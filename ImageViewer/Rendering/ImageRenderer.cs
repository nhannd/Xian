#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;
using System.Runtime.InteropServices;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal unsafe class ImageRenderer
	{
		public static int[] ConstructFinalLut(IComposedLut outputLut, IColorMap colorMap, bool invert)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			colorMap.MinInputValue = outputLut.MinOutputValue;
			colorMap.MaxInputValue = outputLut.MaxOutputValue;

			int[] outputLutData = outputLut.Data;
			int[] colorMapData = colorMap.Data;
			
			int[] finalLutData = new int[outputLutData.Length];

			int numberOfEntries = finalLutData.Length;

			fixed (int* pOutputLutData = outputLutData)
			{
				fixed (int* pColorMapData = colorMapData)
				{
					fixed (int* pFinalLutData = finalLutData)
					{
						int* pFinalLut = pFinalLutData;

						if (!invert)
						{
							int firstColorMappedPixelValue = colorMap.MinInputValue;
							for (int i = 0; i < numberOfEntries; ++i)
								*(pFinalLut++) = pColorMapData[*(pOutputLutData + i) - firstColorMappedPixelValue];
						}
						else
						{
							int lastColorMappedPixelValue = colorMap.MaxInputValue;
							for (int i = 0; i < numberOfEntries; ++i)
								*(pFinalLut++) = pColorMapData[lastColorMappedPixelValue - *(pOutputLutData + i)];
						}
					}
				}
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("ImageRenderer.ConstructFinalLut", clock.Seconds);

			return finalLutData;
		}

		public static void Render(
			ImageGraphic imageGraphic,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			RectangleF clientRectangle)
		{
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			RectangleF srcViewableRectangle;
			RectangleF dstViewableRectangle;

			ImageRenderer.CalculateVisibleRectangles(imageGraphic, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

			byte[] srcPixelData = imageGraphic.PixelData.Raw;

			IndexedImageGraphic grayscaleImage = imageGraphic as IndexedImageGraphic;
			ColorImageGraphic colorImage = imageGraphic as ColorImageGraphic;

			bool swapXY = ImageRenderer.IsRotated(imageGraphic);

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				if (imageGraphic.InterpolationMode == InterpolationMode.Bilinear)
				{
					if (grayscaleImage != null)
					{
						int[] finalLutData = ConstructFinalLut(grayscaleImage.OutputLut, grayscaleImage.ColorMap, grayscaleImage.Invert);

						fixed (int* pFinalLutData = finalLutData)
						{
							ImageInterpolatorBilinear.LUTDATA lutData = new ImageInterpolatorBilinear.LUTDATA();
							lutData.LutData = pFinalLutData;
							lutData.FirstMappedPixelData = grayscaleImage.OutputLut.MinInputValue;
							lutData.Length = finalLutData.Length;

							int srcBytesPerPixel = imageGraphic.BitsPerPixel/8;

							ImageInterpolatorBilinear.Interpolate(
								srcViewableRectangle,
								pSrcPixelData,
								grayscaleImage.Columns,
								grayscaleImage.Rows,
								srcBytesPerPixel,
								grayscaleImage.BitsStored,
								dstViewableRectangle,
								(byte*) pDstPixelData,
								dstWidth,
								dstBytesPerPixel,
								swapXY,
								&lutData, //ok because it's a local variable in an unsafe method, therefore it's already fixed.
								false,
								false,
								grayscaleImage.IsSigned);
						}
					}

					if (colorImage != null)
					{
						int srcBytesPerPixel = 4;

						ImageInterpolatorBilinear.Interpolate(
							srcViewableRectangle,
							pSrcPixelData,
							colorImage.Columns,
							colorImage.Rows,
							srcBytesPerPixel,
							32,
							dstViewableRectangle,
							(byte*) pDstPixelData,
							dstWidth,
							dstBytesPerPixel,
							swapXY,
							null,
							true,
							false,
							false);
					}
				}
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("ImageRenderer.Render", clock.Seconds);
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