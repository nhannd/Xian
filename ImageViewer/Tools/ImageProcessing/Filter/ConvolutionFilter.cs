using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	/// <summary>
	/// A convolution filter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The convolution operation is implemented using unsafe code.
	/// This is absolutely essential if we want to have any hope of performing the 
	/// operation in a reasonable amount of time.
	/// </para>
	/// <para>
	/// There is a method for every type of grayscale pixel data:
	/// 8-bit signed, 8-bit unsigned, 16-bit signed and 16-bit unsigned.  Note that
	/// colour images are not supported (though you could easily add that yourself).
	/// </para>
	/// <para>
	/// Code adapted from: http://www.codeproject.com/cs/media/csharpfilters.asp
	/// </para>
	/// </remarks>
	public unsafe static class ConvolutionFilter
	{
		/// <summary>
		/// Applies a filter to the specified image using convolution.
		/// </summary>
		/// <param name="image">The grayscale image to be filtered.</param>
		/// <param name="kernel">A 3x3 convolution kernel.</param>
		public static void Apply(GrayscaleImageGraphic image, ConvolutionKernel kernel)
		{
			int kernelSize = 2;
			int width = image.Columns - kernelSize;
			int height = image.Rows - kernelSize;
			int stride = width;
			int stride2 = stride * 2;

			int minInputValue = image.ModalityLUT.MinInputValue;
			int maxInputValue = image.ModalityLUT.MaxInputValue;

			if (image.BitsAllocated == 16)
			{
				if (image.IsSigned)
				{
					ApplyToSigned16(
						(byte[])image.PixelData.Raw.Clone(), 
						image.PixelData.Raw, 
						kernel, 
						kernelSize, 
						width, 
						height, 
						stride, 
						stride2, 
						minInputValue, 
						maxInputValue);
				}
				else
				{
					ApplyToUnsigned16(
						(byte[])image.PixelData.Raw.Clone(),
						image.PixelData.Raw,
						kernel,
						kernelSize,
						width,
						height,
						stride,
						stride2,
						minInputValue,
						maxInputValue);
				}
			}
			else
			{
				if (image.IsSigned)
				{
					ApplyToSigned8(
						(byte[])image.PixelData.Raw.Clone(),
						image.PixelData.Raw,
						kernel,
						kernelSize,
						width,
						height,
						stride,
						stride2,
						minInputValue,
						maxInputValue);
				}
				else
				{
					ApplyToUnsigned8(
						(byte[])image.PixelData.Raw.Clone(),
						image.PixelData.Raw,
						kernel,
						kernelSize,
						width,
						height,
						stride,
						stride2,
						minInputValue,
						maxInputValue);
				}
			}
		}

		private static void ApplyToUnsigned16(
			byte[] pixelDataSrc,
			byte[] pixelDataDst, 
			ConvolutionKernel m,
			int kernelSize, 
			int width, 
			int height, 
			int stride, 
			int stride2, 
			int minInputValue, 
			int maxInputValue)
		{
			fixed (byte* pDstByte = pixelDataDst)
			{
				fixed (byte* pSrcByte = pixelDataSrc)
				{
					ushort* pSrc = (ushort*)pSrcByte;
					ushort* pDst = (ushort*)pDstByte;
					int pixelValue;

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							pixelValue = ((((pSrc[0] * m.TopLeft) + (pSrc[1] * m.TopMid) + (pSrc[2] * m.TopRight) +
								(pSrc[0 + stride] * m.MidLeft) + (pSrc[1 + stride] * m.Pixel) + (pSrc[2 + stride] * m.MidRight) +
								(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[1 + stride2] * m.BottomMid) + (pSrc[2 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

							if (pixelValue < minInputValue)
								*pDst = (ushort)minInputValue;
							else if (pixelValue > maxInputValue)
								*pDst = (ushort)maxInputValue;
							else
								*pDst = (ushort)pixelValue;

							pDst++;
							pSrc++;
						}

						pDst += kernelSize;
						pSrc += kernelSize;
					}
				}
			}
		}

		private static void ApplyToSigned16(
			byte[] pixelDataSrc,
			byte[] pixelDataDst,
			ConvolutionKernel m,
			int kernelSize,
			int width,
			int height,
			int stride,
			int stride2,
			int minInputValue,
			int maxInputValue)
		{
			fixed (byte* pDstByte = pixelDataDst)
			{
				fixed (byte* pSrcByte = pixelDataSrc)
				{
					short* pSrc = (short*)pSrcByte;
					short* pDst = (short*)pDstByte;
					int pixelValue;

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							pixelValue = ((((pSrc[0] * m.TopLeft) + (pSrc[1] * m.TopMid) + (pSrc[2] * m.TopRight) +
								(pSrc[0 + stride] * m.MidLeft) + (pSrc[1 + stride] * m.Pixel) + (pSrc[2 + stride] * m.MidRight) +
								(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[1 + stride2] * m.BottomMid) + (pSrc[2 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

							if (pixelValue < minInputValue)
								*pDst = (short)minInputValue;
							else if (pixelValue > maxInputValue)
								*pDst = (short)maxInputValue;
							else
								*pDst = (short)pixelValue;

							pDst++;
							pSrc++;
						}

						pDst += kernelSize;
						pSrc += kernelSize;
					}
				}
			}
		}

		private static void ApplyToUnsigned8(
			byte[] pixelDataSrc,
			byte[] pixelDataDst,
			ConvolutionKernel m,
			int kernelSize,
			int width,
			int height,
			int stride,
			int stride2,
			int minInputValue,
			int maxInputValue)
		{
			fixed (byte* pDstByte = pixelDataDst)
			{
				fixed (byte* pSrcByte = pixelDataSrc)
				{
					byte* pSrc = (byte*)pSrcByte;
					byte* pDst = (byte*)pDstByte;
					int pixelValue;

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							pixelValue = ((((pSrc[0] * m.TopLeft) + (pSrc[1] * m.TopMid) + (pSrc[2] * m.TopRight) +
								(pSrc[0 + stride] * m.MidLeft) + (pSrc[1 + stride] * m.Pixel) + (pSrc[2 + stride] * m.MidRight) +
								(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[1 + stride2] * m.BottomMid) + (pSrc[2 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

							if (pixelValue < minInputValue)
								*pDst = (byte)minInputValue;
							else if (pixelValue > maxInputValue)
								*pDst = (byte)maxInputValue;
							else
								*pDst = (byte)pixelValue;

							pDst++;
							pSrc++;
						}

						pDst += kernelSize;
						pSrc += kernelSize;
					}
				}
			}
		}

		private static void ApplyToSigned8(
			byte[] pixelDataSrc,
			byte[] pixelDataDst,
			ConvolutionKernel m,
			int kernelSize,
			int width,
			int height,
			int stride,
			int stride2,
			int minInputValue,
			int maxInputValue)
		{
			fixed (byte* pDstByte = pixelDataDst)
			{
				fixed (byte* pSrcByte = pixelDataSrc)
				{
					sbyte* pSrc = (sbyte*)pSrcByte;
					sbyte* pDst = (sbyte*)pDstByte;
					int pixelValue;

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							pixelValue = ((((pSrc[0] * m.TopLeft) + (pSrc[1] * m.TopMid) + (pSrc[2] * m.TopRight) +
								(pSrc[0 + stride] * m.MidLeft) + (pSrc[1 + stride] * m.Pixel) + (pSrc[2 + stride] * m.MidRight) +
								(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[1 + stride2] * m.BottomMid) + (pSrc[2 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

							if (pixelValue < minInputValue)
								*pDst = (sbyte)minInputValue;
							else if (pixelValue > maxInputValue)
								*pDst = (sbyte)maxInputValue;
							else
								*pDst = (sbyte)pixelValue;

							pDst++;
							pSrc++;
						}

						pDst += kernelSize;
						pSrc += kernelSize;
					}
				}
			}
		}
	}
}
