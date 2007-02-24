// ClearCanvas.ImageViewer.Renderer.BilinearInterpolation.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "BilinearInterpolation.h"
#include "math.h"
#include <memory>

#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

using std::auto_ptr;

#define SLIGHTLYGREATERTHANONE 1.001F
#define FIXEDPRECISION 7
#define FIXEDSCALE 128.0F

///////////////////////////////////////////////////////////////////////
///
/// Fixed Point (fast) bilinear interpolation algorithm.  Except
/// for the inner (x) loop, the function is the same as the RGB
/// function, but they have been implemented separately for the sake
/// of speed.  Basically, we don't want to have to check isRGB 
/// within the x-loop for every single pixel since it is unnecessary -
/// we only want to perform the operations that are absolutely
/// necessary within the inner (x) loop.
///
/// Some notes about fixed point math:
///   - Fixed point math is a way of performing floating point
///     arithmetic with integers.
///   - Take, for example, a 16 bit image where the bits stored is 16.
///     We need to be sure that we won't overflow the 32 bit integer
///     in which we are calculating the fixed point results. 
///   - Basically, a fixed point# is represented as an integer in the 
///     format I.F, where I is the number of bits representing the 
///     integer portion of the number, and F represents the fractional
///     part.
///   - When multiplying or dividing, the decimal place gets shifted to
///     the left depending on the # of fractional bits in the numbers
///     being multiplied together.  For example, if we multiply a 16.8
///     (I.F) number by another 16.8 number, we get a 32.16 (48 bits!!)
///     number as a result.
///   - In the function below, you will notice that 7 bits are used 
///     for the fractional portion of the #.  This is because we can 
///     guarantee that the dx and dy values are between 0 and 1, so
///     they are 0.7 #s.  And the pixel values (depending on the #bits
///     stored) CAN be as large as 16 bits.  Therefore, if we multiply
///     #s like this together, we get:
///            16.7 * 0.7 = 16.14 (30 bits).
///     Leaving 1 bit for the (potential) sign of the pixel value makes
///     the result a 31 bit number.  Therefore, 7 is the maximum value
///     we can use for this type of calculation without using a 64 bit
///     integer (this slows things down a lot!).
///   - Basically, what this means if we use 7 bits for the fractional
///     part of the #, is that we can only calculate #s to the resolution
///     of 1/128 ~ 0.007.  So, you can think of this interpolation
///     algorithm as one that subdivides each pixel into a 128x128 grid
///     and interpolates using a finite fraction for dx and dy 
///     (e.g. 1/128, 2/128, 3/128).
///   - What this means is that the algorithm is not a 'true' bilinear
///     interpolation algorithm, but for the most part it is very close.
///     The maximum absolute error can be calculated as follows:
///
///
///     Consider the following neighbourhood of 4 pixels:
///                  0      0
///              65535  65535
///
///     where dx = dy = 0.992187 (just less than 127/128)
///     
///     Using floating point arithmetic, 
///
///     yInterpolated1 = 65535 + (0-65535)*(0.992187) = 512.025
///     yInterpolated1 = 65535 + (0-65535)*(0.992187) = 512.025
///     yInterpolated  = 512.025
///
///     Using fixed point,
///
///     yInterpolated1 = 65535 + (0-65535)*(126/128) = 1023.984
///     yInterpolated1 = 65535 + (0-65535)*(126/128) = 1023.984
///     yInterpolated  = 1023.984
///
///     For this extreme case, we are out by ~512, which is less than
///     1% of the total range of pixel values.  This is a very extreme
///     case - in most situations, neighbouring pixel values are much
///     closer together than this and result in a much smaller error.
///
///     Consider also that the purpose of doing interpolation is simply
///     to smooth out the image.  Whether or not the algorithm 
///     does the calculation *exactly* is pretty much irrelevant.  The
///     interpolated data is only an approximation, and in most
///     situations (except perhaps at a sharp edge boundary) this 
///     approximation is reasonably accurate.
///
///     The error also decreases as the bits stored of the image
///     decreases.  For example, for a 14 bit image, the error would
///     be 512/4 = 128 for the extreme case shown above.
///
///////////////////////////////////////////////////////////////////////
template <class T, class U>
void InterpolateBilinearT(

		BYTE* pDstPixelData,

		unsigned int dstRegionWidth,
		unsigned int dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		T* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		int srcRegionOriginY,

		float xRatio,
		float yRatio,
		int* pLutData,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{
	// NY: Bug #295: When I originally changed this method so that
	// int pointers are used instead of byte pointers, I simply
	// incremented the pointer by 1 to go to the next pixel.  That obviously
	// doesn't work when the image has been rotated 90 deg.  And so we need
	// to use xDstIncrement when incrementing the pointer.  Problem is though,
	// xDstIncrement is in bytes.  So to compensate, we divide xDstIncrement
	// by 4 (i.e. right shift 2 bits) to get the increment in ints.
	xDstIncrement = xDstIncrement >> 2;

	float floatDstRegionHeight = (float)dstRegionHeight;
	float floatsrcRegionOriginY = (float)srcRegionOriginY;

    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;

	for (float y = 0; y < floatDstRegionHeight; ++y)  //so we're not constantly converting ints to floats.
	{
		float ySrcCoordinate = floatsrcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		int ySrcPixel = (int)ySrcCoordinate;
		int dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		int* pRowDstPixelData = (int*)pDstPixelData;
		T* pRowSrcPixelData = pSrcPixelData + ySrcPixel * srcWidth;
	    
		int* pxPixel = spxSrcPixels.get();
		int* pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
			T* pSrcPixel00 = pRowSrcPixelData + (*pxPixel);
			T* pSrcPixel01 = pSrcPixel00 + 1;
			T* pSrcPixel10 = pSrcPixel00 + srcWidth;
			T* pSrcPixel11 = pSrcPixel10 + 1;

			//wherever you multiply, you have to downshift again to keep the decimal precision of the #s the same.
			int yInterpolated1 = ((*pSrcPixel00) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
			int yInterpolated2 = ((*pSrcPixel01) << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);
			int iFinal = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;

			int value = *(pLutData + (U)iFinal);
			*pRowDstPixelData = value;

			pRowDstPixelData += xDstIncrement;

			++pxPixel;
			++pdxFixed;
		}

		pDstPixelData += yDstIncrement;
	}
}

void InterpolateBilinearRGB(

		BYTE* pDstPixelData,

		unsigned int dstRegionWidth,
		unsigned int dstRegionHeight,
		int xDstIncrement,
		int yDstIncrement,

		BYTE* pSrcPixelData,
		unsigned int srcWidth,
		unsigned int srcHeight,
		int srcRegionOriginY,

		int xSrcStride,
		int ySrcStride,
		unsigned int srcNextChannelOffset,

		float xRatio,
		float yRatio,

		std::auto_ptr<int>& spxSrcPixels,
		std::auto_ptr<int>& spdxFixedAtSrcPixelCoordinates)
{

	float floatDstRegionHeight = (float)dstRegionHeight;
	float floatsrcRegionOriginY = (float)srcRegionOriginY;
    float srcSlightlyLessThanHeightMinusOne = (float)srcHeight - SLIGHTLYGREATERTHANONE;

	for (float y = 0; y < floatDstRegionHeight; ++y)
	{
		float ySrcCoordinate = floatsrcRegionOriginY + (y + 0.5F) * yRatio;

		//a necessary evil, I'm afraid.
		if (ySrcCoordinate < 0)
			ySrcCoordinate = 0;
		else if (ySrcCoordinate > srcSlightlyLessThanHeightMinusOne)
			ySrcCoordinate = srcSlightlyLessThanHeightMinusOne; //force it to be just barely before the last pixel.

		int ySrcPixel = (int)ySrcCoordinate;
		int dyFixed = int((ySrcCoordinate - (float)ySrcPixel) * FIXEDSCALE);

		BYTE* pRowDstPixelData = pDstPixelData;
		BYTE* pRowSrcPixelData = pSrcPixelData + ySrcPixel * ySrcStride;
	    
		int* pxPixel = spxSrcPixels.get();
		int* pdxFixed = spdxFixedAtSrcPixelCoordinates.get();
		
		for (unsigned int x = 0; x < dstRegionWidth; ++x)
		{
            pRowDstPixelData[3] = 0xff;  //A

            BYTE* pSrcPixel00 = pRowSrcPixelData + (*pxPixel) * xSrcStride; 
    
            for (int i = 0; i < 3; ++i)
            {
                BYTE* pSrcPixel01 = pSrcPixel00 + xSrcStride;
                BYTE* pSrcPixel10 = pSrcPixel00 + ySrcStride;
                BYTE* pSrcPixel11 = pSrcPixel10 + xSrcStride;

				int yInterpolated1 = (*pSrcPixel00 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel10 - *pSrcPixel00) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int yInterpolated2 = (*pSrcPixel01 << FIXEDPRECISION) + ((dyFixed * ((*pSrcPixel11 - *pSrcPixel01) << FIXEDPRECISION)) >> FIXEDPRECISION);
				int IFinal = (yInterpolated1 + (((*pdxFixed) * (yInterpolated2 - yInterpolated1)) >> FIXEDPRECISION)) >> FIXEDPRECISION;

				//2-i because the destination pixel data goes BGRA and the source goes RGB
                pRowDstPixelData[2 - i] = (BYTE)(IFinal); //R(i=0), G(1), B(2)

                pSrcPixel00 += srcNextChannelOffset;
            }

			pRowDstPixelData += xDstIncrement;
			++pxPixel;
			++pdxFixed;
		}


		pDstPixelData += yDstIncrement;
	}
}


BOOL InterpolateBilinear
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
)
{
	unsigned int dstRegionHeight, dstRegionWidth,
	xDstStride, yDstStride,
	xDstIncrement, yDstIncrement;

    if (swapXY)
    {
        dstRegionHeight = abs(dstRegionRectRight - dstRegionRectLeft);
        dstRegionWidth = abs(dstRegionRectBottom - dstRegionRectTop);
        xDstStride = dstWidth * dstBytesPerPixel;
        yDstStride = dstBytesPerPixel;
		xDstIncrement = ((dstRegionRectBottom - dstRegionRectTop) < 0 ? -1: 1) * xDstStride;
		yDstIncrement = ((dstRegionRectRight - dstRegionRectLeft) < 0 ? -1: 1) * yDstStride;

		//Offset Top/Left by 1 if the height/width is negative.
		int zeroBasedTop = dstRegionRectTop;
		if (xDstIncrement < 0) //use the width because it's actually the height.
			--zeroBasedTop;

		int zeroBasedLeft = dstRegionRectLeft;
		if (yDstIncrement < 0) //use the height because it's actually the width.
			--zeroBasedLeft;

		pDstPixelData += (zeroBasedTop * xDstStride) + (zeroBasedLeft * yDstStride);
    }
    else
    {
        dstRegionHeight = abs(dstRegionRectBottom - dstRegionRectTop);
        dstRegionWidth = abs(dstRegionRectRight - dstRegionRectLeft);
        xDstStride = dstBytesPerPixel;
        yDstStride = dstWidth * dstBytesPerPixel;
        xDstIncrement = ((dstRegionRectRight - dstRegionRectLeft) < 0 ? -1: 1) * xDstStride;
        yDstIncrement = ((dstRegionRectBottom - dstRegionRectTop) < 0 ? -1: 1) * yDstStride;

		//Offset Top/Left by 1 if the height/width is negative.
		int zeroBasedTop = dstRegionRectTop;
		if (yDstIncrement < 0)
			--zeroBasedTop;

		int zeroBasedLeft = dstRegionRectLeft;
		if (xDstIncrement < 0)
			--zeroBasedLeft;

		pDstPixelData += (zeroBasedTop * yDstStride) + (zeroBasedLeft * xDstStride);
    }

    int srcRegionWidth = srcRegionRectRight - srcRegionRectLeft;
    int srcRegionHeight = srcRegionRectBottom - srcRegionRectTop;

    float srcSlightlyLessThanWidthMinusOne = (float)srcWidth - SLIGHTLYGREATERTHANONE;

    float xRatio = (float)srcRegionWidth / dstRegionWidth;
    float yRatio = (float)srcRegionHeight / dstRegionHeight;

	std::auto_ptr<int> spxSrcPixels(new int[dstRegionWidth]);
	int* pxPixel = spxSrcPixels.get();

	std::auto_ptr<int> spdxFixedAtSrcPixelCoordinates(new int[dstRegionWidth]);
	int * pdxFixed = spdxFixedAtSrcPixelCoordinates.get();

	float floatDstRegionWidth = (float)dstRegionWidth;
	float floatSrcRegionOriginX = (float)srcRegionRectLeft;

	for (float x = 0; x < floatDstRegionWidth; ++x)
	{
		float xCoord = floatSrcRegionOriginX + (x + 0.5F) * xRatio;

		//a necessary evil, I'm afraid.
		if (xCoord < 0)
			xCoord = 0;
		if (xCoord > srcSlightlyLessThanWidthMinusOne)
			xCoord = srcSlightlyLessThanWidthMinusOne; //force it to be just barely before the last pixel.

		*pxPixel = (int)xCoord;
		*pdxFixed = (int)((xCoord - (float)(*pxPixel)) * FIXEDSCALE);

		++pxPixel;
		++pdxFixed;
	}

	try
	{
		if (isRGB != FALSE)
		{
			int xSrcStride, ySrcStride, srcNextChannelOffset;

			if (!isPlanar)
			{
				xSrcStride = 3;
				ySrcStride = srcWidth * 3;
			}
			else
			{
				xSrcStride = 1;
				ySrcStride = srcWidth;
			}

			if (!isPlanar)
				srcNextChannelOffset = 1;
			else
				srcNextChannelOffset = srcWidth * srcHeight;

			InterpolateBilinearRGB(
					pDstPixelData,
					dstRegionWidth,
					dstRegionHeight,
					xDstIncrement,
					yDstIncrement,
					pSrcPixelData,
					srcWidth,
					srcHeight,
					srcRegionRectTop,
					xSrcStride,
					ySrcStride,
					srcNextChannelOffset,
					xRatio,
					yRatio,
					spxSrcPixels,
					spdxFixedAtSrcPixelCoordinates);
		}
		else
		{
			if (srcBytesPerPixel == 2)
			{
				if (isSigned == FALSE)
				{
					InterpolateBilinearT<unsigned short, unsigned short>
					(
						pDstPixelData,
						dstRegionWidth,
						dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						(unsigned short*)pSrcPixelData,
						srcWidth,
						srcHeight,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
				else
				{
					InterpolateBilinearT<short, unsigned short>
					(
						pDstPixelData,
						dstRegionWidth,
						dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						(short*)pSrcPixelData,
						srcWidth,
						srcHeight,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
			}
			else
			{
				if (isSigned == FALSE)
				{
					InterpolateBilinearT<BYTE, BYTE>
					(
						pDstPixelData,
						dstRegionWidth,
						dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						pSrcPixelData,
						srcWidth,
						srcHeight,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
				else
				{
					InterpolateBilinearT<char, BYTE>
					(
						pDstPixelData,
						dstRegionWidth,
						dstRegionHeight,
						xDstIncrement,
						yDstIncrement,
						(char*)pSrcPixelData,
						srcWidth,
						srcHeight,
						srcRegionRectTop,
						xRatio,
						yRatio,
						pLutData,
						spxSrcPixels,
						spdxFixedAtSrcPixelCoordinates);
				}
			}
		}
	
		return TRUE;
	}
	catch (...)
	{
	}

	return FALSE;
}
