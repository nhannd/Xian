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

using System.Drawing;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal unsafe class ImageInterpolatorBilinear
    {
		[StructLayout(LayoutKind.Sequential)]
		public struct LutData
		{
			public int* Data;
			public int FirstMappedPixelData;
			public int Length;
		}

        public static unsafe void Interpolate(
            RectangleF srcRegionRectangle,
            byte* pSrcPixelData,
            int srcWidth,
            int srcHeight,
            int srcBytesPerPixel,
			int srcBitsStored,
            Rectangle dstRegionRectangle,
            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            bool swapXY,
            LutData* lutData,
            bool isRGB,
            bool isPlanar,
            bool isSigned)
        {
			InterpolateBilinear(
				pSrcPixelData, 
				srcWidth, 
				srcHeight, 
				srcBytesPerPixel,
				srcBitsStored,
				isSigned, 
				isRGB, 
				isPlanar,
				srcRegionRectangle.Left, 
				srcRegionRectangle.Top, 
				srcRegionRectangle.Right, 
				srcRegionRectangle.Bottom,
				pDstPixelData, 
				dstWidth, 
				dstBytesPerPixel,
				dstRegionRectangle.Left, 
				dstRegionRectangle.Top, 
				dstRegionRectangle.Right, 
				dstRegionRectangle.Bottom,
				swapXY, 
				lutData);
		}

		/// <summary>
		/// Import the C++ DLL that implements the fixed point bilinear interpolation method.
		/// </summary>
		[DllImport("BilinearInterpolation.dll", EntryPoint = "InterpolateBilinear")]
		private static extern int InterpolateBilinear
		(
			byte* pSrcPixelData,

			int srcWidth,
			int srcHeight,
			int srcBytesPerPixel,
			int srcBitsStored,

			bool isSigned,
			bool isRGB,
			bool isPlanar,

			float srcRegionRectLeft,
			float srcRegionRectTop,
			float srcRegionRectRight,
			float srcRegionRectBottom,

			byte* pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,

			int dstRegionRectLeft,
			int dstRegionRectTop,
			int dstRegionRectRight,
			int dstRegionRectBottom,

			bool swapXY,
			LutData* lutData
		);
    }
}

