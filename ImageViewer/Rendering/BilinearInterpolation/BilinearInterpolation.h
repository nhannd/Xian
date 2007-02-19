// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the BILINEARINTERPOLATION_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// BILINEARINTERPOLATION_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.

#ifdef BILINEARINTERPOLATION_EXPORTS
#define BILINEARINTERPOLATION_API __declspec(dllexport)
#else
#define BILINEARINTERPOLATION_API __declspec(dllimport)
#endif

extern "C"
{
	BILINEARINTERPOLATION_API BOOL InterpolateBilinear
	(
            BYTE* pSrcPixelData,

			unsigned int srcWidth,
            unsigned int srcHeight,
            unsigned int srcBytesPerPixel,

			BOOL isSigned,
			BOOL isRGB,
			BOOL isPlanar,

			int srcRegionRectLeft,
            int srcRegionRectTop,
            int srcRegionRectRight,
            int srcRegionRectBottom,
			
            BYTE* pDstPixelData,
            unsigned int dstWidth,
            unsigned int dstBytesPerPixel,

			int dstRegionRectLeft,
            int dstRegionRectTop,
            int dstRegionRectRight,
            int dstRegionRectBottom,

			BOOL swapXY,
            int* pLutData
	);
}
