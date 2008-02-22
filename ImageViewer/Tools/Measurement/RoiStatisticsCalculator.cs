using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public delegate bool IsPointInRoiDelegate(int x, int y);

	public static class RoiStatisticsCalculator
	{
		public static string Calculate(RoiInfo roiInfo, IsPointInRoiDelegate isPointInRoi)
		{
			string str;

			bool isGrayscale = roiInfo.PixelData is GrayscalePixelData;

			if (isGrayscale && IsBoundingBoxInImage(roiInfo.BoundingBox, roiInfo.ImageColumns, roiInfo.ImageRows))
			{
				if (roiInfo.Mode == RoiAnalysisMode.Responsive)
				{
					return String.Format("{0} {1}\n{2} {1}", SR.ToolsMeasurementMean, SR.ToolsMeasurementNoValue, SR.ToolsMeasurementStdev);
				}

				int mean = CalculateMean(
					roiInfo.BoundingBox, 
					(GrayscalePixelData)roiInfo.PixelData, 
					roiInfo.ModalityLut, 
					isPointInRoi);
				
				int stdDev = CalculateStandardDeviation(
					mean, 
					roiInfo.BoundingBox, 
					(GrayscalePixelData)roiInfo.PixelData, 
					roiInfo.ModalityLut, 
					isPointInRoi);

				if (roiInfo.Modality == "CT")
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

		private static int CalculateMean
			(
				RectangleF roiBoundingBox,
				GrayscalePixelData pixelData, 
				IModalityLut modalityLut,
				IsPointInRoiDelegate isPointInRoi
			)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckForNullReference(isPointInRoi, "isPointInRoi");

			Int64 sum = 0;
			int pixelCount = 0;

			pixelData.ForEachPixel(
				(int)roiBoundingBox.Left,
				(int)roiBoundingBox.Top,
				(int)roiBoundingBox.Right,
				(int)roiBoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							++pixelCount;
							// Make sure we run the raw pixel through the modality LUT
							// when doing the calculation. Note that the modality LUT
							// can be something other than a rescale intercept, so we can't
							// just run the mean through the LUT.
							int value = pixelData.GetPixel(pixelIndex);
							if (modalityLut != null)
								value = modalityLut[value];
							
							sum += value;
						}
					});

			int mean;

			if (pixelCount == 0)
				mean = 0;
			else
				mean = (int)Math.Round(sum/(double)pixelCount);

			return mean;
		}

		private static int CalculateStandardDeviation
			(
				int mean,
				RectangleF roiBoundingBox,
				GrayscalePixelData pixelData, 
				IModalityLut modalityLut,
				IsPointInRoiDelegate isPointInRoi
			)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckForNullReference(isPointInRoi, "isPointInRoi");

			Int64 sum = 0;
			int pixelCount = 0;

			pixelData.ForEachPixel(
				(int)roiBoundingBox.Left,
				(int)roiBoundingBox.Top,
				(int)roiBoundingBox.Right,
				(int)roiBoundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							++pixelCount;
							int deviation = pixelData.GetPixel(pixelIndex);
							if (modalityLut != null)
								deviation = modalityLut[deviation];

							deviation -= mean;
							sum += deviation * deviation;
						}
					});

			int stdDev;

			if (pixelCount == 0)
				stdDev = 0;
			else
				stdDev = (int)Math.Round(Math.Sqrt(sum / (double)pixelCount));

			return stdDev;
		}

		private static bool IsBoundingBoxInImage(RectangleF boundingBox, float imageColumns, float imageRows)
		{
			boundingBox = RectangleUtilities.ConvertToPositiveRectangle(boundingBox);

			if (boundingBox.Width == 0 || boundingBox.Height == 0)
				return false;

			if (boundingBox.Left < 0 ||
				boundingBox.Top < 0 ||
				boundingBox.Right > (imageColumns - 1) ||
				boundingBox.Bottom > (imageRows - 1))
				return false;

			return true;
		}

	}
}