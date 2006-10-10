using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
    public unsafe class ImageRenderer
    {
        public static void Render(
            ImageLayer imageLayer,
            IntPtr pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            RectangleF clientRectangle)
        {
            if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
                return;

            Rectangle srcViewableRectangle;
            Rectangle dstViewableRectangle;

            ImageRenderer.CalculateVisibleRectangles(imageLayer, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

            if (srcViewableRectangle.IsEmpty)
                return;

            byte[] srcPixelData = imageLayer.GetPixelData();

            byte[] lutData = null;
            int srcBytesPerPixel = imageLayer.BitsAllocated / 8;

            if (imageLayer.IsGrayscale)
                lutData = imageLayer.GrayscaleLUTPipeline.OutputLUT;

            bool swapXY = ImageRenderer.IsRotated(imageLayer);

            fixed (byte* pSrcPixelData = srcPixelData)
            {
                fixed (byte* pLutData = lutData)
                {
                    //Use this loop code to quickly determine how well a particular interpolator will perform.

                    //CodeClock clock = new CodeClock();
                    //clock.Start();

                    //int numimages = 1;
                    //if (_showMessageBox)
                    //    numimages = 50;

                    //for (int i = 0; i < numimages; ++i)
                    //{
                        ImageInterpolator.AllocateInterpolator(imageLayer.InterpolationMethod).Interpolate(
                            srcViewableRectangle,
                            pSrcPixelData,
                            imageLayer.Columns,
                            imageLayer.Rows,
                            srcBytesPerPixel,
                            dstViewableRectangle,
                            (byte*)pDstPixelData,
                            dstWidth,
                            dstBytesPerPixel,
                            swapXY,
                            pLutData,
                            imageLayer.IsColor,
                            imageLayer.IsPlanar,
                            imageLayer.IsSigned);
                    //}

                    //clock.Stop();

                    //string str = String.Format("50x Draw: {0}\n", clock.ToString());
                    //Trace.Write(str);
                    //if (_showMessageBox)
                    //    Platform.ShowMessageBox(str);

                    //_showMessageBox = false;
                }
            }
        }

        private static bool IsRotated(ImageLayer imageLayer)
        {
            float m12 = imageLayer.SpatialTransform.Transform.Elements[2];
            return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
        }

        private static void CalculateVisibleRectangles(
            ImageLayer imageLayer,
            RectangleF clientRectangle,
            out Rectangle dstVisibleRectangle,
            out Rectangle srcVisibleRectangle)
        {
            Rectangle srcRectangle = imageLayer.SpatialTransform.SourceRectangle;
            RectangleF dstRectangleF = imageLayer.SpatialTransform.ConvertToDestination(srcRectangle);

            // Find the intersection between the drawable client rectangle and
            // the transformed destination rectangle
            RectangleF dstVisibleRectangleF = RectangleUtilities.Intersect(clientRectangle, dstRectangleF);
            if (dstVisibleRectangleF.IsEmpty)
            {
                dstVisibleRectangle = Rectangle.Empty;
                srcVisibleRectangle = Rectangle.Empty;
                return;
            }

            // From that intersection, figure out what portion of the image
            // is Visible in source coordinates
            RectangleF srcVisibleRectangleF = imageLayer.SpatialTransform.ConvertToSource(dstVisibleRectangleF);

            dstVisibleRectangle = Rectangle.Round(dstVisibleRectangleF);
            srcVisibleRectangle = Rectangle.Round(srcVisibleRectangleF);

            dstVisibleRectangle = RectangleUtilities.MakeRectangleZeroBased(dstVisibleRectangle);
            srcVisibleRectangle = RectangleUtilities.MakeRectangleZeroBased(srcVisibleRectangle);
        }
    }

    /// <summary>
    /// A common (abstract) class for creating different interpolators.  Later, we can use an ExtensionPoint.
    /// </summary>
    public abstract class ImageInterpolator
    {
		public static ImageInterpolator AllocateInterpolator(ImageLayer.InterpolationMethods interpolationMethod)
        {
			if (interpolationMethod == ImageLayer.InterpolationMethods.BILINEAR)
                return new ImageInterpolatorBilinear();

			if (interpolationMethod == ImageLayer.InterpolationMethods.BILINEAR_FAST)
				return new ImageInterpolatorBilinearFast();

            return new ImageInterpolatorNearestNeighbour();
        }

        public abstract unsafe void Interpolate(
            Rectangle srcRegionRect,
            byte* pSrcPixelData,
            int srcWidth,
            int srcHeight,
            int srcBytesPerPixel,
            Rectangle dstRegionRect,
            byte* pDstPixelData,
            int dstWidth,
            int dstBytesPerPixel,
            bool swapXY,
            byte* pLutData,
            bool isRGB,
            bool isPlanar,
            bool IsSigned);
    }
}
