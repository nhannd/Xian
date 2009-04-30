#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.ImageViewer.Graphics;
using itk;

namespace ClearCanvas.ImageViewer.VtkItkAdapters
{
	/// <summary>
	/// An adapter to bridge ClearCanvas Image and ManagedITK Image.
	/// </summary>
	/// <remarks>
	/// <para>
	/// </para>
	/// <para>
	/// There is a method for every type of grayscale pixel data:
	/// 8-bit signed, 8-bit unsigned, 16-bit signed and 16-bit unsigned.  Note that
	/// colour images are not supported (though you could easily add that yourself).
	/// </para>
    /// <para>
    /// The ManagedITK 2-D scalar image supports only unsigned char (UC), signed short (SS),
    /// and unsigned long (UC), although the underlying ITK supports signed char, unsigned short
    /// and signed long as well. Thus, the 8-bit signed and unsigned map to UC, and the
    /// 16-bit signed and unsigned map to SS.
    /// </para>
    /// 
	/// </remarks>
	public static class ItkHelper
	{
		/// <summary>
		/// Create a managed ITK image from a given ClearCanvas image.
		/// </summary>
		/// <param name="image">The given grayscale image.</param>
        public static itkImageBase CreateItkImage(GrayscaleImageGraphic image)
		{
            itkPixelType pixelType;
            if (image.BitsPerPixel == 16)
            {
                if (image.IsSigned)
                {
                    pixelType = itkPixelType.SS;
                }
                else
                {
                    pixelType = itkPixelType.SS;// itkPixelType.US; there is no itkImage_US<dim>
                }
            }
            else //if (image.BitsPerPixel == 8)
            {
                if (image.IsSigned)
                {
                    pixelType = itkPixelType.UC;// itkPixelType.SC; threre is no itkImage_UC<dim>
                }
                else
                {
                    pixelType = itkPixelType.UC;
                }
            }
            itkImage itkImage = itkImage.New(pixelType, 2);
            // Create some image information
            itkSize size = new itkSize(image.Columns, image.Rows);
            itkSpacing spacing = new itkSpacing(1.0, 1.0);
            itkIndex index = new itkIndex(0, 0);
            itkPoint origin = new itkPoint(0.0, 0.0);
            itkImageRegion region = new itkImageRegion(size, index);
            // Set the information
            // Note: we must call SetRegions() *before* calling Allocate().
            itkImage.SetRegions(region);
            itkImage.Allocate();
            itkImage.Spacing = spacing;
            itkImage.Origin = origin;
            //itkImage.BufferedRegion;
            return itkImage;
		}

        public static void CopyToItkImage(GrayscaleImageGraphic image, itkImageBase itkImage)
        {
            if (image.BitsPerPixel == 16)
            {
                if (image.IsSigned)
                {
                    CopyToSigned16(image, itkImage);
                }
                else
                {
                    CopyToUnsigned16(image, itkImage);
                }
            }
            else
            {
                if (image.IsSigned)
                {
                    CopyToSigned8(image, itkImage);
                }
                else
                {
                    CopyToUnsigned8(image, itkImage);
                }
            }
        }

        private unsafe static void CopyToSigned16(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pSrcByte = image.PixelData.Raw)
            {
                itkImageRegionIterator_ISS2 inputIt = new itkImageRegionIterator_ISS2(itkImage, itkImage.LargestPossibleRegion);
                short* pSrc = (short*)pSrcByte;
                int height = image.Rows;
                int width = image.Columns;
                short pixelValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelValue = pSrc[0];
                        inputIt.Set(pixelValue);
                        pSrc++;
                        inputIt++;
                    }
                }
            }
        }

        private unsafe static void CopyToUnsigned16(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pSrcByte = image.PixelData.Raw)
            {
                // itkImageIterator has F, SS, and UC
                itkImageRegionIterator_ISS2 inputIt = new itkImageRegionIterator_ISS2(itkImage, itkImage.LargestPossibleRegion);
                ushort* pSrc = (ushort*)pSrcByte;
                int height = image.Rows;
                int width = image.Columns;
                ushort pixelValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelValue = pSrc[0];
                        inputIt.Set(pixelValue);
                        pSrc++;
                        inputIt++;
                    }
                }
            }
        }

        private unsafe static void CopyToSigned8(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pSrcByte = image.PixelData.Raw)
            {
                // itkImageIterator has F, SS, and UC
                itkImageRegionIterator_IUC2 inputIt = new itkImageRegionIterator_IUC2(itkImage, itkImage.LargestPossibleRegion);
                sbyte* pSrc = (sbyte*)pSrcByte;
                int height = image.Rows;
                int width = image.Columns;
                sbyte pixelValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelValue = pSrc[0];
                        inputIt.Set(pixelValue);
                        pSrc++;
                        inputIt++;
                    }
                }
            }
        }

        private unsafe static void CopyToUnsigned8(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pSrcByte = image.PixelData.Raw)
            {
                itkImageRegionIterator_IUC2 inputIt = new itkImageRegionIterator_IUC2(itkImage, itkImage.LargestPossibleRegion);
                byte* pSrc = (byte*)pSrcByte;
                int height = image.Rows;
                int width = image.Columns;
                byte pixelValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelValue = pSrc[0];
                        inputIt.Set(pixelValue);
                        pSrc++;
                        inputIt++;
                    }
                }
            }
        }

        public static void CopyFromItkImage(GrayscaleImageGraphic image, itkImageBase itkImage)
        {
            if (image.BitsPerPixel == 16)
            {
                if (image.IsSigned)
                {
                    CopyFromSigned16(image, itkImage);
                }
                else
                {
                    CopyFromUnsigned16(image, itkImage);
                }
            }
            else
            {
                if (image.IsSigned)
                {
                    CopyFromSigned8(image, itkImage);
                }
                else
                {
                    CopyFromUnsigned8(image, itkImage);
                }
            }
        }

        private unsafe static void CopyFromSigned16(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pDstByte = image.PixelData.Raw)
            {
                itkImageRegionConstIterator_ISS2 itkIt = new itkImageRegionConstIterator_ISS2(itkImage, itkImage.LargestPossibleRegion);
                short* pDst = (short*)pDstByte;
                int height = image.Rows;
                int width = image.Columns;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pDst[0] = itkIt.Get().ValueAsSS;
                        pDst++;
                        itkIt++;
                    }
                }
            }
        }

        private unsafe static void CopyFromUnsigned16(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pDstByte = image.PixelData.Raw)
            {
                itkImageRegionConstIterator_ISS2 itkIt = new itkImageRegionConstIterator_ISS2(itkImage, itkImage.LargestPossibleRegion);
                ushort* pDst = (ushort*)pDstByte;
                int height = image.Rows;
                int width = image.Columns;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int sl = itkIt.Get().ValueAsSL;
                        pDst[0] = (ushort) sl;// itkIt.Get().ValueAsUS;
                        pDst++;
                        itkIt++;
                    }
                }
            }
        }
        private unsafe static void CopyFromSigned8(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pDstByte = image.PixelData.Raw)
            {
                itkImageRegionConstIterator_IUC2 itkIt = new itkImageRegionConstIterator_IUC2(itkImage, itkImage.LargestPossibleRegion);
                sbyte* pDst = (sbyte*)pDstByte;
                int height = image.Rows;
                int width = image.Columns;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pDst[0] = itkIt.Get().ValueAsSC;
                        pDst++;
                        itkIt++;
                    }
                }
            }
        }

        private unsafe static void CopyFromUnsigned8(ImageGraphic image, itkImageBase itkImage)
        {
            fixed (byte* pDstByte = image.PixelData.Raw)
            {
                itkImageRegionConstIterator_IUC2 itkIt = new itkImageRegionConstIterator_IUC2(itkImage, itkImage.LargestPossibleRegion);
                byte* pDst = (byte*)pDstByte;
                int height = image.Rows;
                int width = image.Columns;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pDst[0] = itkIt.Get().ValueAsUC;
                        pDst++;
                        itkIt++;
                    }
                }
            }
        }
	}
}
