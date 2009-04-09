#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal unsafe class ImageRenderer
	{
		[ThreadStatic] private static int[] _finalLutBuffer;

		public static void Render(
			ImageGraphic imageGraphic,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			Rectangle clientRectangle)
		{
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
				return;

			if (imageGraphic.SizeInBytes != imageGraphic.PixelData.Raw.Length)
				throw new InvalidOperationException(String.Format(SR.ExceptionIncorrectPixelDataSize, imageGraphic.SizeInBytes, imageGraphic.PixelData.Raw.Length));

			CodeClock clock = new CodeClock();
			clock.Start();

			RectangleF srcViewableRectangle;
			Rectangle dstViewableRectangle;

			CalculateVisibleRectangles(imageGraphic, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

			if (imageGraphic is GrayscaleImageGraphic)
			{
				RenderGrayscale(
					imageGraphic as GrayscaleImageGraphic,
					srcViewableRectangle,
					dstViewableRectangle,
					pDstPixelData,
					dstWidth,
					dstBytesPerPixel);
			}
			else if (imageGraphic is ColorImageGraphic)
			{
				RenderColor(
					imageGraphic as ColorImageGraphic,
					srcViewableRectangle,
					dstViewableRectangle,
					pDstPixelData,
					dstWidth,
					dstBytesPerPixel);
			}
			else
			{
				throw new Exception("Unknown ImageGraphic.");
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("ImageRenderer", "Render", clock.Seconds);
		}

		private static void RenderGrayscale(
			GrayscaleImageGraphic image, 
			RectangleF srcViewableRectangle, 
			Rectangle dstViewableRectangle,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel)
		{
			fixed (byte* pSrcPixelData = image.PixelData.Raw)
			{
				if (image.InterpolationMode == InterpolationMode.Bilinear)
				{
					int[] finalLutBuffer = ConstructFinalLut(image.OutputLut, image.ColorMap, image.Invert);

					fixed (int* pFinalLutData = finalLutBuffer)
					{
						ImageInterpolatorBilinear.LutData lutData;
						lutData.Data = pFinalLutData;
						lutData.FirstMappedPixelData = image.OutputLut.MinInputValue;
						lutData.Length = finalLutBuffer.Length;

						ImageInterpolatorBilinear.Interpolate(
							srcViewableRectangle,
							pSrcPixelData,
							image.Columns,
							image.Rows,
							image.BytesPerPixel,
							image.BitsStored,
							dstViewableRectangle,
							(byte*) pDstPixelData,
							dstWidth,
							dstBytesPerPixel,
							IsRotated(image),
							&lutData, //ok because it's a local variable in an unsafe method, therefore it's already fixed.
							false,
							false,
							image.IsSigned);
					}
				}
			}
		}

		private static void RenderColor(
			ColorImageGraphic image,
			RectangleF srcViewableRectangle,
			Rectangle dstViewableRectangle,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel)
		{
			fixed (byte* pSrcPixelData = image.PixelData.Raw)
			{
				if (image.InterpolationMode == InterpolationMode.Bilinear)
				{
					int srcBytesPerPixel = 4;

					ImageInterpolatorBilinear.Interpolate(
						srcViewableRectangle,
						pSrcPixelData,
						image.Columns,
						image.Rows,
						srcBytesPerPixel,
						32,
						dstViewableRectangle,
						(byte*)pDstPixelData,
						dstWidth,
						dstBytesPerPixel,
						IsRotated(image),
						null,
						true,
						false,
						false);
				}
			}
		}

		private static bool IsRotated(ImageGraphic imageGraphic)
		{
			float m12 = imageGraphic.SpatialTransform.CumulativeTransform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		//internal for unit testing only.
		internal static void CalculateVisibleRectangles(
			ImageGraphic imageGraphic,
			Rectangle clientRectangle,
			out Rectangle dstVisibleRectangle,
			out RectangleF srcVisibleRectangle)
		{
			Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);
			RectangleF dstRectangle = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);

			// Find the intersection between the drawable client rectangle and
			// the transformed destination rectangle
			dstRectangle = RectangleUtilities.RoundInflate(dstRectangle);
			dstRectangle = RectangleUtilities.Intersect(clientRectangle, dstRectangle);

			if (dstRectangle.IsEmpty)
			{
				dstVisibleRectangle = Rectangle.Empty;
				srcVisibleRectangle = RectangleF.Empty;
				return;
			}

			// Figure out what portion of the image is visible in source coordinates
			srcVisibleRectangle = imageGraphic.SpatialTransform.ConvertToSource(dstRectangle);
			//dstRectangle is already rounded, this is just a conversion to Rectangle.
			dstVisibleRectangle = Rectangle.Round(dstRectangle);
		}

		private static int[] ConstructFinalLut(IComposedLut outputLut, IDataLut colorMap, bool invert)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			colorMap.MinInputValue = outputLut.MinOutputValue;
			colorMap.MaxInputValue = outputLut.MaxOutputValue;

			int[] outputLutData = outputLut.Data;
			int[] colorMapData = colorMap.Data;

			if (_finalLutBuffer == null || _finalLutBuffer.Length != outputLutData.Length)
				_finalLutBuffer = new int[outputLutData.Length];

			int numberOfEntries = _finalLutBuffer.Length;

			fixed (int* pOutputLutData = outputLutData)
			{
				fixed (int* pColorMapData = colorMapData)
				{
					fixed (int* pFinalLutData = _finalLutBuffer)
					{
						int* pFinalLut = pFinalLutData;

						if (!invert)
						{
							int firstColorMappedPixelValue = colorMap.FirstMappedPixelValue;
							for (int i = 0; i < numberOfEntries; ++i)
								*(pFinalLut++) = pColorMapData[*(pOutputLutData + i) - firstColorMappedPixelValue];
						}
						else
						{
							int lastColorMappedPixelValue = colorMap.FirstMappedPixelValue + colorMap.Data.Length - 1;
							for (int i = 0; i < numberOfEntries; ++i)
								*(pFinalLut++) = pColorMapData[lastColorMappedPixelValue - *(pOutputLutData + i)];
						}
					}
				}
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("ImageRenderer", "ConstructFinalLut", clock.Seconds);

			return _finalLutBuffer;
		}
	}
}