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

#if	UNIT_TESTS

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
	//class MockImageGraphic : ImageGraphic
	//{
	//    public MockImageGraphic(
	//        int width,
	//        int height,
	//        int bitsAllocated,
	//        int bitsStored,
	//        int highBit,
	//        int samplesPerPixel,
	//        int pixelRepresentation,
	//        int planarConfiguration,
	//        PhotometricInterpretation photometricInterpretation,
	//        byte[] pixelData)
	//        : base(
	//            height, 
	//            width, 
	//            bitsAllocated, 
	//            bitsStored, 
	//            highBit, 
	//            samplesPerPixel, 
	//            pixelRepresentation, 
	//            planarConfiguration, 
	//            photometricInterpretation,
	//            pixelData)
	//    {
	//        InstallLut();
	//    }


	//    private void InstallLut()
	//    {
	//        if (this.IsColor)
	//            return;

	//        double rescaleSlope = 1.0;
	//        double rescaleIntercept = 0.0;

	//        ModalityLUTLinear modalityLUT =
	//            new ModalityLUTLinear(
	//            this.BitsStored,
	//            this.PixelRepresentation,
	//            rescaleSlope,
	//            rescaleIntercept);

	//        //this.GrayscaleLUTPipeline.ModalityLUT = modalityLUT;
	//        //VOILUTLinear linearLut = new VOILUTLinear(modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue);
	//        //linearLut.WindowWidth = 1 << this.BitsStored;
	//        //linearLut.WindowCenter = linearLut.WindowWidth / 2;
	//        //this.GrayscaleLUTPipeline.VoiLUT = linearLut;
	//        //this.GrayscaleLUTPipeline.Execute();
	//    }

	//    public override bool HitTest(Point point)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override void Move(SizeF delta)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//}

	//static class ImageLayerFactory
	//{
	//    public static MockImageGraphic CreateMonochrome8ImageLayer(int width, int height)
	//    {
	//        int bitsAllocated = 8;
	//        int bitsStored = 8;
	//        int highBit = 7;
	//        int samplesPerPixel = 1;
	//        int pixelRepresentation = 0;
	//        int planarConfiguration = 0;
	//        PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;
			
	//        return new MockImageGraphic(
	//            width, 
	//            height, 
	//            bitsAllocated,
	//            bitsStored,
	//            highBit, 
	//            samplesPerPixel, 
	//            pixelRepresentation, 
	//            planarConfiguration, 
	//            photometricInterpretation,
	//            null);
	//    }

	//    public static MockImageGraphic CreateMonochrome16ImageLayer(int width, int height)
	//    {
	//        int bitsAllocated = 16;
	//        int bitsStored = 16;
	//        int highBit = 15;
	//        int samplesPerPixel = 1;
	//        int pixelRepresentation = 0;
	//        int planarConfiguration = 0;
	//        PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Monochrome2;

	//        return new MockImageGraphic(
	//            width,
	//            height,
	//            bitsAllocated,
	//            bitsStored,
	//            highBit,
	//            samplesPerPixel,
	//            pixelRepresentation,
	//            planarConfiguration,
	//            photometricInterpretation,
	//            null);
	//    }

	//    public static MockImageGraphic CreateRGBTripletImageLayer(int width, int height)
	//    {
	//        int bitsAllocated = 8;
	//        int bitsStored = 8;
	//        int highBit = 7;
	//        int samplesPerPixel = 3;
	//        int pixelRepresentation = 0;
	//        int planarConfiguration = 0;
	//        PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Rgb;

	//        return new MockImageGraphic(
	//            width,
	//            height,
	//            bitsAllocated,
	//            bitsStored,
	//            highBit,
	//            samplesPerPixel,
	//            pixelRepresentation,
	//            planarConfiguration,
	//            photometricInterpretation,
	//            null);
	//    }

	//    public static MockImageGraphic CreateRGBPlanarImageLayer(int width, int height)
	//    {
	//        int bitsAllocated = 8;
	//        int bitsStored = 8;
	//        int highBit = 7;
	//        int samplesPerPixel = 3;
	//        int pixelRepresentation = 0;
	//        int planarConfiguration = 1;
	//        PhotometricInterpretation photometricInterpretation = PhotometricInterpretation.Rgb;

	//        return new MockImageGraphic(
	//            width,
	//            height,
	//            bitsAllocated,
	//            bitsStored,
	//            highBit,
	//            samplesPerPixel,
	//            pixelRepresentation,
	//            planarConfiguration,
	//            photometricInterpretation,
	//            null);
	//    }
	//}


	//static class ImageRendererTestUtilities
	//{
	//    public static Bitmap RenderLayer(MockImageGraphic layer, int dstWidth, int dstHeight)
	//    {
	//        Bitmap bitmap = new Bitmap(dstWidth, dstHeight);
	//        RectangleF clientArea = new RectangleF(0, 0, dstWidth, dstHeight);

	//        BitmapData bitmapData = LockBitmap(bitmap);
	//        int bytesPerPixel = 4;
	//        ImageRenderer.Render(layer, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);
	//        bitmap.UnlockBits(bitmapData);
	//        return bitmap;
	//    }

	//    public static void VerifyMonochromePixelValue16(int x, int y, int expectedPixelValue16, Bitmap bitmap)
	//    {
	//        int expectedPixelValue8 = expectedPixelValue16 / 256;

	//        VerifyMonochromePixelValue8(x, y, expectedPixelValue8, bitmap);
	//    }

	//    public static void VerifyMonochromePixelValue8(int x, int y, int expectedPixelValue8, Bitmap bitmap)
	//    {
	//        Color expectedPixelColor = Color.FromArgb(expectedPixelValue8, expectedPixelValue8, expectedPixelValue8);

	//        VerifyRGBPixelValue(x, y, expectedPixelColor, bitmap);
	//    }

	//    public static void VerifyRGBPixelValue(int x, int y, Color expectedPixelColor, Bitmap bitmap)
	//    {
	//        Color actualPixelColor = bitmap.GetPixel(x, y);
	//        Assert.AreEqual(expectedPixelColor, actualPixelColor);
	//    }

	//    private static BitmapData LockBitmap(Bitmap bitmap)
	//    {
	//        BitmapData bitmapData = bitmap.LockBits(
	//            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
	//            ImageLockMode.ReadWrite,
	//            bitmap.PixelFormat);

	//        return bitmapData;
	//    }

	//}
}
#endif