#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{

    [TestFixture]
    public class FusionPresentationImageTests
    {
        [SetUp]
        public void Setup()
        {
            Platform.SetExtensionFactory(new UnitTestExtensionFactory());
        }

        [Test(Description = "Ensure OverlayImageGraphic.PixelData is based on the PET slides")]
        public void TestOverlayImageGraphicSignedCTSignedPT()
        {
            TestOverlayImageGraphic(true, true);
        }

        [Test(Description = "Ensure OverlayImageGraphic.PixelData is based on the PET slides")]
        public void TestOverlayImageGraphicSignedCTUnsignedPT()
        {
            TestOverlayImageGraphic(true, false);
        }

        [Test(Description = "Ensure OverlayImageGraphic.PixelData is based on the PET slides")]
        public void TestOverlayImageGraphicUnsignedCTSignedPT()
        {
            TestOverlayImageGraphic(false, true);
        }

        [Test(Description = "Ensure OverlayImageGraphic.PixelData is based on the PET slides")]
        public void TestOverlayImageGraphicUnsignedCTUnsignedPT()
        {
            TestOverlayImageGraphic(false, false);
        }

        private void TestOverlayImageGraphic(bool CTIsSigned, bool PETIsSigned)
        {
            // NOTE: There's no simple way to verify overlay is derived solely from PET slices.
            // In these tests, we fix the pixel values in all CT and PET slides (at different levels)
            // and verify that the pixel values in the overlay is the same as those in the PET slides.

            var CTPixelValue = CTIsSigned ? -1000f : 1000f;
            var PETPixelValue = PETIsSigned ? -100f : 100f;

            Assert.IsTrue(CTPixelValue != PETPixelValue, "Need to use different values for pet and ct pixels");

            // Slicer sets the background pixel values if it's outside the volume. 
            int CTBackgroundPixelValue = CTIsSigned ? (int)short.MinValue : (int)ushort.MinValue;
            int PETBackgroundPixelValue = PETIsSigned ? (int)short.MinValue : (int)ushort.MinValue;

            var baseSopDataSources = new UniformFunction(CTPixelValue).CreateSops(CTIsSigned, Modality.CT, new Vector3D(0.8f, 0.8f, 0.8f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit);
            var overlaySopDataSources = new UniformFunction(PETPixelValue).CreateSops(PETIsSigned, Modality.PT, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.zUnit, Vector3D.xUnit, Vector3D.yUnit);
            var testDisplaySetGenerator = new TestDisplaySetGenerator(baseSopDataSources, overlaySopDataSources);

            IDisplaySet displaySet = testDisplaySetGenerator.CreateFusionDisplaySet();

            try
            {

                foreach (var image in displaySet.PresentationImages)
                {
                    Assert.IsTrue(image is FusionPresentationImage);

                    var fusionImage = image as FusionPresentationImage;
                    var CTPixels = fusionImage.ImageGraphic.PixelData;

                    // Make sure all CT pixels are negative
                    CTPixels.ForEachPixel((i, x, y, pixelIndex) =>
                    {
                        var p = CTPixels.GetPixel(pixelIndex);

                        // Note: this could also mean the function that generates the pixels is incorrect.
                        Assert.IsTrue(p == CTPixelValue || p == CTBackgroundPixelValue, string.Format("CT pixel values are incorrect: Pixel index={0}, value={1}", pixelIndex, p));
                    });

                    // force it to draw 
                    using (fusionImage.DrawToBitmap(100, 100))
                    {
                        // Make sure all overlay pixels are positive
                        Assert.IsTrue(fusionImage.OverlayImageGraphic is GrayscaleImageGraphic);
                        var overlay = fusionImage.OverlayImageGraphic as GrayscaleImageGraphic;

                        var PETPixels = overlay.PixelData;
                        PETPixels.ForEachPixel((i, x, y, pixelIndex) =>
                        {
                            var p = PETPixels.GetPixel(pixelIndex);
                            p = (int)Math.Round(overlay.ModalityLut[p]);

                            // Note: this could also mean the function that generates the pixels is incorrect.
                            Assert.IsTrue(p == PETPixelValue || p == PETBackgroundPixelValue, string.Format("Overlay pixel values are incorrect: Pixel index={0}, value={1}", pixelIndex, p));
                        });
                    }
                }
            }
            finally
            {
                testDisplaySetGenerator.Dispose();
                displaySet.Dispose();

                foreach (var overlaySopDataSource in overlaySopDataSources)
                    overlaySopDataSource.Dispose();
                foreach (var baseSopDataSource in baseSopDataSources)
                    baseSopDataSource.Dispose();
            }

        }
    }
}

#endif