using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class SeriesNumberComparer : StandardDisplaySetComparer
	{
		public SeriesNumberComparer()
		{

		}

		#region IComparer<IDisplaySet> Members

		protected override int Compare(ImageSop x, ImageSop y)
		{
			int seriesNumber1 = x.SeriesNumber;
			int seriesNumber2 = y.SeriesNumber;

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
