using System.Drawing;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal unsafe class ImageInterpolatorBilinear
    {
		[StructLayout(LayoutKind.Sequential)]
		public struct LUTDATA
		{
			public int* LutData;
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
            RectangleF dstRegionRectangle,
            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            bool swapXY,
            LUTDATA* lutData,
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
		[DllImport("ClearCanvas.ImageViewer.Rendering.BilinearInterpolation.dll", EntryPoint = "InterpolateBilinear")]
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

			float dstRegionRectLeft,
			float dstRegionRectTop,
			float dstRegionRectRight,
			float dstRegionRectBottom,

			bool swapXY,
			LUTDATA* lutData
		);
    }
}

