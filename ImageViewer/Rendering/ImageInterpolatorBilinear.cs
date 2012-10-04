#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		[DllImport("BilinearInterpolation.dll", EntryPoint = "InterpolateBilinear", CallingConvention = CallingConvention.Cdecl)]
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

