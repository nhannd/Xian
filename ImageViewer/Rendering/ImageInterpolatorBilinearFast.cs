#pragma warning disable 1591,0419,1574,1587

using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal unsafe class ImageInterpolatorBilinearFast : ImageInterpolator
    {
        public override unsafe void Interpolate(
            Rectangle srcRegionRectangle,
            byte* pSrcPixelData,
            int srcWidth,
            int srcHeight,
            int srcBytesPerPixel,
            Rectangle dstRegionRectangle,
            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            bool swapXY,
            int* pLutData,
            bool isRGB,
            bool isPlanar,
            bool isSigned)
        {
            //use the 'fast' bilinear interpolation method.
            InterpolateBilinearExternal(srcRegionRectangle, pSrcPixelData, srcWidth, srcHeight, srcBytesPerPixel,
                                    dstRegionRectangle, pDstPixelData, dstWidth, dstBytesPerPixel, swapXY, pLutData,
                                    isRGB, isPlanar, isSigned);
        }

        /// <summary>
        /// Import the C++ DLL that implements the fast bilinear interpolation method.
        /// </summary>
        [DllImport("ClearCanvas.ImageViewer.Rendering.BilinearInterpolation.dll", EntryPoint = "InterpolateBilinear")]
        private static extern int InterpolateBilinearEx
        (
            byte* pSrcPixelData,

            int srcWidth,
            int srcHeight,
            int srcBytesPerPixel,

            bool isSigned,
            bool isRGB,
            bool isPlanar,

            int srcRegionRectLeft,
            int srcRegionRectTop,
            int srcRegionRectRight,
            int srcRegionRectBottom,

            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,

            int dstRegionRectLeft,
            int dstRegionRectTop,
            int dstRegionRectRight,
            int dstRegionRectBottom,

            bool swapXY,
            int* pLutData
        );

        /// <summary>
        /// Implements the Fast Bilinear Interpolation algorithm (Fixed Point Arithmetic).
        /// </summary>
        /// <remarks>
        /// This code has been tested in both C# and C++, but proved to be substantially faster in C++.
        /// (1.5 seconds/ 50 images in C# vs. 1.2 seconds /50 images in C++).  So, it has been implemented in C++
        /// and the C# version has been removed.  If it needed to be ported to C# again, it only takes a short time
        /// to convert the code.
        /// </remarks>
        private static unsafe void InterpolateBilinearExternal
            (
                Rectangle srcRegionRectangle,
                byte* pSrcPixelData,
                int srcWidth,
                int srcHeight,
                int srcBytesPerPixel,
                Rectangle dstRegionRectangle,
                byte* pDstPixelData,
                int dstWidth,
                int dstBytesPerPixel,
                bool swapXY,
                int* pLutData,
                bool isRGB,
                bool isPlanar,
                bool isSigned
            )
        {
            InterpolateBilinearEx(
                    (byte*)pSrcPixelData, (int)srcWidth, (int)srcHeight, (int)srcBytesPerPixel,
                    isSigned, isRGB, isPlanar,
                    (int)srcRegionRectangle.Left, (int)srcRegionRectangle.Top, (int)srcRegionRectangle.Right, (int)srcRegionRectangle.Bottom, 
                    pDstPixelData, (int)dstWidth, (int)dstBytesPerPixel,
                    (int)dstRegionRectangle.Left, (int)dstRegionRectangle.Top, (int)dstRegionRectangle.Right, (int)dstRegionRectangle.Bottom, 
                    swapXY, pLutData);
        }
    }
}

