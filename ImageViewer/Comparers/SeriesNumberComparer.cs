using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on series number.
	/// </summary>
	public class SeriesNumberComparer : DicomSeriesComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesNumberComparer"/>.
		/// </summary>
		public SeriesNumberComparer()
		{

		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on series number.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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
