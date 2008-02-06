using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public delegate bool IsPointInRoiDelegate(int x, int y);

	public static class RoiStatisticsCalculator
	{
		public static string Calculate(			
			InteractiveGraphic graphic, 
			IsPointInRoiDelegate isPointInRoi)
		{
			IImageGraphicProvider provider = graphic.ParentPresentationImage as IImageGraphicProvider;

			if (provider == null)
				return String.Empty;

			GrayscaleImageGraphic image = provider.ImageGraphic as GrayscaleImageGraphic;

			if (image == null)
				return String.Empty;

			string str;

			if (IsGraphicInImage(graphic, image))
			{
				int mean = CalculateMean(graphic, image, isPointInRoi);
				int stdDev = CalculateStandardDeviation(graphic, image, mean, isPointInRoi);

				if (IsCT(graphic))
				{
					str = String.Format(SR.ToolsMeasurementFormatMeanCT, mean) + "\n" +
					      String.Format(SR.ToolsMeasurementFormatStdDevCT, stdDev);
				}
				else
				{
					str = String.Format(SR.ToolsMeasurementFormatMean, mean) + "\n" +
						  String.Format(SR.ToolsMeasurementFormatStdDev, stdDev);
				}
			}
			else
			{
				str = String.Format(SR.ToolsMeasurementFormatMean, SR.NotApplicable) + "\n" +
					  String.Format(SR.ToolsMeasurementFormatStdDev, SR.NotApplicable);
			}

			return str;
		}

		private static bool IsCT(InteractiveGraphic graphic)
		{
			IImageSopProvider provider = graphic.ParentPresentationImage as IImageSopProvider;

			if (provider == null)
				return false;

			return provider.ImageSop.Modality == "CT";
		}

		private static int CalculateMean(
			InteractiveGraphic graphic,
			GrayscaleImageGraphic image,
			IsPointInRoiDelegate isPointInRoi)
		{
			Int64 sum = 0;
			int pixelCount = 0;

			image.PixelData.ForEachPixel(
				(int)graphic.BoundingBox.Left,
				(int)graphic.BoundingBox.Top,
				(int)graphic.BoundingBox.Right,
				(int)graphic.BoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							pixelCount++;
							// Make sure we run the raw pixel through the modality LUT
							// when doing the calculation. Note that the modality LUT
							// can be something other than a rescale intercept, so we can't
							// just run the mean through the LUT.
							sum += image.ModalityLut[image.PixelData.GetPixel(pixelIndex)];
						}
					});

			int mean;

			if (pixelCount == 0)
				mean = 0;
			else
				mean = (int)Math.Round(sum/(double)pixelCount);

			return mean;
		}

		private static int CalculateStandardDeviation(
			InteractiveGraphic graphic,
			GrayscaleImageGraphic image, 
			int mean,
			IsPointInRoiDelegate isPointInRoi)
		{
			Int64 sum = 0;
			int pixelCount = 0;
			int deviation;

			image.PixelData.ForEachPixel(
				(int)graphic.BoundingBox.Left,
				(int)graphic.BoundingBox.Top,
				(int)graphic.BoundingBox.Right,
				(int)graphic.BoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							pixelCount++;
							deviation = image.ModalityLut[image.PixelData.GetPixel(pixelIndex)] - mean;
							sum += deviation*deviation;
						}
					});

			int stdDev;

			if (pixelCount == 0)
				stdDev = 0;
			else
				stdDev = (int)Math.Round(Math.Sqrt(sum / (double)pixelCount));

			return stdDev;
		}

		private static bool IsGraphicInImage(InteractiveGraphic graphic, GrayscaleImageGraphic image)
		{
			RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(graphic.BoundingBox);

			if (boundingBox.Width == 0 || boundingBox.Height == 0)
				return false;

			if (boundingBox.Left < 0 ||
				boundingBox.Top < 0 ||
				boundingBox.Right > (image.Columns - 1) ||
				boundingBox.Bottom > (image.Rows - 1) )
				return false;

			return true;
		}

	}
}