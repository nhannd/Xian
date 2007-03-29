using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	public class DisplaySetSortBySeriesNumber : IComparer<IDisplaySet>
	{
		public DisplaySetSortBySeriesNumber()
		{

		}

		#region IComparer<IDisplaySet> Members

		public int Compare(IDisplaySet x, IDisplaySet y)
		{
			if (x.PresentationImages.Count == 0 ||
				y.PresentationImages.Count == 0 )
				return 0;

			IImageSopProvider image1 = x.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider image2 = y.PresentationImages[0] as IImageSopProvider;

			if (image1 == null)
			{
				if (image2 == null)
					return 0; // x == y
				else
					return -1; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (image2 == null)
					return 1; // x < y (because we want it at the end for non-reverse sorting)
			}

			int seriesNumber1 = image1.ImageSop.SeriesNumber;
			int seriesNumber2 = image2.ImageSop.SeriesNumber;

			if (seriesNumber1 > seriesNumber2)
				return 1;
			else if (seriesNumber1 < seriesNumber2)
				return -1;
			else
				return 0;
		}

		#endregion
	}
}
