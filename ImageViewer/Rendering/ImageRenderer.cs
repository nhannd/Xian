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

        protected static bool IsRotated(ImageLayer imageLayer)
        {
            float m12 = imageLayer.SpatialTransform.Transform.Elements[2];
            return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
        }

        protected static void CalculateVisibleRectangles(
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

			// Round the rectangles, but round away from the centre of the rectangle,
			// rather than simply truncating the LTRB values.
			srcVisibleRectangleF = RectangleUtilities.RoundInflate(srcVisibleRectangleF);
			dstVisibleRectangleF = RectangleUtilities.RoundInflate(dstVisibleRectangleF);

			// Redo the intersection(s) after rounding, otherwise if you render to a bitmap
			// that is exactly the same size as the clientRectangle, the dstVisibleRectangle
			// will occasionally be outside the clientRectangle and cause a crash.
			dstVisibleRectangleF = RectangleUtilities.Intersect(dstVisibleRectangleF, clientRectangle);
			srcVisibleRectangleF = RectangleUtilities.Intersect(srcVisibleRectangleF, srcRectangle);

			// Just a formality now, these are already rounded.
			srcVisibleRectangle = Rectangle.Round(srcVisibleRectangleF);
			dstVisibleRectangle = Rectangle.Round(dstVisibleRectangleF);
		}
    }
}

//string str = String.Format("dstRectangle: {0}", dstRectangleF);
//Platform.Log(str);
//str = String.Format("dstViewableRectangle: {0}", dstViewableRectangle);
//Platform.Log(str);
//str = String.Format("srcViewableRectangle: {0}", srcViewableRectangle);
//Platform.Log(str);

//Matrix matrix = imageLayer.SpatialTransform.Transform;
//str = String.Format("matrix: {0},{1},{2},{3}", matrix.Elements[0], matrix.Elements[1], matrix.Elements[2], matrix.Elements[3]);
//Platform.Log(str);

			//    srcBytesPerPixel = imageLayer.BitsAllocated / 8;
			//}
			//else
			//{
			//    if (imageLayer.IsPlanar)
			//    {
			//        srcBytesPerPixel = imageLayer.BitsAllocated;
			//    }
			//    else
			//    {
			//        int samplesPerPixel = 3;
			//        srcStride = imageLayer.Columns * imageLayer.BitsAllocated / 8 * samplesPerPixel;
			//        srcBytesPerPixel = imageLayer.BitsAllocated * samplesPerPixel;
			//    }
			//}

			//int planeSize = srcStride * imageLayer.Rows;
